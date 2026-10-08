using System;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    // Keep factorial's recursive DSL implementation, but reject values for which it is undefined.
    internal sealed class FactorialInputImplementation : AbstractFunctionImplementation {
        public FactorialInputImplementation() : base(FactorialInput) { }

        protected override Symbol Evaluate(Expression expression) {
            if (!(expression.Arguments[0] is Constant value)) {
                return expression;
            }
            if (value.Value < 0 || decimal.Truncate(value.Value) != value.Value) {
                throw new ArgumentOutOfRangeException(nameof(expression), "Factorial requires a non-negative integer argument.");
            }
            if (value.Value > 27) {
                throw new OverflowException("Factorial above 27 exceeds the supported decimal numeric range.");
            }
            return value;
        }
    }
}
