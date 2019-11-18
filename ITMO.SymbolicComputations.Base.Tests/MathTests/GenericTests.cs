using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests {
    public sealed class GenericTests {
        public GenericTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void Example1() {
            Symbol x = "x";
            Symbol y = "y";

            var source = Times[BinaryPlus[x, y], BinaryPlus[x, y]];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(Power[BinaryPlus[x, y], 2], symbol);
        }

        [Fact]
        public void Example2() {
            Symbol x = "x";
            Symbol y = "y";

            var source = BinaryPlus[Power[x, y], Power[x, y]];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(Times[Power[x, y], 2], symbol);
        }
    }
}