using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Functions.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class ApplyListImplementation : AbstractFunctionImplementation{
        public ApplyListImplementation() : base(ApplyList) {
        }

        protected override Symbol Evaluate(Expression expression) {
            return expression;
        }
    }
}