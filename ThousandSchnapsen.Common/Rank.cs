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
        public static int GetPoints(this Rank rank)
        {
            switch (rank)
            {
                case Rank.Nine:
                    return 0;
                case Rank.Jack:
                    return 2;
                case Rank.Queen:
                    return 3;
                case Rank.King:
                    return 4;
                case Rank.Ten:
                    return 10;
                case Rank.Ace:
                    return 11;
            }

            return -1;
        }
    }
}