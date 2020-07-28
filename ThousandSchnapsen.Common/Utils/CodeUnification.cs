using System.Linq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.Utils
{
    public static class CodeUnification
    {
        public static int[] Unify(int[] data)
        {
            var presentColorsIds = Enumerable.Range(0, Constants.Colors.Length)
                .Reverse()
                .Where(colorId => data.Any(
                    code => (code & CardsSet.Color((Color) colorId).Code) != 0));

            var result = new int[data.Length];
            int i = 3;
            foreach (var color in presentColorsIds)
            {
                result = result.Select((code, index) =>
                {
                    var activeColor = (Color) color;
                    var colorMask = CardsSet.Color(activeColor).Code;
                    var fetchedColor = data[index] & colorMask;
                    var shiftedColorCards = fetchedColor << ((i - color) * 6);
                    return code | shiftedColorCards;
                }).ToArray();
                i--;
            }

            return result;
        }
    }
}