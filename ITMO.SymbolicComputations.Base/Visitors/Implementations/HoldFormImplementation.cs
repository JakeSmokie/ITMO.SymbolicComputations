using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public sealed class HoldFormImplementation : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) =>
            Equals(expression.Head, Functions.HoldForm)
                ? expression.Arguments.First()
                : expression;

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}