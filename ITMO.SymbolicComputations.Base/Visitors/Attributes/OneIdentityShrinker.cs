using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Attributes {
    public sealed class OneIdentityShrinker : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker IsOneIdentityVisitor =
            new HasAttributeChecker(Predefined.Attributes.OneIdentity);

        public Symbol VisitFunction(Expression expression) =>
            expression.Head.Visit(IsOneIdentityVisitor)
            && expression.Arguments.Count == 1
                ? expression.Arguments[0].Visit(this)
                : expression;

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}