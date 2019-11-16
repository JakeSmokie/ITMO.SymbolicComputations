using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class IfImplementation : AbstractFunctionImplementation {
        public IfImplementation() : base(Functions.If) {
        }

        protected override Symbol Evaluate(Expression expression) {
            if (expression.Arguments[0].Equals(Boolean.True)) {
                return expression.Arguments[1];
            }

            if (expression.Arguments[0].Equals(Boolean.False)) {
                return expression.Arguments[2];
            }

            return expression.Arguments[3];
        }
    }
}