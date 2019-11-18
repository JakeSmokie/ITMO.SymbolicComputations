using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Functions.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Functions.Functions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;
using static ITMO.SymbolicComputations.Polynomial.SumConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class PolynomialTests {
        public PolynomialTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
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
                List[Times[x, 3], Times[y, 4], z, 16]
            );
        }
    }
}