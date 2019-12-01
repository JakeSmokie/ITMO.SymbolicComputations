using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class ConstantsSumTests {
        public ConstantsSumTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void ConstantsSummed() {
            Symbol x = "x";
            Symbol y = "y";
            
            evaluateAndAssert(
                SumConstants[
                    Plus[3, x, y, 10, -1]
                ],
                Plus[x, y, 12]
            );
        }

        [Fact]
        public void NotPlusIsOkay() {
            Symbol x = "x";
            Symbol y = "y";
            
            evaluateAndAssert(
                SumConstants[
                    Times[3, x, y, 10, -1]
                ],
                Times[x, y, -1, 3, 10]
            );
        }

        [Fact]
        public void ConstantsLost() {
            Symbol x = "x";
            Symbol y = "y";
            
            evaluateAndAssert(
                SumConstants[
                    Plus[3, x, y, 10, -13]
                ],
                Plus[x, y]
            );
        }
    }
}