using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class OneIdentityTests {
        public OneIdentityTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void ConstInsidePlusIsReducedToConst() {
            var expression = Plus[Plus[Plus[Plus[2]]]]
                .Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(2, expression);
        }

        [Fact]
        public void PlusInsidePlusIsReducedToOnePlus() {
            var expression = Plus[Plus[Plus[Plus["x", "y"]]]]
                .Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Plus["x", "y"], expression);
        }
    }
}