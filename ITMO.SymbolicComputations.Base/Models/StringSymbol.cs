using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class StringSymbol : Symbol {
        public StringSymbol(string name) =>
            Name = name;

        public string Name { get; }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitSymbol(this);
    }
}