using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FullEvaluator : ISymbolVisitor<Symbol> {
        private static readonly OneIdentityShrinker OneIdentityShrinker =
            new OneIdentityShrinker();

        private static readonly AttributesEvaluator AttributesEvaluator =
            new AttributesEvaluator();

        public Symbol VisitFunction(Expression expression) {
            return expression
                .Visit(AttributesEvaluator)
                .Visit(OneIdentityShrinker);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}