using ThousandSchnapsen.Common.Utils;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Utils
{
    public class CodeUnification_UnifyShould
    {
        [Fact]
        public void Unify_NoUnifyableSets_ReturnsUnchangedCodes()
        {
            var data = new[]
            {
                0b011000,
                0b011000000000,
                0b011000000000000000,
                0b011000000000000000000000,
            };
            var expected = data;

            var result = CodeUnification.Unify(data);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Unify_NoFirstColorSet_ReturnsShiftedCodes()
        {
            var data = new[]
            {
                0b011000,
                0b011000000000,
                0b011000000000000000,
                0b000110,
            };
            var expected = new[]
            {
                0b011000000000,
                0b011000000000000000,
                0b011000000000000000000000,
                0b000110000000,
            };

            var result = CodeUnification.Unify(data);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Unify_NoTwoColorsSets_ReturnsShiftedCodes()
        {
            var data = new[]
            {
                0b011000,
                0b001100000000000000,
                0b011000000000000000,
                0b000110,
            };
            var expected = new[]
            {
                0b011000000000000000,
                0b001100000000000000000000,
                0b011000000000000000000000,
                0b000110000000000000,
            };

            var result = CodeUnification.Unify(data);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Unify_MoreThanTwoEmptyColors_ReturnsShiftedCodes()
        {
            var data = new[]
            {
                0b011000,
                0b000110,
            };
            var expected = new[]
            {
                0b011000000000000000000000,
                0b000110000000000000000000,
            };

            var result = CodeUnification.Unify(data);

            Assert.Equal(expected, result);
        }
    }
}