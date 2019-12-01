using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.TimesConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class ConstantsTimesTests {
        public ConstantsTimesTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output, Seq[
                Set[TimesConstants, TimesConstantsImplementation]
            ]);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void ConstantsMultipliedGeneral() {
            Symbol x = "x";
            Symbol y = "y";

            evaluateAndAssert(
                TimesConstants[
                    Times[3, x, y, 10, -1]
                ],
                Times[x, y, -30]
            );
        }
        
        [Fact]
        public void NotPlusIsOkay() {
            Symbol x = "x";
            Symbol y = "y";

            evaluateAndAssert(
                TimesConstants[
                    Plus[3, x, y, 10, -1]
                ],
                Plus[x, y, -1, 3, 10]
            );
        }
    }
}