using System;
using ThousandSchnapsen.Common.Commons;
using Xunit;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Tests.States
{
    public class PublicState_StockColorShould
    {
        [Fact]
        public void StockColor_NonEmptyStock_ReturnFirstCardColor()
        {
            var publicState = new PublicState()
            {
                Stock = new[] {new StockItem(0, new Card(Rank.Ace, Color.Clubs))}
            };

            var result = publicState.StockColor;

            Assert.Equal(Color.Clubs, result);
        }

        [Fact]
        public void StockColor_EmptyStock_ThrowError()
        {
            var publicState = new PublicState();

            Assert.Throws<Exception>(() => publicState.StockColor);
        }
    }
}