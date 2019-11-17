using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Predefined.CastingFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.Casting {
    public class AsConstantImplementation : AbstractFunctionImplementation {
        public AsConstantImplementation() : base(AsConstant) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var argument = expression.Arguments[0];
            Symbol constant = argument.Visit(AsConstantVisitor.Instance);
            
            return constant ?? Null;
        }
    }
}