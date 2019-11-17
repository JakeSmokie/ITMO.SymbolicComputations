using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Boolean {
        public static readonly StringSymbol True = new StringSymbol(nameof(True));
        public static readonly StringSymbol False = new StringSymbol(nameof(False));

        public static readonly StringSymbol If = new StringSymbol(nameof(If), Attributes.HoldRest);

        public static readonly StringSymbol Eq = new StringSymbol(nameof(Eq));
        public static readonly StringSymbol Compare = new StringSymbol(nameof(Compare));

        public static readonly Expression Not = Fun[x, If[x, False, True]];

        public static readonly Expression Less = Fun[x, Fun[y,
            Eq[Compare[x, y], -1]
        ]];

        public static readonly Expression More = Fun[x, Fun[x,
            Eq[Compare[x, y], 1]
        ]];
    }
}