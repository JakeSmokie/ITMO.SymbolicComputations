using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Attributes {
    public sealed class HasAttributeChecker : ISymbolVisitor<bool> {
        public HasAttributeChecker(StringSymbol attribute) =>
            Attribute = attribute;

        public StringSymbol Attribute { get; }

        public bool VisitSymbol(StringSymbol symbol) =>
            symbol.Attributes.Contains(Attribute);

        public bool VisitFunction(Expression expression) => false;
        public bool VisitConstant(Constant constant) => false;
    }
}