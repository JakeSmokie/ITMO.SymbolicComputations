using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public class Boolean {
        public static readonly StringSymbol True = new StringSymbol(nameof(True));
        public static readonly StringSymbol False = new StringSymbol(nameof(False));

        public static readonly StringSymbol If = new StringSymbol(nameof(If), Attributes.HoldRest);

        public static readonly StringSymbol Eq = new StringSymbol(nameof(Eq));
        public static readonly StringSymbol Compare = new StringSymbol(nameof(Compare));

        public static readonly Expression Not = Fun["local_not", If["local_not", False, True]];

        public static readonly Expression Less = Fun["less_x", Fun["less_y",
            Eq[Compare["less_x", "less_y"], -1]
        ]];

        public static readonly Expression More = Fun["more_x", Fun["more_y",
            Eq[Compare["more_x", "more_y"], 1]
        ]];
    }
}