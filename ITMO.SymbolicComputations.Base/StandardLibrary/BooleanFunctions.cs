using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class BooleanFunctions {
        public static readonly StringSymbol True = new StringSymbol(nameof(True));
        public static readonly StringSymbol False = new StringSymbol(nameof(False));

        public static readonly StringSymbol If = new StringSymbol(nameof(If), Attributes.HoldRest);

        public static readonly StringSymbol Eq = new StringSymbol(nameof(Eq));
        public static readonly StringSymbol Compare = new StringSymbol(nameof(Compare));

        public static readonly StringSymbol Not = new StringSymbol(nameof(Not));
        public static readonly StringSymbol Less = new StringSymbol(nameof(Less));
        public static readonly StringSymbol More = new StringSymbol(nameof(More));
        public static readonly StringSymbol And = new StringSymbol(nameof(And));
        public static readonly StringSymbol Or = new StringSymbol(nameof(Or));

        public static Expression NotImplementation => Fun[x, If[x, False, True]];

        public static Expression LessImplementation =>
            Fun[x, Fun[y,
                Eq[Compare[x, y], -1]
            ]];

        public static Expression MoreImplementation =>
            Fun[x, Fun[y,
                Eq[Compare[x, y], 1]
            ]];

        public static Expression AndImplementation =>
            Fun[x, Fun[y,
                If[x,
                    y,
                    False,
                    "Error"
                ]
            ]];

        public static Expression OrImplementation =>
            Fun[x, Fun[y,
                If[x,
                    True,
                    y,
                    "Error"
                ]
            ]];
    }
}