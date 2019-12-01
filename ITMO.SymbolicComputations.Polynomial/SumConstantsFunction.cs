using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class SumConstantsFunction {
        public static readonly StringSymbol SumConstants = new StringSymbol(nameof(SumConstants));

        public static readonly Expression SumConstantsImplementation =
            Fun[expr,
                If[Not[IsExpressionWithName[Plus][expr]],
                    expr,
                    //
                    Fun["plusArgs'",
                        Fun["constants", Fun["others",
                            Seq[
//                                "constants",
//                                "others",
                                If[
                                    Eq[Length["constants"], 0],
                                    //
                                    list,
                                    ApplyList[
                                        Plus,
                                        If[
                                            Or[
                                                And[
                                                    Eq[ListPlus["constants"], 0]
                                                ][
                                                    Not[Eq[Length["others"], 0]]
                                                ]
                                            ][
                                                Eq[Length["constants"], 0]
                                            ],
                                            //
                                            "others",
                                            Append[
                                                "others",
                                                ListPlus["constants"]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]][
                            Filter["plusArgs'"][Fun[x, IsConstant[x]]]
                        ][
                            Filter["plusArgs'"][Fun[x, Not[IsConstant[x]]]]
                        ]
                    ][DefaultValue[AsExpressionArgs[Plus, expr]][EmptyList]]
                ]
            ];
    }
}