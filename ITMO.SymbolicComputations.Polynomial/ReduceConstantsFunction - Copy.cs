using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Polynomial {
    public static class GroupBy {
        public static readonly Expression ReduceConstants =
            Fun[list,
                Fun["constants",
                    If[
                        Eq[Length["constants"], 0],
                        list,
                        Append[
                            Filter[list, Fun[x, Not[IsConstant[x]]]],
                            ListPlus["constants"]
                        ],
                        "Error"
                    ]
                ][Filter[list, Fun[x, IsConstant[x]]]]
            ];
    }
}