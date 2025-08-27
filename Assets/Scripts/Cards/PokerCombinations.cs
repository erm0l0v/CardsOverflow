#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cards
{
    public abstract class Combination
    {
        public abstract HandCategory Category { get; }

        public bool TryGetScore(CombinationsConfig config, Hand hand, out uint value)
        {
            var result = TryGetRaw(hand, out var raw);
            value = (config.GetScore(Category) << 20) | (raw & 0xFFFFF);
            return result;
        }

        protected abstract bool TryGetRaw(Hand hand, out uint value);

        protected static uint Pack(params int[] parts)
        {
            uint res = 0;
            for (int i = 0; i < 5; i++)
            {
                uint v = i < parts.Length ? (uint)parts[i] : 0;
                res = (res << 4) | (v & 0xF);
            }

            return res;
        }

        protected static int[] SortedRanksDesc(Hand h) =>
            h.Cards
                .Select(c => (int)c.Rank)
                .OrderByDescending(r => r)
                .ToArray();

        protected static bool IsFlush(Hand h) => h.Cards.All(c => c.Suit == h.Cards[0].Suit);

        protected static bool TryStraightHigh(Hand h, out int high)
        {
            var ranks = h.Cards
                .Select(c => (int)c.Rank)
                .Distinct()
                .OrderByDescending(r => r)
                .ToList();
            if (ranks.Count == 5)
            {
                bool seq = ranks
                    .Zip(ranks.Skip(1), (a, b) => a - b)
                    .All(d => d == 1);
                if (seq)
                {
                    high = ranks[0];
                    return true;
                }

                if (ranks.SequenceEqual(new[] { 14, 5, 4, 3, 2 }))
                {
                    high = 5;
                    return true;
                }
            }

            high = 0;
            return false;
        }

        protected static Dictionary<int, int> RankCounts(Hand h)
            => h.Cards
                .GroupBy(c => (int)c.Rank)
                .ToDictionary(g => g.Key, g => g.Count());
    }

    public sealed class RoyalFlush : Combination
    {
        public override HandCategory Category => HandCategory.RoyalFlush;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            if (!IsFlush(hand))
            {
                return false;
            }

            if (!TryStraightHigh(hand, out var high))
            {
                return false;
            }

            if (high == 14)
            {
                var raw = Pack(14, 13, 12, 11, 10);
                value = raw;
                return true;
            }

            return false;
        }
    }

    public sealed class StraightFlush : Combination
    {
        public override HandCategory Category => HandCategory.StraightFlush;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            if (!IsFlush(hand))
            {
                return false;
            }

            if (!TryStraightHigh(hand, out var high))
            {
                return false;
            }

            value = Pack(high);
            return true;
        }
    }

    public sealed class FiveOfAKind : Combination
    {
        public override HandCategory Category => HandCategory.FiveOfAKind;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            var counts = RankCounts(hand);
            var five = counts.FirstOrDefault(kv => kv.Value == 5).Key;
            if (five == 0)
            {
                return false;
            }

            value = Pack(five);
            return true;
        }
    }

    public sealed class FourOfAKind : Combination
    {
        public override HandCategory Category => HandCategory.FourOfAKind;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            var counts = RankCounts(hand);
            var quad = counts.FirstOrDefault(kv => kv.Value == 4).Key;
            if (quad == 0)
            {
                return false;
            }

            var kicker = hand.Cards
                .OrderByDescending(c => c.Value == quad)
                .Skip(4)
                .Select(c => c.Value)
                .Single();
            value = Pack(quad, kicker);
            return true;
        }
    }

    public sealed class FullHouse : Combination
    {
        public override HandCategory Category => HandCategory.FullHouse;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            var counts = RankCounts(hand);
            int three = counts
                .Where(kv => kv.Value == 3)
                .Select(kv => kv.Key)
                .DefaultIfEmpty(0)
                .Max();
            int pair = counts
                .Where(kv => kv.Value == 2)
                .Select(kv => kv.Key)
                .DefaultIfEmpty(0)
                .Max();
            if (three == 0 || pair == 0)
            {
                return false;
            }

            value = Pack(three, pair);
            return true;
        }
    }

    public sealed class Flush : Combination
    {
        public override HandCategory Category => HandCategory.Flush;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            if (!IsFlush(hand))
            {
                return false;
            }

            var r = SortedRanksDesc(hand);
            value = Pack(r[0], r[1], r[2], r[3], r[4]);
            return true;
        }
    }

    public sealed class Straight : Combination
    {
        public override HandCategory Category => HandCategory.Straight;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            if (!TryStraightHigh(hand, out var high))
            {
                return false;
            }

            value = Pack(high);
            return true;
        }
    }

    public sealed class ThreeOfAKind : Combination
    {
        public override HandCategory Category => HandCategory.ThreeOfAKind;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            var counts = RankCounts(hand);
            int three = counts
                .Where(kv => kv.Value == 3)
                .Select(kv => kv.Key)
                .DefaultIfEmpty(0)
                .Max();
            if (three == 0)
            {
                return false;
            }

            var kickers = hand.Cards
                .OrderByDescending(c => c.Value == three)
                .Skip(3)
                .Select(c => c.Value)
                .ToArray();
            value = Pack(three, kickers[0], kickers[1]);
            return true;
        }
    }

    public sealed class TwoPair : Combination
    {
        public override HandCategory Category => HandCategory.TwoPair;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            var counts = RankCounts(hand);
            var pairs = counts
                .Where(kv => kv.Value == 2)
                .Select(kv => kv.Key)
                .OrderByDescending(x => x)
                .ToArray();
            if (pairs.Length != 2)
            {
                return false;
            }

            int kicker = counts
                .Where(kv => kv.Value == 1)
                .Select(kv => kv.Key)
                .Single();
            value = Pack(pairs[0], pairs[1], kicker);
            return true;
        }
    }

    public sealed class OnePair : Combination
    {
        public override HandCategory Category => HandCategory.OnePair;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            value = 0;
            var counts = RankCounts(hand);
            int pair = counts
                .Where(kv => kv.Value == 2)
                .Select(kv => kv.Key)
                .DefaultIfEmpty(0)
                .Max();
            if (pair == 0)
            {
                return false;
            }

            var kickers = hand.Cards
                .OrderByDescending(c => c.Value == pair)
                .Skip(2)
                .Select(c => c.Value)
                .ToArray();
            value = Pack(pair, kickers[0], kickers[1], kickers[2]);
            return true;
        }
    }

    public sealed class HighCard : Combination
    {
        public override HandCategory Category => HandCategory.HighCard;

        protected override bool TryGetRaw(Hand hand, out uint value)
        {
            var r = SortedRanksDesc(hand);
            value = Pack(r[0], r[1], r[2], r[3], r[4]);
            return true;
        }
    }

    public sealed class HandScore : IComparable<HandScore>, IEquatable<HandScore>
    {
        public HandScore(HandCategory category, uint score)
        {
            HandCategory = category;
            Score = score;
        }

        public uint Score { get; }
        public HandCategory HandCategory { get; }

        public int CompareTo(HandScore other) =>
            Score.CompareTo(other.Score);

        public bool Equals(HandScore other) =>
            Score == other.Score;

        public override bool Equals(object? obj) =>
            obj is HandScore c && Score == c.Score;

        public override int GetHashCode() =>
            HashCode.Combine(HandCategory, Score);
    }

    public static class HandEvaluator
    {
        private static readonly Combination[] ByStrengthDesc =
        {
            new FiveOfAKind(),
            new RoyalFlush(),
            new StraightFlush(),
            new FourOfAKind(),
            new FullHouse(),
            new Flush(),
            new Straight(),
            new ThreeOfAKind(),
            new TwoPair(),
            new OnePair(),
            new HighCard(),
        };

        public static HandScore GetScore(this Hand hand, CombinationsConfig config)
        {
            HandScore maxScore = new HandScore(HandCategory.HighCard, 0);
            foreach (var comb in ByStrengthDesc)
            {
                if (comb.TryGetScore(config, hand, out var value))
                {
                    if (value > maxScore.Score)
                    {
                        maxScore = new HandScore(comb.Category, value);
                    }
                }
            }

            return maxScore;
        }
    }
}