﻿using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class SymbolsTimesToPower {
        public static readonly StringSymbol TimesSymbols = new StringSymbol(nameof(TimesSymbols));

        private static Expression GroupAndMultiply =>
            Fun[list,
                Map[Group[list]][Fun[tuple,
                    If[
                        Eq[Part[tuple, 1], 1],
                        //
                        Part[tuple, 0],
                        Power[Part[tuple, 0], Part[tuple, 1]]
                    ]
                ]]
            ];

        public static Expression TimesSymbolsImplementation =>
            Fun[expr,
                If[Not[IsExpressionWithName[Times][expr]],
                    expr,
                    Fun["timesArgs'",
                        Fun["symbols", Fun["others",
                            If[
                                Eq[Length["symbols"], 0],
                                //
                                expr,
                                Seq[
                                    ApplyList[
                                        Times,
                                        Concat[GroupAndMultiply["symbols"]]["others"]
                                    ]
                                ]
                            ]
                        ]][
                            Filter["timesArgs'"][Fun[x, Not[IsConstant[x]]]]
                        ][
                            Filter["timesArgs'"][Fun[x, IsConstant[x]]]
                        ]
                    ][DefaultValue[AsExpressionArgs[Times, expr]][EmptyList]]
                ]
            ];
    }
}