using System.Linq;
using System.Collections.Generic;

namespace ThousandSchnapsen.Common.Commons
{
    public static class Constants
    {
        public const int PlayersCount = 4;
        public const int CardsCount = 24;
        public const int CardsInColorCount = 6;
        public const int CardsPerPlayerCount = 7;
        public const int MaxCardValue = 11;

        public static readonly Color[] Colors =
        {
            Color.Spades,
            Color.Clubs,
            Color.Diamonds,
            Color.Hearts
        };

        private static readonly Rank[] Ranks =
        {
            Rank.Nine,
            Rank.Ten,
            Rank.Jack,
            Rank.Queen,
            Rank.King,
            Rank.Ace
        };

        public static IEnumerable<Card> Deck =>
            Colors.SelectMany(color => Ranks.Select(rank => new Card(rank, color)));

        public static readonly Dictionary<Rank, int> RankValues = new Dictionary<Rank, int>()
        {
            {Rank.Nine, 0},
            {Rank.Jack, 2},
            {Rank.Queen, 3},
            {Rank.King, 4},
            {Rank.Ten, 10},
            {Rank.Ace, 11}
        };

        public static readonly Dictionary<Color, int> ColorValues = new Dictionary<Color, int>()
        {
            {Color.Spades, 40},
            {Color.Clubs, 60},
            {Color.Diamonds, 80},
            {Color.Hearts, 100},
        };
    }
}