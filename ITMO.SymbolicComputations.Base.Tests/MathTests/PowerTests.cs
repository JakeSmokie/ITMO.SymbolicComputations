using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests {
    public class PowerTests {
        public PowerTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void PowerWorks() {
            evaluateAndAssert(Power[2, 1], 2);
            evaluateAndAssert(Power[2, 0], 1);
            evaluateAndAssert(Power[2, 2], 4);
        }

//        [Fact]
//        public void XPowerOneEqualsX() {
//            var source = Power[Plus["x", "y"], 1];
//            var (steps, symbol) = source.Visit(FullEvaluator.Default);
//
//            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
//            Assert.Equal(Plus["x", "y"], symbol);
//        }
//
//        [Fact]
//        public void XPowerZeroEqualsOne() {
//            var source = Power[Plus["x", "y"], 0];
//            var (steps, symbol) = source.Visit(FullEvaluator.Default);
//
//            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
//            Assert.Equal(1, symbol);
//        }
    }
}