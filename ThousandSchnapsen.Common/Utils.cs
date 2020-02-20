using System;
using System.Linq;

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

        public static Func<CardsSet, int> GetCardsSetsIndexer(int dealerId)
        {
            var indicesEnumerator = Enumerable.Range(0, Constants.PLAYERS_COUNT)
                .Where(id => id != dealerId)
                .Append(dealerId)
                .GetEnumerator();
            return (cardsSet) => {;
                indicesEnumerator.MoveNext();
                return indicesEnumerator.Current;
            };
        }
    }
}
