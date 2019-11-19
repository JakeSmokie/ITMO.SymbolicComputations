using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class DivideImplementation : AbstractFunctionImplementation{
        public DivideImplementation() : base(Divide) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var first = expression.Arguments[0].Visit(AsConstantVisitor.Instance);
            var second = expression.Arguments[1].Visit(AsConstantVisitor.Instance);

            if (first == null || second == null) {
                return expression;
            }

            return first.Value / second.Value;
        }
    }
}