using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class TimesConstantsFunction {
        public static readonly Expression TimesConstants =
            Fun[expr,
                If[Not[IsExpressionWithName[Times][expr]],
                    expr,
                    //
                    Fun["timesArgs'",
                        Fun["constants", Fun["others",
                            If[
                                Eq[Length["constants"], 0],
                                //
                                list,
                                ApplyList[
                                    Times,
                                    If[
                                        Eq[ListTimes["constants"], 0],
                                        List[0],
                                        If[
                                            Eq[ListTimes["constants"], 1],
                                            "others",
                                            Append[
                                                "others",
                                                ListTimes["constants"]
                                            ]   
                                        ]
                                    ]
                                ]]
                        ]][
                            Filter["timesArgs'"][Fun[x, IsConstant[x]]]
                        ][
                            Filter["timesArgs'"][Fun[x, Not[IsConstant[x]]]]
                        ]
                    ][
                        DefaultValue[AsExpressionArgs[Times, expr]][EmptyList]
                    ]
                ]
            ];
    }
}