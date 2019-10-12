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
            var source = Plus[Plus[2], Plus[3]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Plus[2, 3], expression);
        }

        [Fact]
        public void MoreComplexFlatWorks() {
            var source = Plus[Plus[2], Plus[3, Plus[40, 50, Plus[60]], -20], 9, 10];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Plus[2, 3, 40, 50, 60, -20, 9, 10], expression);
        }
    }
}