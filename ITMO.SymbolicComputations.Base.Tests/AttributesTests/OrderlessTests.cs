using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class OrderlessTests {
        public OrderlessTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        private static readonly StringSymbol Orderless = new StringSymbol("Orderless", Attributes.Orderless);

        [Fact]
        public void StringSymbolsOrderingWorks() {
            var source = Orderless["y", "x", "z"];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Orderless["x", "y", "z"], expression);
        }

        [Fact]
        public void ConstantOrderingWorks() {
            var source = Orderless["y", 30, "x", 10, "z", 60];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Orderless["x", "y", "z", 10, 30, 60], expression);
        }
    }
}