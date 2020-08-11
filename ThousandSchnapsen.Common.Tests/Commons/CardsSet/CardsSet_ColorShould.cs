using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_ColorShould
    {
        [Fact]
        public void Color_Always_ReturnColorCardSet()
        {
            const Color color = Color.Diamonds;
            const int expected = 0b000000111111000000000000;

            var colorCardsSet = CardsSet.Color(color);

            Assert.Equal(expected, colorCardsSet.Code);
        }
    }
}