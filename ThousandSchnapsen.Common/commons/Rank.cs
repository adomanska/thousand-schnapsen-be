namespace ThousandSchnapsen.Common
{
    public enum Rank
    {
        Nine,
        Jack,
        Queen,
        King,
        Ten,
        Ace
    }

    public static class RankMethods
    {
        public static int GetPoints(this Rank rank) =>
            Constants.RankValues[rank];

        public static string ToSymbol(this Rank rank)
        {
            switch(rank)
            {
                case Rank.Nine:
                    return " 9";
                case Rank.Ten:
                    return "10";
                case Rank.Jack:
                    return " J";
                case Rank.Queen:
                    return " Q";
                case Rank.King:
                    return " K";
                case Rank.Ace:
                    return " A";
                default:
                    return "--";
            }
        }
    }
}