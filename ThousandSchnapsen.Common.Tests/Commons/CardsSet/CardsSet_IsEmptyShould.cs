using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_IsEmptyShould
    {
        [Fact]
        public void IsEmpty_EmptySet_ReturnTrue()
        {
            var cardsSetA = new CardsSet(new byte[] { });

            Assert.True(cardsSetA.IsEmpty);
        }

        [Fact]
        public void IsEmpty_NonEmptySet_ReturnFalse()
        {
            var cardsSetA = new CardsSet(new byte[] {2, 8, 16});

            Assert.False(cardsSetA.IsEmpty);
        }
    }
}