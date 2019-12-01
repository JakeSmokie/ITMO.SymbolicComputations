﻿using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class Functions {
        public static readonly StringSymbol Evaluate = new StringSymbol(nameof(Evaluate));

        public static readonly StringSymbol Hold = new StringSymbol(nameof(Hold),
            Attributes.HoldAll
        );

        public static readonly StringSymbol HoldComplete = new StringSymbol(nameof(HoldComplete),
            Attributes.HoldAllComplete
        );

        public static readonly StringSymbol HoldForm = new StringSymbol(nameof(HoldForm),
            Attributes.HoldAll
        );

        public static readonly StringSymbol Fun = new StringSymbol(nameof(Fun),
            Attributes.HoldAll
        );
        
        public static readonly StringSymbol Seq = new StringSymbol(nameof(Seq), Attributes.Flat);
        public static readonly StringSymbol Set = new StringSymbol(nameof(Set));

        public static readonly StringSymbol ApplyList = new StringSymbol(nameof(ApplyList));
    }
}