using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public sealed class SinFunctionImplementation : AbstractFunctionImplementation {
        public SinFunctionImplementation() : base(ArithmeticFunctions.Sin) {
        }

        protected override Symbol Evaluate(Expression expression) {
            if (!(expression.Arguments[0] is Constant x)) {
                throw new NotImplementedException();
            }

            return (decimal) Math.Sin((double) x.Value);
        }
    }
}