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
        const int MIN_COLOR_VALUE = 40;
        const int COLOR_VALUE_STEP = 20;
        public static int GetPoints(this Color color)
        {
            return MIN_COLOR_VALUE + ((int)color) * COLOR_VALUE_STEP;
        }
    }
}