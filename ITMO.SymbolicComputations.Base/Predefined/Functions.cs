using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Functions {
        public static readonly StringSymbol Evaluate = new StringSymbol(nameof(Evaluate));

        public static readonly StringSymbol Hold = new StringSymbol(nameof(Hold),
            ImmutableSortedSet<StringSymbol>.Empty.Add(Attributes.HoldAll)
        );

        public static readonly StringSymbol HoldComplete = new StringSymbol(nameof(HoldComplete),
            ImmutableSortedSet<StringSymbol>.Empty.Add(Attributes.HoldAll)
        );

        public static readonly StringSymbol HoldForm = new StringSymbol(nameof(HoldForm),
            ImmutableSortedSet<StringSymbol>.Empty.Add(Attributes.HoldAll)
        );
    }
}