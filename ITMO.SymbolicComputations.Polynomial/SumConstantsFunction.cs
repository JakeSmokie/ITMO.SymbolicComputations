using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class SumConstantsFunction {
        public static readonly Expression SumConstants =
            Fun[expr,
                If[Not[IsExpressionWithName[Plus][expr]],
                    expr,
                    //
                    Evaluate[
                        Fun["plusArgs'",
                            Fun["constants", Fun["others",
                                    If[
                                        Eq[Length["constants"], 0],
                                        //
                                        list,
                                        Evaluate[
                                            ApplyList[
                                                Plus,
                                                If[
                                                    And[
                                                        Eq[ListPlus["constants"], 0]
                                                    ][
                                                        Not[Eq[Length["others"], 0]]
                                                    ],
                                                    //
                                                    "others",
                                                    Evaluate[
                                                        Append[
                                                            "others",
                                                            ListPlus["constants"]
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ][
                                Filter["plusArgs'"][Fun[x, IsConstant[x]]]
                            ][
                                Filter["plusArgs'"][Fun[x, Not[IsConstant[x]]]]
                            ]
                        ][AsExpressionArgs[Plus, expr]]
                    ]
                ]
            ];
    }
}