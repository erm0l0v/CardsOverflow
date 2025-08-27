using System;
using System.Collections.Generic;
using Cards;
using NUnit.Framework;

namespace Tests
{
    public class PokerHandsTests
    {
        private static readonly CombinationsConfig config =
            new CombinationsConfig(new Dictionary<HandCategory, uint>());

        private static uint ScoreOf(string s) => new Hand(s).GetScore(config).Score;

        private static bool LeftGreater(string left, string right) =>
            ScoreOf(left) > ScoreOf(right);

        [TestCase("A♠,K♠,Q♠,J♠,10♠", "9♠,8♠,7♠,6♠,5♠", ExpectedResult = true,
            TestName = "RoyalFlush > StraightFlush")]
        [TestCase("9♣,8♣,7♣,6♣,5♣", "K♦,K♣,K♥,K♠,2♣", ExpectedResult = true,
            TestName = "StraightFlush > FourOfAKind")]
        [TestCase("K♦,K♣,K♥,K♠,2♣", "A♦,A♣,A♥,K♠,K♣", ExpectedResult = true,
            TestName = "FourOfAKind > FullHouse")]
        [TestCase("A♦,A♣,A♥,K♠,K♣", "A♣,J♣,9♣,5♣,3♣", ExpectedResult = true,
            TestName = "FullHouse > Flush")]
        [TestCase("7♦,6♣,5♥,4♠,3♣", "K♣,J♦,9♥,6♠,2♣", ExpectedResult = true,
            TestName = "Straight > HighCard")]
        [TestCase("Q♦,Q♣,Q♥,A♠,K♣", "J♦,J♣,J♥,A♠,K♣", ExpectedResult = true,
            TestName = "ThreeOfAKind(Q) > ThreeOfAKind(J) (kickers eq)")]
        [TestCase("K♦,K♣,Q♥,Q♠,A♣", "K♥,K♠,J♦,J♣,A♦", ExpectedResult = true,
            TestName = "TwoPair(K,Q,A-kicker) > TwoPair(K,J,A-kicker)")]
        [TestCase("A♦,A♣,K♥,Q♠,J♣", "A♥,A♠,K♦,Q♣,10♦", ExpectedResult = true,
            TestName = "OnePair(A,K,Q,J) > OnePair(A,K,Q,10)")]
        public bool Compare_BetweenCategories(string left, string right) =>
            LeftGreater(left, right);

        [Test]
        public void FourOfAKind_Kicker_Decides()
        {
            var left = "9♦,9♣,9♥,9♠,K♣";
            var right = "9♦,9♣,9♥,9♠,A♣";
            Assert.Less(ScoreOf(left), ScoreOf(right));
        }

        [Test]
        public void FullHouse_TripRank_ComesBefore_PairRank()
        {
            var left = "A♦,A♣,A♥,2♠,2♣";
            var right = "K♦,K♣,K♥,Q♠,Q♣";
            Assert.Greater(ScoreOf(left), ScoreOf(right));
        }

        [Test]
        public void Flush_Lexicographic_Kickers()
        {
            var left = "A♣,J♣,9♣,5♣,3♣";
            var right = "A♦,J♦,9♦,5♦,2♦";
            Assert.Greater(ScoreOf(left), ScoreOf(right));
        }

        [Test]
        public void Straight_Wheel_A2345_Is_LowestStraight()
        {
            var wheel = "A♦,2♣,3♥,4♠,5♣";
            var sixHi = "6♦,5♣,4♥,3♠,2♣";
            Assert.Less(ScoreOf(wheel), ScoreOf(sixHi));
        }

        [Test]
        public void Straight_SameHigh_Is_Tie()
        {
            var s1 = "A♣,K♦,Q♥,J♠,10♣";
            var s2 = "A♦,K♣,Q♠,J♦,10♥";
            Assert.AreEqual(ScoreOf(s1), ScoreOf(s2));
        }

        [Test]
        public void Trips_Kickers_Compare_Descending()
        {
            var left = "7♣,7♦,7♥,A♠,K♣";
            var right = "7♣,7♦,7♥,A♠,Q♣";
            Assert.Greater(ScoreOf(left), ScoreOf(right));
        }

        [Test]
        public void TwoPair_TieOnPairs_Use_Kicker()
        {
            var left = "K♣,K♦,Q♣,Q♦,A♠";
            var right = "K♠,K♥,Q♥,Q♠,J♣";
            Assert.Greater(ScoreOf(left), ScoreOf(right));
        }

        [Test]
        public void OnePair_AllThreeKickers_Matter()
        {
            var left = "A♣,A♦,K♣,Q♦,J♠";
            var right = "A♠,A♥,K♦,Q♣,10♠";
            Assert.Greater(ScoreOf(left), ScoreOf(right));
        }

        [Test]
        public void HighCard_ExactTie()
        {
            var h1 = "A♣,K♦,9♥,5♠,3♣";
            var h2 = "A♦,K♣,9♠,5♥,3♦";
            Assert.AreEqual(ScoreOf(h1), ScoreOf(h2));
        }


        [Test]
        public void Hand_Order_DoesNotMatter()
        {
            var a = "A♣,K♦,Q♥,J♠,10♣";
            var b = "10♣,J♠,Q♥,K♦,A♣";
            Assert.AreEqual(ScoreOf(a), ScoreOf(b));
        }

        [Test]
        public void StraightFlush_Beats_FourOfAKind_Borderline()
        {
            var sf = "6♥,5♥,4♥,3♥,2♥";
            var fk = "A♦,A♣,A♥,A♠,K♣";
            Assert.Greater(ScoreOf(sf), ScoreOf(fk));
        }

        [Test]
        public void FullHouse_Beats_Flush()
        {
            var fh = "Q♦,Q♣,Q♥,2♠,2♣";
            var fl = "A♣,J♣,9♣,7♣,5♣";
            Assert.Greater(ScoreOf(fh), ScoreOf(fl));
        }

        [Test]
        public void RoyalFlush_Beats_StraightFlush_Borderline()
        {
            var rf = "A♦,K♦,Q♦,J♦,10♦";
            var sf = "6♥,5♥,4♥,3♥,2♥";
            Assert.Greater(ScoreOf(rf), ScoreOf(sf));
        }

        [Test]
        public void FiveOfAKind_Beats_RoyalFlush_Borderline()
        {
            var fk = "A♦,A♦,A♦,A♦,A♦";
            var rf = "A♦,K♦,Q♦,J♦,10♦";
            Assert.Greater(ScoreOf(fk), ScoreOf(rf));
        }

        [Test]
        public void InvalidRank_ShouldThrow()
        {
            Assert.Throws<NotImplementedException>(() => ScoreOf("1♣,K♦,Q♥,J♠,9♣"));
        }
    }
}