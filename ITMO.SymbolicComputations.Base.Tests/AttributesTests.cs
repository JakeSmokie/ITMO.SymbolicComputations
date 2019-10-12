using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class AttributesTests {
        public AttributesTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void ConstInsidePlusIsReducedToConst() {
            var expression = Plus[Plus[Plus[Plus[2]]]]
                .Visit(new OneIdentityShrinker());

            Assert.Equal(2, expression);
        }
        
        [Fact]
        public void PlusInsidePlusIsReducedToOnePlus() {
            var expression = Plus[Plus[Plus[Plus[2, 3]]]]
                .Visit(new OneIdentityShrinker());
            
            Assert.Equal(Plus[2, 3], expression);
        }
    }
}