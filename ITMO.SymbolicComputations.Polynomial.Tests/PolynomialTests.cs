using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;
using static ITMO.SymbolicComputations.Polynomial.SumConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class PolynomialTests {
        public PolynomialTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output, new FullEvaluator());
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void AllOkay() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            _evaluateAndAssert(
                SumConstants[SumSymbols[
                    Plus[3, x, y, 10, x, y, y, x, -1, y, z, 4]
                ]],
                Plus[Times[x, 3], Times[y, 4], z, 16]
            );
        }
    }
}