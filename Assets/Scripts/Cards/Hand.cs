#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cards
{
    public class Hand : IEquatable<Hand>
    {
        public Hand(Card c1, Card c2, Card c3, Card c4, Card c5)
        {
            Cards = new List<Card> { c1, c2, c3, c4, c5 };
        }

        public Hand(string str)
        {
            var cardsStr = str.Split(',');
            if (cardsStr.Length != 5)
            {
                throw new ArgumentException("Invalid hand format");
            }

            Cards = cardsStr.Select(s => new Card(s)).ToList();
        }

        public List<Card> Cards { get; }


        public override string ToString() =>
            $"{string.Join(",", Cards.Select(c => c.ToString()))}";

        public bool Equals(Hand other) =>
            Cards.OrderBy(c => c) == other.Cards.OrderBy(c => c);

        public override bool Equals(object? obj) =>
            obj is Hand h && Cards.OrderBy(c => c) == h.Cards.OrderBy(c => c);

        public override int GetHashCode() =>
            HashCode.Combine(Cards);
    }
}