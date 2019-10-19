using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class FlatTests {
        public FlatTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void FlatWorks() {
            var source = Plus[Plus["x"], Plus["y"]];
            var expression = source.Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Plus["x", "y"], expression);
        }

        [Fact]
        public void MoreComplexFlatWorks() {
            var source = Plus[Plus["a"], Plus["b", Plus["c", "d", Plus["e"]], "f"], "g", "h"];
            var expression = source.Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Plus["a", "b", "c", "d", "e", "f", "g", "h"], expression);
        }
    }
}