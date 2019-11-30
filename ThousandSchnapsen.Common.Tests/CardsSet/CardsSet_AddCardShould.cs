using System;
using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_AddCardShould
    {
        [Fact]
        public void AddCard_CardOutOfSet_SetCardInCode()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            int cardId = 17;
            int expectedCode  = 0b010000100000010001000110;

            A.AddCard(cardId);

            Assert.Equal(A.Code, expectedCode);
        }

        [Fact]
        public void AddCard_CardInSet_DoNothing()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            int cardId = 6;
            int expectedCode  = 0b010000000000010001000110;

            A.AddCard(cardId);

            Assert.Equal(A.Code, expectedCode);
        }

        [Fact]
        public void AddCard_InvalidCardId_ThrowException()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            int cardId = 25;

            Assert.Throws<InvalidOperationException>(() => A.AddCard(cardId));
        }
    }
}
