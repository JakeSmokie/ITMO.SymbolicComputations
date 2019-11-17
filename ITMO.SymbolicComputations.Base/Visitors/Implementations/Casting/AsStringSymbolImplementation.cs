using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Predefined.CastingFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.Casting {
    public class AsStringSymbolImplementation : AbstractFunctionImplementation {
        public AsStringSymbolImplementation() : base(AsStringSymbol) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var argument = expression.Arguments[0];
            Symbol stringSymbol = argument.Visit(AsStringSymbolVisitor.Instance);
            
            return stringSymbol ?? Null;
        }
    }
}