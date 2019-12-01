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

            return (decimal) Math.Pow((double) x.Value, (double) y.Value);
        }
    }
}