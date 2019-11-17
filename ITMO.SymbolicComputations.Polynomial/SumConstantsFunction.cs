using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class SumConstantsFunction {
        public static readonly Expression SumConstants =
            Fun[list,
                Fun["constants", Fun["others", 
                    If[
                        Eq[Length["constants"], 0],
                        
                        list,
                        Evaluate[
                            Append[
                                "others",
                                ListPlus["constants"]
                            ]
                        ],
                        
                        "Error"
                    ]
//                    List["constants", "others"]
                ]][
                    Filter[list][Fun[x, IsConstant[x]]]
                ][
                    Filter[list][Fun[x, Not[IsConstant[x]]]]
                ]
            ];
    }
}