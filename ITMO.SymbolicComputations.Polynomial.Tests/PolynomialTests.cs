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
using static ITMO.SymbolicComputations.Polynomial.SymbolsTimesToPower;
using static ITMO.SymbolicComputations.Polynomial.TimesConstantsFunction;

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
                    Plus[
                        TimesSymbols[TimesConstants[Times[x, y, x, 3, 6]]],
                        3, x, y, 10, x, y, y, x, -1, y, z, 4
                    ]
                ]],
                Plus[Times[Power[x, 2], y, 18], Times[x, 3], Times[y, 4], z, 16]
            );
        }
    }
}