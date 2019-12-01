namespace ThousandSchnapsen.Common
{
    public class Card
    {
        const int CARDS_IN_COLOR = 6;
        public Card(int cardId)
        {
            CardId = cardId;
        }

        public Card(Rank rank, Color color)
        {
            CardId = (int)color * CARDS_IN_COLOR + (int)rank;
        }

        public int CardId { get; }
        public Color Color => (Color)(CardId / CARDS_IN_COLOR);
        public Rank Rank => (Rank)(CardId % CARDS_IN_COLOR);

        public bool IsPartOfMarriage => Rank == Rank.Queen || Rank == Rank.King;

        public Card? SecondMarriagePart => null;

        public int Evaluate(Color firstColor, Color? trump)
        {
            if (trump.HasValue && trump.Value == Color)
                return 24 + Rank.GetPoints();
            else if (firstColor == Color)
                return 12 + Rank.GetPoints();
            else
                return Rank.GetPoints();
        }
    }
}