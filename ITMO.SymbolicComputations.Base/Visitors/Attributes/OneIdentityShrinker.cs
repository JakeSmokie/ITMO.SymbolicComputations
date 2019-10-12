using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Attributes {
    public sealed class OneIdentityShrinker : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker IsOneIdentityVisitor =
            new HasAttributeChecker(Predefined.Attributes.OneIdentity);

        public Symbol VisitFunction(Function function) =>
            function.Symbol.Visit(IsOneIdentityVisitor)
            && function.Arguments.Count == 1
                ? function.Arguments[0].Visit(this)
                : function;

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}