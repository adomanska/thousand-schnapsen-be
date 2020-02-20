using System;
using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_GetCardsIdsShould
    {
        [Fact]
        public void GetCardsIds_EmptySet_ReturnEmptyCollection()
        {
            CardsSet A = new CardsSet();
            int[] expected = new int[] {};

            var result = A.GetCardsIds();

            Assert.Equal(result, expected);
        }

        [Fact]
        public void GetCardsIds_NonEmptySet_ReturnArrayOfIds()
        {
            int[] cardsIds = new int[] { 1, 2, 6, 10, 22 };
            CardsSet A = new CardsSet(cardsIds);
            int[] expected = cardsIds;

            var result = A.GetCardsIds();

            Assert.Equal(result, expected);
        }
    }
}
