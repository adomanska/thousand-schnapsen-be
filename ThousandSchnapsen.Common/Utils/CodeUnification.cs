using System.Collections.Generic;
using System.Linq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.Utils
{
    public static class CodeUnification
    {
        private static readonly Dictionary<int, int> ColorMasks = new Dictionary<int, int>()
        {
            {0, 0b111111},
            {1, 0b111111000000},
            {2, 0b111111000000000000},
            {3, 0b111111000000000000000000},
        };
        public static int[] Unify(int[] data)
        {
            var aggregatedData = data.Aggregate((acc, code) => acc | code);
            var presentColorsIds = Enumerable.Range(0, Constants.Colors.Length)
                .Reverse()
                .Where(colorId => (aggregatedData & ColorMasks[colorId]) != 0);

            var i = 3;
            return presentColorsIds.Aggregate(new int[data.Length], (acc, colorId) =>
            {
                var result = acc
                    .Select((code, index) => 
                        code | ((data[index] & ColorMasks[colorId]) << ((i - colorId) * Constants.CardsInColorCount)))
                    .ToArray();
                i--;
                return result;
            });
        }
    }
}