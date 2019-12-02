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

        public static readonly StringSymbol BinaryPlus = new StringSymbol(nameof(BinaryPlus));
        public static readonly StringSymbol BinaryTimes = new StringSymbol(nameof(BinaryTimes));

        public static readonly StringSymbol Times = new StringSymbol(nameof(Times),
            Attributes.Flat,
            Attributes.OneIdentity,
            Attributes.Orderless
        );

        public static readonly StringSymbol Divide = new StringSymbol(nameof(Divide));
        public static readonly StringSymbol Sin = new StringSymbol(nameof(Sin));

        public static readonly StringSymbol Minus = new StringSymbol(nameof(Minus));
        public static readonly StringSymbol ListPlus = new StringSymbol(nameof(ListPlus));
        public static readonly StringSymbol ListTimes = new StringSymbol(nameof(ListTimes));
        public static readonly StringSymbol Power = new StringSymbol(nameof(Power));
        public static readonly StringSymbol TaylorSin = new StringSymbol(nameof(TaylorSin));
        public static readonly StringSymbol Factorial = new StringSymbol(nameof(Factorial));

        public static Expression MinusImplementation =>
            Fun[x, Times[x, -1]];

        public static Expression Abs => Fun[x, If[Less[x][0], Minus[x], x]];

        public static Expression ListPlusImplementation =>
            Fun[list,
                Fold[list][0][Fun[acc, Fun[x, BinaryPlus[acc, x]]]]
            ];

        public static Expression ListTimesImplementation =>
            Fun[list,
                Fold[list][1][Fun[acc, Fun[x, BinaryTimes[acc, x]]]]
            ];

        public static Expression FactorialImplementation =>
            Fun[x,
                If[More[x][1], Times[x, Factorial[Plus[x, -1]]], 1]
            ];

        public static Expression TaylorSinImplementation =>
            Fun[x, Seq[
                ApplyList[
                    Plus,
                    FastMap[GenerateList[10], Fun[n,
                        Times[
                            Power[-1, n],
                            Divide[
                                Power[x, Plus[Times[2, n], 1]],
                                Factorial[Plus[Times[2, n], 1]]
                            ]
                        ]
                    ]]
                ]
            ]];
    }
}