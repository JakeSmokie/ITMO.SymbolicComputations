using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class PowerTests {
        public PowerTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void NotPowerIsNotProcessed() {
            evaluateAndAssert(
                PowerImplementation[Plus["x", "y"]],
                Plus["x", "y"]
            );

            evaluateAndAssert(
                PowerImplementation[Times["x", "y"]],
                Times["x", "y"]
            );
        }

        [Fact]
        public void PowerWithSymbolsIsNotProcessed() {
            evaluateAndAssert(
                PowerImplementation[Power["x", "y"]],
                Power["x", "y"]
            );

            evaluateAndAssert(
                PowerImplementation[Power["x", 2]],
                Power["x", 2]
            );
        }

        [Fact]
        public void ConstantsPower() {
            evaluateAndAssert(
                PowerImplementation[Power[2, 8]],
                256
            );
        }
    }
}