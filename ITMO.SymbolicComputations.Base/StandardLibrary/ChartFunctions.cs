using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class ChartFunctions {
        public static readonly StringSymbol FormInput = new StringSymbol(nameof(FormInput),
            Attributes.HoldFirst
        );

        public static readonly StringSymbol Plot = new StringSymbol(nameof(Plot));

        public static Expression PlotImplementation =>
            Fun[List[f, "range"],
                FastMap["range", Fun[x,
                    KindaList[x, f[x]]
                ]]
            ];
    }
}