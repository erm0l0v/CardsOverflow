#nullable enable
using System;

namespace Cards
{
    public class Card : IComparable<Card>, IEquatable<Card>
    {
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Card(string str)
        {
            if (str.Length < 2)
            {
                throw new ArgumentException("String is too short");
            }

            Suit = str[^1].ParseSuit();
            Rank = str[..^1].ParseRank();
        }

        public Suit Suit { get; }
        public Rank Rank { get; }

        public int Value => (int)Rank;


        public override string ToString() =>
            $"{Rank.ToHumanString()} {Suit.ToHumanString()}";

        public int CompareTo(Card other) =>
            Rank == other.Rank ? Suit.CompareTo(other.Suit) : Rank.CompareTo(other.Rank);

        public bool Equals(Card other) =>
            Suit == other.Suit && Rank == other.Rank;

        public override bool Equals(object? obj) =>
            obj is Card c && Suit == c.Suit && Rank == c.Rank;

        public override int GetHashCode() =>
            HashCode.Combine(Suit, Rank);
    }
}