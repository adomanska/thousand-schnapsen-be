using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class Color_GetPointsShould
    {
        [Theory]
        [InlineData(Color.Spades, 40)]
        [InlineData(Color.Clubs, 60)]
        [InlineData(Color.Diamonds, 80)]
        [InlineData(Color.Hearts, 100)]
        public void GetPoints_SpecificColor_ReturnValidPoints(Color color, int expectedPoints)
        {
            Assert.Equal(expectedPoints, color.GetPoints());
        }
    }
}