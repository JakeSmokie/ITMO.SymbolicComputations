using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.BooleanFunctions {
    public class IfImplementation : AbstractFunctionImplementation {
        public IfImplementation() : base(If) {
        }

        protected override Symbol Evaluate(Expression expression) {
            if (expression.Arguments[0].Equals(True)) {
                return expression.Arguments[1];
            }

            if (expression.Arguments[0].Equals(False)) {
                return expression.Arguments[2];
            }

            return expression.Arguments[3];
        }
    }
}