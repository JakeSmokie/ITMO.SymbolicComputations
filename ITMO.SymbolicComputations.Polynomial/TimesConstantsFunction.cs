using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Functions.Alphabet;
using static ITMO.SymbolicComputations.Base.Functions.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Functions.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Functions.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Functions.Functions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;

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