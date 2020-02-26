namespace ThousandSchnapsen.Common.Commons
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

        public static string ToSymbol(this Rank rank) =>
            rank switch
            {
                Rank.Nine => " 9",
                Rank.Ten => "10",
                Rank.Jack => " J",
                Rank.Queen => " Q",
                Rank.King => " K",
                Rank.Ace => " A",
                _ => "--"
            };
    }
}