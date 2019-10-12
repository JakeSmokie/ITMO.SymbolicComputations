using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FullEvaluator : ISymbolVisitor<Symbol> {
        private static readonly OneIdentityShrinker OneIdentityShrinker = new OneIdentityShrinker();
        private static readonly AttributesEvaluator AttributesEvaluator = new AttributesEvaluator();
        private static readonly HoldFormReleaser HoldFormReleaser = new HoldFormReleaser();
        private static readonly FlatFlattener FlatFlattener = new FlatFlattener();

        public Symbol VisitFunction(Expression expression) =>
            expression
                .Visit(AttributesEvaluator)
                .Visit(FlatFlattener)
//                .Visit(Orderless)
                .Visit(OneIdentityShrinker)
                .Visit(HoldFormReleaser);

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}