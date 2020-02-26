using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_OrOperatorShould
    {
        [Fact]
        public void OrOperator_DisjunctiveSets_ReturnValidSet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new[] {0, 8, 13, 17, 20});
            const int expected = 0b010100100010010101000111;

            var cardsSetC = cardsSetA | cardsSetB;

            Assert.Equal(expected, cardsSetC.Code);
        }

        [Fact]
        public void OrOperator_IntersectingSets_ReturnValidSet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new[] {0, 2, 6, 17, 20});
            const int expected = 0b010100100000010001000111;

            var cardsSetC = cardsSetA | cardsSetB;

            Assert.Equal(expected, cardsSetC.Code);
        }

        [Fact]
        public void OrOperator_OneEmptySet_ReturnValidSet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new int[] { });
            const int expected = 0b010000000000010001000110;

            var cardsSetC = cardsSetA | cardsSetB;

            Assert.Equal(expected, cardsSetC.Code);
        }
    }
}