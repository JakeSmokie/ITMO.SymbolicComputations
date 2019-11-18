using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class ApplyListImplementation : AbstractFunctionImplementation{
        public ApplyListImplementation() : base(ApplyList) {
        }

        protected override Symbol Evaluate(Expression expression) {
            return expression;
        }
    }
}