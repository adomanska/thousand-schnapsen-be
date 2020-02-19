using System.Linq;

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
    }
}
