using System;
using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_ColorShould
    {
        [Fact]
        public void Color_Always_ReturnColorCardSet()
        {
            Color color = Color.Diamonds;
            int expectedCode = 0b000000111111000000000000;

            var colorCardsSet = CardsSet.Color(color);

            Assert.Equal(expectedCode, colorCardsSet.Code);
        }
    }
}
