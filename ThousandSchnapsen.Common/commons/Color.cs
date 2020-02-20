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
        public static int GetPoints(this Color color) =>
            Constants.ColorValues[color];

        public static char ToSymbol(this Color color)
        {
            switch(color)
            {
                case Color.Clubs:
                    return '\u2667';
                case Color.Spades:
                    return '\u2664';
                case Color.Diamonds:
                    return '\u2666';
                case Color.Hearts:
                    return '\u2665';
                default:
                    return '-';
            }
        }
    }
}