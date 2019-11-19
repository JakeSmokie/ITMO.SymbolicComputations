using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class SumSymbolsFunction {
        private static readonly Expression GroupAndSum =
            Fun[list,
                Map[Group[list]][Fun[tuple,
                    If[
                        Eq[Part[tuple, 1], 1],
                        //
                        Part[tuple, 0],
                        Times[Part[tuple, 1], Part[tuple, 0]],
                        //
                        "Error"
                    ]
                ]]
            ];

        public static readonly Expression SumSymbols =
            Fun[list,
                Fun["symbols", Fun["others",
                    If[
                        Eq[Length["symbols"], 0],
                        //
                        list,
                        Evaluate[
                            Concat[GroupAndSum["symbols"]]["others"]
                        ],
                        //
                        "Error"
                    ]
                ]][
                    Filter[list][Fun[x, Not[IsConstant[x]]]]
                ][
                    Filter[list][Fun[x, IsConstant[x]]]
                ]
            ];
    }
}