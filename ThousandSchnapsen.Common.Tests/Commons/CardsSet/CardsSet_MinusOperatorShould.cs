using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_MinusOperatorShould
    {
        [Fact]
        public void MinusOperator_DisjunctiveSets_ReturnValidSet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new[] {0, 8, 13, 17, 20});
            const int expected = 0b010000000000010001000110;

            var cardsSetC = cardsSetA - cardsSetB;

            Assert.Equal(expected, cardsSetC.Code);
        }

        [Fact]
        public void MinusOperator_IntersectingSets_ReturnValidSet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new[] {0, 2, 6, 17, 20});
            const int expected = 0b010000000000010000000010;

            var cardsSetC = cardsSetA - cardsSetB;

            Assert.Equal(expected, cardsSetC.Code);
        }
    }
}