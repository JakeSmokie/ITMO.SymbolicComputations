using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public abstract class AbstractFunctionImplementation : ISymbolVisitor<Symbol> {
        private readonly StringSymbol _name;
        protected AbstractFunctionImplementation(StringSymbol name) => _name = name;

        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, _name)) {
                return expression;
            }

            return Evaluate(expression);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
        protected abstract Symbol Evaluate(Expression expression);
    }
}