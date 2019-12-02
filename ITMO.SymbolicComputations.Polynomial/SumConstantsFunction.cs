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

        public static Expression SumConstantsImplementation => Seq[
            Fun[expr,
                If[Not[IsExpressionWithName[Plus][expr]],
                    expr,
                    //
                    Evaluate[
                        Fun["plusArgs'",
                            Fun["constants", Fun["others",
                                Part[List[
                                    Set["sum", ListPlus["constants"]],
                                    If[
                                        Eq[Length["constants"], 0],
                                        //
                                        List[list],
                                        Evaluate[
                                            ApplyList[
                                                Plus,
                                                Append[
                                                    "others",
                                                    "sum"
                                                ]
                                            ]
                                        ]
                                    ]
                                ], 1]
                            ]][
                                Filter["plusArgs'"][Fun[x, IsConstant[x]]]
                            ][
                                Filter["plusArgs'"][Fun[x, Not[IsConstant[x]]]]
                            ]
                        ][DefaultValue[AsExpressionArgs[Plus, expr]][EmptyList]]
                    ]
                ]
            ]
        ];
    }
}