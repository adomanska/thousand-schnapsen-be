using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_GetCardsIdsShould
    {
        [Fact]
        public void GetCardsIds_EmptySet_ReturnEmptyCollection()
        {
            var cardsSetA = new CardsSet();
            var expected = new int[] { };

            var actual = cardsSetA.GetCardsIds();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetCardsIds_NonEmptySet_ReturnArrayOfIds()
        {
            var cardsIds = new[] {1, 2, 6, 10, 22};
            var cardsSetA = new CardsSet(cardsIds);
            var expected = cardsIds;

            var actual = cardsSetA.GetCardsIds();

            Assert.Equal(expected, actual);
        }
    }
}