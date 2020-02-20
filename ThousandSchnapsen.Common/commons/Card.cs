#nullable enable

namespace ThousandSchnapsen.Common
{
    public class Card
    {
        public Card(int cardId) =>
            CardId = cardId;

        public Card(Rank rank, Color color): 
            this((int)color * Constants.CARDS_IN_COLOR_COUNT + (int)rank) { }

        public int CardId { get; }
        public Color Color => (Color)(CardId / Constants.CARDS_IN_COLOR_COUNT);
        public Rank Rank => (Rank)(CardId % Constants.CARDS_IN_COLOR_COUNT);
        public bool IsPartOfMarriage => Rank == Rank.Queen || Rank == Rank.King;
        public Card? SecondMarriagePart
        {
            get
            {
                switch (Rank)
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

        public int GetValue(Color firstColor, Color? trump)
        {
            if (trump.HasValue && trump.Value == Color)
                return 2 * (Constants.MAX_CARD_VALUE + 1) + Rank.GetPoints();
            else if (firstColor == Color)
                return (Constants.MAX_CARD_VALUE + 1) + Rank.GetPoints();
            else
                return Rank.GetPoints();
        }

        public Card Clone() => 
            new Card(CardId);

        public override string ToString() =>
            $"{Color.ToSymbol()}{Rank.ToSymbol()}";
    }
}