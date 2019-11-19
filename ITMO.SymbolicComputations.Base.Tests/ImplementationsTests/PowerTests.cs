using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class PowerTests {
        public PowerTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void NotPowerIsNotProcessed() {
            _evaluateAndAssert(
                PowerImplementation[Plus["x", "y"]],
                Plus["x", "y"]
            );

            _evaluateAndAssert(
                PowerImplementation[Times["x", "y"]],
                Times["x", "y"]
            );
        }

        [Fact]
        public void PowerWithSymbolsIsNotProcessed() {
            _evaluateAndAssert(
                PowerImplementation[Power["x", "y"]],
                Power["x", "y"]
            );

            _evaluateAndAssert(
                PowerImplementation[Power["x", 2]],
                Power["x", 2]
            );
        }

        [Fact]
        public void ConstantsPower() {
            _evaluateAndAssert(
                PowerImplementation[Power[2, 8]],
                256
            );
        }
    }
}