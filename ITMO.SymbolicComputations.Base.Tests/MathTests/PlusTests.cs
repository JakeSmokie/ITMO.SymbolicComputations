using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests
{
    public sealed class PlusTests {
        public PlusTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void ConstantsReduced() {
            var source = Plus[1m, 3m, 5m, 6m];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(15, symbol);
        }
        
        [Fact]
        public void ConstantsReduced2() {
            var source = Plus[1, 3, 4, 5, Plus[9, -7]];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(15, symbol);
        }

        [Fact]
        public void PlusForSameSymbolCreatesTimes() {
            Symbol x = "x";
            Symbol y = "y";

            var source = Plus[y, x, x];
            var (steps, symbol) = source.Visit(new FullEvaluator());
            
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(Plus[Times[x, 2], y], symbol);
        }

        [Fact]
        public void PlusForSameSymbolCreatesTimes2() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var source = Plus[x, y, z, z, x, x, y];
            var (steps, symbol) = source.Visit(new FullEvaluator());
            
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(Plus[Times[x, 3], Times[y, 2], Times[z, 2]], symbol);
        }
    }
}