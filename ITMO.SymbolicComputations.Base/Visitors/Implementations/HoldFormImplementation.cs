using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public sealed class HoldFormImplementation : ISymbolVisitor<Symbol> {
        public Symbol VisitExpression(Expression expression) =>
            Equals(expression.Head, Functions.Functions.HoldForm)
                ? expression.Arguments.First()
                : expression;

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}