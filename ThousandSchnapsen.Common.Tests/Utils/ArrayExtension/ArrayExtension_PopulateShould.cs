using System;
using ThousandSchnapsen.Common.Utils;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Utils
{
    public class ArrayExtension_PopulateShould
    {
        [Fact]
        public void Populate_NoArgProvider_FillArrayProperly()
        {
            Func<int, int> provider = _ => 1;
            var expected = new[] {1, 1, 1};

            var result = new int[3].Populate(provider);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Populate_OneArgProvider_FillArrayProperly()
        {
            Func<int, int> provider = id => id + 1;
            var expected = new[] {1, 2, 3};

            var result = new int[3].Populate(provider);

            Assert.Equal(expected, result);
        }
    }
}