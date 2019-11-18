using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Functions.Alphabet;
using static ITMO.SymbolicComputations.Base.Functions.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Functions.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Functions.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Functions.Functions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;

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
                            AppendList[GroupAndSum["symbols"]]["others"]
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