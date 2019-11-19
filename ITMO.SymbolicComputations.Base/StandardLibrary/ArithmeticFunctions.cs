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

        public static readonly StringSymbol Power = new StringSymbol(nameof(Power));

        public static readonly StringSymbol Sin = new StringSymbol(nameof(Sin));

        public static readonly Expression ListPlus =
            Fun[list,
                Fold[list, 0, Fun[acc, Fun[x, Plus[acc, x]]]]
            ];

        public static readonly Expression ListTimes =
            Fun[list,
                Fold[list, 1, Fun[acc, Fun[x, Times[acc, x]]]]
            ];

        public static Expression PowerImplementation =>
            Fun[expr,
                If[Not[IsExpressionWithName[Power][expr]],
                    expr,
                    Evaluate[
                        Fun["powerArgs'",
                            Fun["others", Fun["constants",
                                If[More[Length["others"]][0],
                                    expr,
                                    Evaluate[""]
                                ]
                            ]][
                                Filter["powerArgs'"][Fun[x, Not[IsConstant[x]]]]
                            ]
                            [
                                Filter["powerArgs'"][Fun[x, IsConstant[x]]]
                            ]
                        ][DefaultValue[AsExpressionArgs[Power, expr]][EmptyList]]
                    ]
                ]
            ];
    }
}