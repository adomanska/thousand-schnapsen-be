namespace ThousandSchnapsen.Common
{
    public class Card
    {
        const int CARDS_IN_COLOR = 6;
        const int MAX_CARD_VALUE = 11;
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

        #nullable enable
        public Card? SecondMarriagePart
        {
            get 
            {
                switch(Rank)
                {
                    case Rank.Queen:
                        return new Card(Rank.King, Color);
                    case Rank.King:
                        return new Card(Rank.Queen, Color);
                    default:
                        return null;
                }
            }
        }

        public int Evaluate(Color firstColor, Color? trump)
        {
            if (trump.HasValue && trump.Value == Color)
                return 2 * (MAX_CARD_VALUE + 1) + Rank.GetPoints();
            else if (firstColor == Color)
                return (MAX_CARD_VALUE + 1) + Rank.GetPoints();
            else
                return Rank.GetPoints();
        }
    }
}