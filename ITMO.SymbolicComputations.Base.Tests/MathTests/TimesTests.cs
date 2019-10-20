using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests {
    public sealed class TimesTests {
        public TimesTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void ConstantsReduced() {
            var source = Times[1m, 3m, 5m, -6m];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(-90, symbol);
        }

        [Fact]
        public void TimesForSameSymbolCreatesPower() {
            Symbol x = "x";
            Symbol y = "y";

            var source = Times[y, x, x];
            var (steps, symbol) = source.Visit(new FullEvaluator());
            
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(Times[Power[x, 2], y], symbol);
        }
    }
}