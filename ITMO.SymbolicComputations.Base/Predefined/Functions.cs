﻿using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Functions {
        public static readonly StringSymbol Evaluate = new StringSymbol(nameof(Evaluate));
        public static readonly StringSymbol SortArguments = new StringSymbol(nameof(SortArguments));
        public static readonly StringSymbol List = new StringSymbol(nameof(List));

        public static readonly StringSymbol Hold = new StringSymbol(nameof(Hold),
            Attributes.HoldAll
        );

        public static readonly StringSymbol HoldComplete = new StringSymbol(nameof(HoldComplete),
            Attributes.HoldAllComplete
        );

        public static readonly StringSymbol HoldForm = new StringSymbol(nameof(HoldForm),
            Attributes.HoldAll
        );

        public static readonly StringSymbol Function = new StringSymbol(nameof(Function),
            Attributes.HoldAll
        );

        public static readonly StringSymbol If = new StringSymbol(nameof(If),
            Attributes.HoldRest
        );
    }
}