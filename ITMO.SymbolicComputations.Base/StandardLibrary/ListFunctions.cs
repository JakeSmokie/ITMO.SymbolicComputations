using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class ListFunctions {
        public static readonly StringSymbol List = new StringSymbol(nameof(List));
        public static readonly StringSymbol GenerateList = new StringSymbol(nameof(GenerateList));
        
        public static readonly StringSymbol Part = new StringSymbol(nameof(Part));
        public static readonly StringSymbol Fold = new StringSymbol(nameof(Fold));
        public static readonly StringSymbol Append = new StringSymbol(nameof(Append));

        public static readonly Expression EmptyList = List[new Symbol[0]];

        public static Expression Range {
            get {
                var a = new StringSymbol("a'");
                var b = new StringSymbol("b'");
                var stepsCount = new StringSymbol("stepsCount'");
                var stepInterval = Divide[Plus[b, Minus[a]], stepsCount];
                
                return Fun[a, Fun[b, Fun[stepsCount,
                    Map[GenerateList[stepsCount]][
                        Fun[x, Plus[a, Times[stepInterval, x]]]
                    ]
                ]]];
            }
        }

        public static readonly Expression Map =
            Fun[list, Fun[f,
                Fold[list, EmptyList, Fun[acc, Fun[x,
                    Append[acc, f[x]]
                ]]]
            ]];

        public static readonly Expression Filter =
            Fun[list, Fun[f,
                Fold[list, EmptyList, Fun[acc, Fun[x,
                    If[f[x],
                        Append[acc, x],
                        acc
                    ]
                ]]]
            ]];

        public static readonly Expression Length =
            Fun[list,
                Fold[list, 0, Fun[acc, Fun[x, Plus[acc, 1]]]]
            ];

        public static readonly Expression Concat =
            Fun[list, Fun[list2,
                Fold[list2, list, Fun[acc, Fun[x,
                    Append[acc, x]
                ]]]
            ]];

        public static readonly Expression CountItem =
            Fun[list, Fun[x,
                Length[
                    Filter[list][Fun[y, Eq[x, y]]]
                ]
            ]];

        public static readonly Expression Contains =
            Fun[list, Fun[x,
                Not[Eq[
                    CountItem[list][x],
                    0
                ]]
            ]];

        public static readonly Expression Distinct =
            Fun[list,
                Fold[list, EmptyList, Fun[acc, Fun[x,
                    If[
                        Contains[acc][x],
                        acc,
                        Append[acc, x]
                    ]
                ]]]
            ];

        public static readonly Expression Group =
            Fun[list,
                Map[Distinct[list]][Fun[x,
                    List[x, CountItem[list][x]]]
                ]
            ];
    }
}