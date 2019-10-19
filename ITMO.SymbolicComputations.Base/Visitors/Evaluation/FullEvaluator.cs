using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using ITMO.SymbolicComputations.Base.Visitors.Implementations;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FullEvaluator : ISymbolVisitor<Symbol> {
        private static readonly OneIdentityShrinker OneIdentityShrinker = new OneIdentityShrinker();
        private static readonly AttributesEvaluator AttributesEvaluator = new AttributesEvaluator();
        private static readonly HoldFormImplementation HoldFormImplementation = new HoldFormImplementation();
        private static readonly FlatFlattener FlatFlattener = new FlatFlattener();
        private static readonly ArgumentsSorter ArgumentsSorter = new ArgumentsSorter();

        public Symbol VisitFunction(Expression expression) {
            var visitors = new ISymbolVisitor<Symbol>[] {
                AttributesEvaluator,
                FlatFlattener,
                ArgumentsSorter,
                OneIdentityShrinker,
                HoldFormImplementation
            };

            return visitors.Fold((Symbol) expression, (symbol, visitor) => symbol.Visit(visitor));
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}