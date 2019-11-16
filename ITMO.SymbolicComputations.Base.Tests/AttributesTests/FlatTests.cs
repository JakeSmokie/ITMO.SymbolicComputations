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
            var source = BinaryPlus[BinaryPlus["x"], BinaryPlus["y"]];
            var expression = source.Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(BinaryPlus["x", "y"], expression);
        }

        [Fact]
        public void MoreComplexFlatWorks() {
            var source = BinaryPlus[BinaryPlus["a"], BinaryPlus["b", BinaryPlus["c", "d", BinaryPlus["e"]], "f"], "g", "h"];
            var expression = source.Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(BinaryPlus["a", "b", "c", "d", "e", "f", "g", "h"], expression);
        }
    }
}