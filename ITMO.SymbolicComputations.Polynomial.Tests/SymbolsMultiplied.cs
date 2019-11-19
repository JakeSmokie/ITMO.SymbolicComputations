﻿using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;
using static ITMO.SymbolicComputations.Polynomial.SymbolsTimesToPower;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class SymbolsMultiplied {
        public SymbolsMultiplied(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void SymbolsSummed() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            _evaluateAndAssert(
                TimesSymbols[
                    Times[3, x, y, 10, x, y, y, x, -1, z, y]
                ],
                Plus[Power[x, 3], Power[y, 4], z, -1, 3, 10]
            );
        }
    }
}