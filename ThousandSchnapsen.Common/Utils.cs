using System;

namespace ThousandSchnapsen.Common
{
    static class Utils
    {
        public static string CreateTitle(string title, int lineWidth)
        {
            double dashesCount = (lineWidth - title.Length) / 2;
            string startDashes = new String('-', (int)Math.Floor(dashesCount));
            string endDashes = new String('-', (int)Math.Ceiling(dashesCount));
            return $"{startDashes}{title}{endDashes}";
        }
    }
}
