using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Visitors;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class StringSymbol : Symbol {
        public StringSymbol(string name, ImmutableHashSet<Symbol>? attributes = null) {
            Name = name;
            Attributes = attributes ?? ImmutableHashSet<Symbol>.Empty;
        }

        public string Name { get; }

        [JsonIgnore]
        public ImmutableHashSet<Symbol> Attributes { get; }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitSymbol(this);
    }
}