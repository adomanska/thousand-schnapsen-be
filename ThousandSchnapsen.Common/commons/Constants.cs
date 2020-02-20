using System.Linq;
using System.Collections.Generic;

namespace ThousandSchnapsen.Common
{
    public class Constants
    {
        public const int PLAYERS_COUNT = 4;
        public const int CARDS_COUNT = 24;
        public const int CARDS_IN_COLOR_COUNT = 6;
        public const int COLORS_COUNT = 4;
        public const int CARDS_PER_PLAYER_COUNT = 7;
        public const int REST_CARDS_COUNT = 3;
        public const int MAX_CARD_VALUE = 11;
        public static Color[] Colors = new Color[]{
            Color.Spades,
            Color.Clubs,
            Color.Diamonds,
            Color.Hearts
        };
        public static Rank[] Ranks = new Rank[]{
            Rank.Nine,
            Rank.Ten,
            Rank.Jack,
            Rank.Queen,
            Rank.King,
            Rank.Ace
        };
        public static Card[] Deck => 
            Colors.SelectMany(color => Ranks.Select(rank => new Card(rank, color))).ToArray();
        public static Dictionary<Rank, int> RankValues = new Dictionary<Rank, int>() {
            {Rank.Nine, 0},
            {Rank.Jack, 2},
            {Rank.Queen, 3},
            {Rank.King, 4},
            {Rank.Ten, 10},
            {Rank.Ace, 11}
        };
        public static Dictionary<Color, int> ColorValues = new Dictionary<Color, int>() {
            {Color.Spades, 40},
            {Color.Clubs, 60},
            {Color.Diamonds, 80},
            {Color.Hearts, 100},
        };
    }
}