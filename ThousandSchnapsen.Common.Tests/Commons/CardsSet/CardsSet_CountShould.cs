using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_CountShould
    {
        [Theory]
        [InlineData(0b00110010110, 5)]
        [InlineData(0b0111001001000111, 8)]
        [InlineData(0b0, 0)]
        [InlineData(0b111111111111111111111111, 24)]
        public void Count_ReturnValidCardsCount(int code, int expected)
        {
            var result = new CardsSet(code).Count;

            Assert.Equal(expected, result);
        }
    }
}