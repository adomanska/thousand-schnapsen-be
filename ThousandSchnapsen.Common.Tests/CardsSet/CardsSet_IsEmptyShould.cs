using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_IsEmptyShould
    {
        [Fact]
        public void IsEmpty_EmptySet_ReturnTrue()
        {
            CardsSet A = new CardsSet(new int[] { });

            Assert.True(A.IsEmpty);
        }

        [Fact]
        public void IsEmpty_NonEmptySet_ReturnFalse()
        {
            CardsSet A = new CardsSet(new int[] { 2, 8, 16 });

            Assert.False(A.IsEmpty);
        }
    }
}
