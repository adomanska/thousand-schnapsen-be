namespace ThousandSchnapsen.Common.Commons
{
    public struct Card
    {
        public Card(byte cardId) =>
            CardId = cardId;

        public Card(Rank rank, Color color) :
            this((byte)((int)color * Constants.CardsInColorCount + (int)rank))
        {
        }

        public byte CardId { get; }
        public Color Color => (Color) (CardId / Constants.CardsInColorCount);
        public Rank Rank => (Rank) (CardId % Constants.CardsInColorCount);
        public bool IsPartOfMarriage => Rank == Rank.Queen || Rank == Rank.King;

        public Card? SecondMarriagePart
        {
            get
            {
                return Rank switch
                {
                    Rank.Queen => new Card(Rank.King, Color),
                    Rank.King => new Card(Rank.Queen, Color),
                    _ => (Card?) null
                };
            }
        }

        public int GetValue(Color firstColor, Color? trump)
        {
            if (trump.HasValue && trump.Value == Color)
                return 2 * (Constants.MaxCardValue + 1) + Rank.GetPoints();
            if (firstColor == Color)
                return (Constants.MaxCardValue + 1) + Rank.GetPoints();
            return Rank.GetPoints();
        }

        public override string ToString() =>
            $"{Color.ToSymbol()}{Rank.ToSymbol()}";
    }
}