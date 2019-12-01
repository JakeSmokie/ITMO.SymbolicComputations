using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public abstract class AbstractFunctionImplementation : ISymbolVisitor<Symbol> {
        protected readonly StringSymbol Name;
        protected AbstractFunctionImplementation(StringSymbol name) => Name = name;

        public Symbol VisitExpression(Expression expression) {
            if (!Equals(expression.Head, Name)) {
                return expression;
            }

            return Evaluate(expression);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
        protected abstract Symbol Evaluate(Expression expression);
    }
}