using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class Rank_GetPointsShould
    {
        [Theory]
        [InlineData(Rank.Nine, 0)]
        [InlineData(Rank.Jack, 2)]
        [InlineData(Rank.Queen, 3)]
        [InlineData(Rank.King, 4)]
        [InlineData(Rank.Ten, 10)]
        [InlineData(Rank.Ace, 11)]
        public void GetPoints_SpecificRank_ReturnValidPoints(Rank rank, int expectedPoints)
        {
            Assert.Equal(expectedPoints, rank.GetPoints());
        }
    }
}