using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public abstract class AbstractFunctionImplementation : ISymbolVisitor<Symbol> {
        private readonly StringSymbol[] names;
        protected AbstractFunctionImplementation(params StringSymbol[] names) {
            this.names = names;
        }

        public Symbol VisitExpression(Expression expression) {
            return names.Any(x => Equals(expression.Head, x)) 
                ? Evaluate(expression) 
                : expression;
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
        protected abstract Symbol Evaluate(Expression expression);
    }
}