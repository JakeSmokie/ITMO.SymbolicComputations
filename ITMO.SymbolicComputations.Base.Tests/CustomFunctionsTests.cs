using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class CustomFunctionsTests {
        public CustomFunctionsTests(ITestOutputHelper output) {
            @out = output;
            evaluateAndAssert = Test.CreateAsserter(output, Seq[Set[TaylorSin, TaylorSinImplementation]]);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;
        private readonly ITestOutputHelper @out;

        [Fact]
        public void BigSinFunction() {
            Symbol x = "x";

            var func = Fun[x, Times[Sin[Plus[x, -2]], 10]];
            var (steps, result) = func[2].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => @out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(0, result);
        }

        [Fact]
        public void TaylorSinFunction() {
            evaluateAndAssert(TaylorSin[2], 2);
        }

        [Fact]
        public void TwoPowerX() {
            Symbol x = "x";
            evaluateAndAssert(Fun[x, Power[2][x]][7], 128);
        }


        [Fact]
        public void XMinus2() {
            Symbol x = "x";
            evaluateAndAssert(Fun[x, Plus[x, -2]][3], 1);
        }

        [Fact]
        public void XPlusY() {
            Symbol x = "x";
            Symbol y = "y";

            var f = Fun;

            var func = f[x, f[y, Plus[x, y]]];
            var (steps, result) = func[2][3].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => @out.WriteLine(e.Visit(new MathematicaPrinter())));
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

            steps.WithoutDuplicates().ForEach(e => @out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(25, result);
        }

        [Fact]
        public void XPower2() {
            Symbol x = "x";
            evaluateAndAssert(Fun[x, Power[x][2]][7], 49);
        }

        [Fact]
        public void XReturnsConstant() {
            Symbol x = "x";

            var func = Fun[x, 3];
            var (steps, result) = func[0].Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => @out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(3, result);
        }

        [Fact]
        public void ZPowerXPlusY() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var f = Fun;

            var func = f[x, f[y, f[z, Power[z][Plus[x, y]]]]];
            evaluateAndAssert(func[2][3][2], 32);
        }
    }
}