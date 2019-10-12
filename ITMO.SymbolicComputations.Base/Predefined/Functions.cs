using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Functions {
        public static Function Evaluate = Function.Declare("Evaluate");

        public static Function Hold = Function.Declare("Hold",
            ImmutableHashSet<Symbol>.Empty
                .Add(Attributes.HoldAll)
        );
    }
}