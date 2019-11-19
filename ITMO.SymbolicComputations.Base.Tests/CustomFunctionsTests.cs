using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class CustomFunctionsTests {
        public CustomFunctionsTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void BigSinFunction() {
            Symbol x = "x";

            var func = Fun[x, Times[Sin[Plus[x, -2]], 10]];
            var (steps, result) = func[2].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(0, result);
        }

        [Fact]
        public void TwoPowerX() {
            Symbol x = "x";

            var func = Fun[x, Power[2, x]];
            var (steps, result) = func[7].Visit(FullEvaluator.Default);

            _out.WriteLine(func.Visit(new MathematicaPrinter()));
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));

            Assert.Equal(128, result);
        }


        [Fact]
        public void XMinus2() {
            Symbol x = "x";

            var func = Fun[x, Plus[x, -2]];
            var (steps, result) = func[3].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(1, result);
        }

        [Fact]
        public void XPlusY() {
            Symbol x = "x";
            Symbol y = "y";

            var f = Fun;

            var func = f[x, f[y, Plus[x, y]]];
            var (steps, result) = func[2][3].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(5, result);
        }

        [Fact]
        public void XPlusYTimesZ() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var f = Fun;
            var e = Evaluate;

            var func = f[x, f[y, f[z, e[Times[Plus[x, y], z]]]]];
            var (steps, result) = func[2][3][5].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(25, result);
        }

        [Fact]
        public void XPower2() {
            Symbol x = "x";

            var func = Fun[x, Power[x, 2]];
            var ex = PowerImplementation[func[7]];
            var (steps, result) = ex.Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(49, result);
        }

        [Fact]
        public void XReturnsConstant() {
            Symbol x = "x";

            var func = Fun[x, 3];
            var (steps, result) = func[0].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(3, result);
        }

        [Fact]
        public void ZPowerXPlusY() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var f = Fun;

            var func = f[x, f[y, f[z, PowerImplementation[Power[z, Plus[x, y]]]]]];
            var (steps, result) = func[2][3][2].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(32, result);
        }
    }
}