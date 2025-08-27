using System;

namespace Cards
{
    public enum Suit
    {
        Club, // ♣
        Diamond, // ♦
        Heart, // ♥
        Spade, // ♠
    }

    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum HandCategory
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        Straight = 4,
        Flush = 5,
        FullHouse = 6,
        FourOfAKind = 7,
        StraightFlush = 8,
        RoyalFlush = 9,
        FiveOfAKind = 10,
    }

    public static class CardsExtensions
    {
        public static string ToHumanString(this Suit suit) =>
            suit switch
            {
                Suit.Club => "♣",
                Suit.Diamond => "♦",
                Suit.Heart => "♥",
                Suit.Spade => "♠",
                _ => throw new NotImplementedException($"Not implemented for {suit}")
            };

        public static string ToHumanString(this Rank rank) =>
            rank switch
            {
                Rank.Two => "2",
                Rank.Three => "3",
                Rank.Four => "4",
                Rank.Five => "5",
                Rank.Six => "6",
                Rank.Seven => "7",
                Rank.Eight => "8",
                Rank.Nine => "9",
                Rank.Ten => "10",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                Rank.Ace => "A",
                _ => throw new NotImplementedException($"Not implemented for {rank}")
            };

        public static Suit ParseSuit(this char str) =>
            str switch
            {
                '♣' => Suit.Club,
                '♦' => Suit.Diamond,
                '♥' => Suit.Heart,
                '♠' => Suit.Spade,
                _ => throw new NotImplementedException($"Not implemented for {str}")
            };

        public static Rank ParseRank(this string str) =>
            str switch
            {
                "2" => Rank.Two,
                "3" => Rank.Three,
                "4" => Rank.Four,
                "5" => Rank.Five,
                "6" => Rank.Six,
                "7" => Rank.Seven,
                "8" => Rank.Eight,
                "9" => Rank.Nine,
                "10" => Rank.Ten,
                "J" => Rank.Jack,
                "Q" => Rank.Queen,
                "K" => Rank.King,
                "A" => Rank.Ace,
                _ => throw new NotImplementedException($"Not implemented for {str}")
            };
    }
}