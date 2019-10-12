using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : Symbol {
        public Function(Symbol symbol, ImmutableList<Symbol> arguments) {
            Symbol = symbol;
            Arguments = arguments;
        }

        public Symbol Symbol { get; }
        public ImmutableList<Symbol> Arguments { get; }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitFunction(this);
    }
}