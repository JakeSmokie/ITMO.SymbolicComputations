﻿using System;
using System.Globalization;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class MathematicaPrintingExtensions {
        public static string AsMathematica(this ExpressionInfo expressionInfo) =>
            expressionInfo.BaseSymbol.AsMathematica();

        private static string AsMathematica(this BaseSymbol symbol) =>
            symbol switch {
                Symbol s => s.Name,
                Constant c => c.Value.ToString(CultureInfo.InvariantCulture),
                Function f => f.Symbol.AsMathematica() +
                              $"[{string.Join(", ", f.Arguments.Select(AsMathematica))}]",
                _ => throw new ArgumentException()
            };
    }
}