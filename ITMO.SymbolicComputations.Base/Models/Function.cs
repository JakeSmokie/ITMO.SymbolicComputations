using System.Collections.Immutable;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : ISymbol {
        public Function(ISymbol symbol, ImmutableList<ISymbol> arguments, ImmutableHashSet<ISymbol> attributes = null) {
            Symbol = symbol;
            Arguments = arguments;
            Attributes = attributes ?? ImmutableHashSet<ISymbol>.Empty;
        }

        public ISymbol Symbol { get; }
        public ImmutableList<ISymbol> Arguments { get; }
        
        [JsonIgnore]
        public ImmutableHashSet<ISymbol> Attributes { get; }
    }
}