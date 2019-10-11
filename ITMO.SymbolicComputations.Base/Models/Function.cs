using System.Collections.Immutable;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : BaseSymbol {
        public Function(BaseSymbol symbol, ImmutableList<BaseSymbol> arguments) {
            Symbol = symbol;
            Arguments = arguments;
        }

        public BaseSymbol Symbol { get; }
        public ImmutableList<BaseSymbol> Arguments { get; }
    }
}