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
            Fun[expr,
                If[Not[IsExpressionWithName[Plus][expr]],
                    expr,
                    Evaluate[
                        Fun["plusArgs'",
                            Fun["symbols", Fun["others",
                                If[
                                    Eq[Length["symbols"], 0],
                                    //
                                    expr,
                                    Evaluate[
                                        ApplyList[
                                            Plus,
                                            Concat[GroupAndSum["symbols"]]["others"]
                                        ]
                                    ]
                                ]
                            ]][
                                Filter["plusArgs'"][Fun[x, Not[IsConstant[x]]]]
                            ][
                                Filter["plusArgs'"][Fun[x, IsConstant[x]]]
                            ]
                        ][DefaultValue[AsExpressionArgs[Plus, expr]][EmptyList]]
                    ]
                ]
            ];
    }
}