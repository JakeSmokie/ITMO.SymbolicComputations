using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class ArithmeticFunctions {
        public static readonly StringSymbol Plus = new StringSymbol(nameof(Plus),
            Attributes.Flat,
            Attributes.OneIdentity,
            Attributes.Orderless
        );

        public static readonly StringSymbol Times = new StringSymbol(nameof(Times),
            Attributes.Flat,
            Attributes.OneIdentity,
            Attributes.Orderless
        );

        public static readonly StringSymbol Divide = new StringSymbol(nameof(Divide));

        public static readonly StringSymbol Sin = new StringSymbol(nameof(Sin));

        public static readonly StringSymbol Minus = new StringSymbol(nameof(Minus));

        public static readonly Expression MinusImplementation =
            Fun[x, Times[x, -1]];

        public static readonly StringSymbol ListPlus = new StringSymbol(nameof(ListPlus));
        public static readonly Expression ListPlusImplementation =
            Fun[list,
                Fold[list, 0, Fun[acc, Fun[x, Plus[acc, x]]]]
            ];

        public static readonly Expression ListTimes =
            Fun[list,
                Fold[list, 1, Fun[acc, Fun[x, Times[acc, x]]]]
            ];

        public static readonly StringSymbol Power = new StringSymbol(nameof(Power));

        public static Expression PowerImplementation =>
            Fun[x, Fun[y, If[
                IsConstant[x],
                ApplyList[
                    Times,
                    Map[GenerateList[y]][Fun["_", x]]
                ],
                Power[x][y]
            ]]];

        public static Expression Factorial =>
            Fun[n,
                ApplyList[
                    Times,
                    Map[GenerateList[n]][Fun[x, Plus[x, 1]]]
                ]
            ];

        public static readonly StringSymbol TaylorSin = new StringSymbol(nameof(TaylorSin)); 
        public static Expression TaylorSinImplementation =>
            Fun[x,
                Map[GenerateList[5]][Fun[n,
                    n
                ]]
            ];
    }
}