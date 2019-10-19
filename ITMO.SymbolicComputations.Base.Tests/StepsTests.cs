using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class StepsTests {
        public StepsTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;
        private static readonly StringSymbol Orderless = new StringSymbol("Orderless", Attributes.Orderless);

        
        [Fact]
        public void NestedOrderingWorks() {
            var source = Orderless["y", Orderless[1, "x", Orderless["y"]], "x", "z"];
            var steps = source.Visit(new FullEvaluator()).Steps;

            steps.ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
        }

    }
}