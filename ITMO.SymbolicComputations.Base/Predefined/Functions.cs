using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Functions {
        public static readonly StringSymbol Evaluate = new StringSymbol("Evaluate");

        public static readonly StringSymbol Hold = new StringSymbol("Hold",
            ImmutableSortedSet<StringSymbol>.Empty.Add(Attributes.HoldAll)
        );
        
        public static readonly StringSymbol HoldForm = new StringSymbol("HoldForm",
            ImmutableSortedSet<StringSymbol>.Empty.Add(Attributes.HoldAll)
        );
    }
}