using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_AndOperatorShould
    {
        [Fact]
        public void AndOperator_DisjunctiveSets_ReturnEmptySet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new[] {0, 8, 13, 17, 20});

            var cardsSetC = cardsSetA & cardsSetB;

            Assert.True(cardsSetC.IsEmpty);
        }

        [Fact]
        public void AndOperator_IntersectingSets_ReturnValidSet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new[] {0, 2, 6, 17, 20});
            const int expectedCode = 0b000000000000000001000100;

            var cardsSetC = cardsSetA & cardsSetB;

            Assert.Equal(cardsSetC.Code, expectedCode);
        }

        [Fact]
        public void PlusOperator_OneEmptySet_ReturnEmptySet()
        {
            var cardsSetA = new CardsSet(new[] {1, 2, 6, 10, 22});
            var cardsSetB = new CardsSet(new int[] { });

            var cardsSetC = cardsSetA & cardsSetB;

            Assert.True(cardsSetC.IsEmpty);
        }
    }
}