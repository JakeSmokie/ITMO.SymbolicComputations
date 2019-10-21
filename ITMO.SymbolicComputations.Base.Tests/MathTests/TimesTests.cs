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
        
        [Fact]
        public void ComplexEvaluationIsOkay() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var source = Times[x, x, y, z, Power[x, 3], Power[y, y], Power[Times[x, y, x], 5]];
            var (steps, symbol) = source.Visit(new FullEvaluator());
            
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(Times[Power[x, 15], Power[y, Plus[y, 6]], z], symbol);
        }
        
        [Fact]
        public void ATimesZeroEqualsZero() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var source = Times[x, x, y, z, 15, 0, -10, x];
            var (steps, symbol) = source.Visit(new FullEvaluator());
            
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(0, symbol);
        }
       
    }
}