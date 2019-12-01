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
        public void PowerWorks() {
            evaluateAndAssert(
                Power["x"][2],
                Power["x"][2]
            );
        }
    }
}