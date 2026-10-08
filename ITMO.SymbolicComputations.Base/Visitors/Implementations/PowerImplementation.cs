using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public sealed class PowerImplementation : AbstractFunctionImplementation {
        public PowerImplementation() : base(Power) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var x = expression.Arguments[0].Visit(AsConstantVisitor.Instance);
            var y = expression.Arguments[1].Visit(AsConstantVisitor.Instance);

            if (x == null || y == null) {
                return expression;
            }

            if (x.Value == 0 && y.Value < 0) {
                throw new DivideByZeroException("Zero cannot be raised to a negative power.");
            }
            if (x.Value < 0 && decimal.Truncate(y.Value) != y.Value) {
                throw new ArgumentOutOfRangeException(nameof(expression), "A negative base with a fractional exponent is outside the supported real-number domain.");
            }

            var result = Math.Pow((double) x.Value, (double) y.Value);
            if (double.IsNaN(result) || double.IsInfinity(result)) {
                throw new ArithmeticException("Power is outside the supported finite real-number range.");
            }
            return (decimal) result;
        }
    }
}
