using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;
using static ITMO.SymbolicComputations.Polynomial.SumConstantsFunction;
using static ITMO.SymbolicComputations.Polynomial.SymbolsTimesToPower;
using static ITMO.SymbolicComputations.Polynomial.TimesConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class PolynomialTests {
        public PolynomialTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output, Seq[
                Set[SumSymbols, SumSymbolsImplementation],
                Set[SumConstants, SumConstantsImplementation]
            ]);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void AllOkay() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            evaluateAndAssert(
                SumConstants[SumSymbols[
                    Plus[
                        TimesSymbols[TimesConstants[Times[x, y, x, 3, 6]]],
                        3, x, y, 10, x, y, y, x, -1, y, z, 4
                    ]
                ]],
                Plus[Times[x, x, y, 18], Times[x, 3], Times[y, 4], z, 16]
            );
        }
    }
}