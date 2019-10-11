using System.Collections.Immutable;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : BaseSymbol {
        public Function(BaseSymbol symbol, ImmutableList<BaseSymbol> arguments, ImmutableHashSet<BaseSymbol> attributes = null) {
            Symbol = symbol;
            Arguments = arguments;
            Attributes = attributes ?? ImmutableHashSet<BaseSymbol>.Empty;
        }

        public BaseSymbol Symbol { get; }
        public ImmutableList<BaseSymbol> Arguments { get; }
        
        [JsonIgnore]
        public ImmutableHashSet<BaseSymbol> Attributes { get; }
    }
}