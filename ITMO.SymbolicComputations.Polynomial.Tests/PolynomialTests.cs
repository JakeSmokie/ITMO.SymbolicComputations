using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;
using static ITMO.SymbolicComputations.Polynomial.SumConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class PolynomialTests {
        public PolynomialTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.EvaluateAndAssert(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void AllOkay() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";
            
            _evaluateAndAssert(
                SumConstants[SumSymbols[
                    List[3, x, y, 10, x, y, y, x, -1, y, z, 4]
                ]],
                List[BinaryTimes[3, x], BinaryTimes[4, y], z, 16]
            );
        }
    }
}