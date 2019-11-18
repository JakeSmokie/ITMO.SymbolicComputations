using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class ConstantsSumTests {
        public ConstantsSumTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void ConstantsSummed() {
            Symbol x = "x";
            Symbol y = "y";
            
            _evaluateAndAssert(
                SumConstants[
                    List[3, x, y, 10, -1]
                ],
                List[x, y, 12]
            );
        }
        
        [Fact]
        public void ConstantsLost() {
            Symbol x = "x";
            Symbol y = "y";
            
            _evaluateAndAssert(
                SumConstants[
                    List[3, x, y, 10, -13]
                ],
                List[x, y]
            );
        }
    }
}