﻿using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class ArithmeticFunctions {
        public static readonly StringSymbol Plus = new StringSymbol("Plus",
            ImmutableHashSet<Symbol>.Empty
                .Add(Attributes.Flat)
                .Add(Attributes.OneIdentity)
        );
    }
}