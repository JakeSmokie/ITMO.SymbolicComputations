using System;
using System.Collections.Generic;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Comparers {
    public sealed class SymbolComparer : IComparer<Symbol> {
        public int Compare(Symbol x, Symbol y) =>
            x switch {
                Constant c when y is Constant c2 => CompareInternal(c, c2),
                Constant _ when y is Expression => 1,
                Constant _ when y is StringSymbol => 1,

                Expression e when y is Expression e2 => CompareInternal(e, e2),
                Expression _ when y is Constant => -1,
                Expression _ when y is StringSymbol => -1,

                StringSymbol s when y is StringSymbol s2 => CompareInternal(s, s2),
                StringSymbol _ when y is Constant => -1,
                StringSymbol _ when y is Expression => 1,

                _ => throw new ArgumentException("Unsupported symbols to compare")
            };

        private static int CompareInternal(StringSymbol first, StringSymbol secondStringSymbol) =>
            string.Compare(first.Name, secondStringSymbol.Name, StringComparison.InvariantCulture);

        private static int CompareInternal(Expression firstExpression, Expression secondExpression) {
            return 0;
        }

        private static int CompareInternal(Constant firstConstant, Constant secondConstant) =>
            firstConstant.Value.CompareTo(secondConstant.Value);
    }
}