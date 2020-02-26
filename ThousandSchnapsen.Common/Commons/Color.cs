namespace ThousandSchnapsen.Common.Commons
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
        public static int GetPoints(this Color color) =>
            Constants.ColorValues[color];

        public static char ToSymbol(this Color color) =>
            color switch
            {
                Color.Clubs => '\u2667',
                Color.Spades => '\u2664',
                Color.Diamonds => '\u2666',
                Color.Hearts => '\u2665',
                _ => '-'
            };
    }
}