using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_PlusOperatorShould
    {
        [Fact]
        public void PlusOperator_DisjunctiveSets_ReturnValidSet()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            CardsSet B = new CardsSet(new int[]{0, 8, 13, 17, 20});
            int expectedCode  = 0b010100100010010101000111;

            CardsSet C = A + B;

            Assert.Equal(C.Code, expectedCode);
        }

        [Fact]
        public void PlusOperator_IntersectingSets_ReturnValidSet()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            CardsSet B = new CardsSet(new int[]{0, 2, 6, 17, 20});
            int expectedCode  = 0b010100100000010001000111;

            CardsSet C = A + B;

            Assert.Equal(C.Code, expectedCode);
        }

        [Fact]
        public void PlusOperator_OneEmptySet_ReturnValidSet()
        {
            CardsSet A = new CardsSet(new int[]{1, 2, 6, 10, 22});
            CardsSet B = new CardsSet(new int[]{});
            int expectedCode  = 0b010000000000010001000110;

            CardsSet C = A + B;

            Assert.Equal(C.Code, expectedCode);
        }
    }
}
