using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class FlatTests {
        public FlatTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void FlatWorks() {
            var flat = new StringSymbol("FlatTest", Attributes.Flat);
            
            var source = flat[flat[1], flat[2, flat[3, 4, flat[5]], 6], 7, 8];
            var expression = source.Visit(new FullEvaluator()).Symbol;

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(flat[1, 2, 3, 4, 5, 6, 7, 8], expression);
        }
    }
}