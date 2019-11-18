using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Functions.BooleanFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.BooleanFunctions {
    public class EqImplementation : AbstractFunctionImplementation {
        public EqImplementation() : base(Eq) {
        }

        protected override Symbol Evaluate(Expression expression) {
            return Equals(expression.Arguments[0], expression.Arguments[1])
                ? True
                : False;
        }
    }
}