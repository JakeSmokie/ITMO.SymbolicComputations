using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class DivideTests {
        public DivideTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;


        [Fact]
        public void DivideWorks() {
            _evaluateAndAssert(
                Divide[1, 4],
                0.25m
            );
        }
    }
}