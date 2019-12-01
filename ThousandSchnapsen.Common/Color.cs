namespace ThousandSchnapsen.Common
{
    public enum Color
    {
        Spades,
        Clubs,
        Diamonds,
        Hearts
    }

    public static class ColorMethods
    {
        public static int GetPoints(this Color color)
        {
            return 40 + ((int)color) * 20;
        }
    }
}