using System.Collections.Immutable;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : IBaseSymbol {
        public Function(IBaseSymbol symbol, ImmutableList<IBaseSymbol> arguments, ImmutableHashSet<IBaseSymbol> attributes = null) {
            Symbol = symbol;
            Arguments = arguments;
            Attributes = attributes ?? ImmutableHashSet<IBaseSymbol>.Empty;
        }

        public IBaseSymbol Symbol { get; }
        public ImmutableList<IBaseSymbol> Arguments { get; }
        
        [JsonIgnore]
        public ImmutableHashSet<IBaseSymbol> Attributes { get; }
    }
}