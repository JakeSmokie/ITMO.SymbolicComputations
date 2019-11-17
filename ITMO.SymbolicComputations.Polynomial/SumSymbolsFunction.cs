using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class SumSymbolsFunction {
        private static readonly Expression GroupAndSum =
            Fun[list,
                Map[Group[list]][Fun[tuple,
                    If[
                        Eq[Part[tuple, 1], 1],
                        //
                        Part[tuple, 0],
                        BinaryTimes[Part[tuple, 1], Part[tuple, 0]],
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
                    Filter[list][Fun[x, IsStringSymbol[x]]]
                ][
                    Filter[list][Fun[x, Not[IsStringSymbol[x]]]]
                ]
            ];
    }
}