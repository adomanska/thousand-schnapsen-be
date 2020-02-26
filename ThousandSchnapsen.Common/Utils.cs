using System;
using System.Linq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common
{
    internal static class Utils
    {
        public static string CreateTitle(string title, int lineWidth)
        {
            var dashesCount = (lineWidth - title.Length) / 2.0;
            var startDashes = new string('-', (int) Math.Floor(dashesCount));
            var endDashes = new string('-', (int) Math.Ceiling(dashesCount));
            return $"{startDashes}{title}{endDashes}";
        }

        public static Func<CardsSet, int> GetCardsSetsIndexer(int dealerId)
        {
            var indicesEnumerator = Enumerable.Range(0, Constants.PlayersCount)
                .Where(id => id != dealerId)
                .Append(dealerId)
                .GetEnumerator();
            return (cardsSet) =>
            {
                indicesEnumerator.MoveNext();
                return indicesEnumerator.Current;
            };
        }
    }
}