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
            Fun[list,
                Fun["constants", Fun["others",
                    If[
                        Eq[Length["constants"], 0],
                        //
                        list,
                        Evaluate[If[
                            Eq[ListTimes["constants"], 0],
                            //
                            0,
                            Evaluate[If[
                                Eq[ListTimes["constants"], 1],
                                "others",
                                Evaluate[Append[
                                    "others",
                                    ListTimes["constants"]
                                ]]
                            ]]
                        ]]
                    ]
                ]][
                    Filter[list][Fun[x, IsConstant[x]]]
                ][
                    Filter[list][Fun[x, Not[IsConstant[x]]]]
                ]
            ];
    }
}