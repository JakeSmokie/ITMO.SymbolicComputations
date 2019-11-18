using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Functions.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.Casting {
    public class AsExpressionArgsImplementation : AbstractFunctionImplementation {
        public AsExpressionArgsImplementation() : base(AsExpressionArgs) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var head = expression.Arguments[0];
            var argument = expression.Arguments[1];
            
            var ex = argument.Visit(AsExpressionVisitor.Instance);

            if (ex == null) {
                return Null;
            }
            
            if (!Equals(ex.Head, head)) {
                return Null;
            }

            return List[ex.Arguments.ToArray()];
        }
    }
}