using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Visitors;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : Symbol {
        public Function(Symbol symbol, ImmutableList<Symbol> arguments, ImmutableHashSet<Symbol>? attributes = null) {
            Symbol = symbol;
            Arguments = arguments;
            Attributes = attributes ?? ImmutableHashSet<Symbol>.Empty;
        }

        public Symbol Symbol { get; }
        public ImmutableList<Symbol> Arguments { get; }

        [JsonIgnore]
        public ImmutableHashSet<Symbol> Attributes { get; }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitFunction(this);

        public static Function Declare(string name, ImmutableHashSet<Symbol>? attributes = null) {
            attributes ??= ImmutableHashSet<Symbol>.Empty;

            return new Function(
                new StringSymbol(name),
                ImmutableList<Symbol>.Empty,
                attributes
            );
        }
    }
}