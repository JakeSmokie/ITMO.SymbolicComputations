using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.BooleanFunctions {
    public class EqImplementation : AbstractFunctionImplementation {
        public EqImplementation() : base(Eq) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var x = expression.Arguments[0];
            var y = expression.Arguments[1];
            
            if (x.GetType() != y.GetType() && !Equals(x, Null) && !Equals(y, Null)) {
                return expression;
            }

            return Equals(x, y) ? True : False;
        }
    }
}