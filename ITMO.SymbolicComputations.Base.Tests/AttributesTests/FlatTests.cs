using ITMO.SymbolicComputations.Base.Models;
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
            var source = ListPlus[ListPlus[1], ListPlus[2, ListPlus[3, 4, ListPlus[5]], 6], 7, 8];
            var expression = source.Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(36, expression);
        }
    }
}