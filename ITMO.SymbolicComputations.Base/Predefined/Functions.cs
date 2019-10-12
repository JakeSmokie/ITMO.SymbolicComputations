using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Functions {
        public static readonly StringSymbol Evaluate = new StringSymbol("Evaluate");

        public static readonly StringSymbol Hold = new StringSymbol("Hold",
            ImmutableHashSet<Symbol>.Empty
                .Add(Attributes.HoldAll)
        );
    }
}