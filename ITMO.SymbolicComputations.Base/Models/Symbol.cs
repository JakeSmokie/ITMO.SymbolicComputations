using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Base.Models {
    public abstract class Symbol {
        protected abstract T VisitImplementation<T>(ISymbolVisitor<T> visitor);

        public T Visit<T>(ISymbolVisitor<T> visitor) =>
            VisitImplementation(visitor);
        
        public Function this[params Symbol[] arguments] =>
            new Function(this, arguments.ToImmutableList());
    }
}