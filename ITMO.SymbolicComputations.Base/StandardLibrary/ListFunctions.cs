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

        public static readonly StringSymbol Map = new StringSymbol(nameof(Map));
        public static readonly StringSymbol Filter = new StringSymbol(nameof(Filter));
        public static readonly StringSymbol Length = new StringSymbol(nameof(Length));
        public static readonly StringSymbol Concat = new StringSymbol(nameof(Concat));
        public static readonly StringSymbol CountItem = new StringSymbol(nameof(CountItem));
        public static readonly StringSymbol Contains = new StringSymbol(nameof(Contains));
        public static readonly StringSymbol Distinct = new StringSymbol(nameof(Distinct));
        public static readonly StringSymbol Group = new StringSymbol(nameof(Group));

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

        public static Expression MapImplementation =>
            Fun[list, Fun[f,
                Fold[list, EmptyList, Fun[acc, Fun[x,
                    Append[acc, f[x]]
                ]]]
            ]];

        public static Expression FilterImplementation =>
            Fun[list, Fun[f,
                Fold[list, EmptyList, Fun[acc, Fun[x,
                    If[f[x],
                        Append[acc, x],
                        acc
                    ]
                ]]]
            ]];

        public static Expression LengthImplementation =>
            Fun[list,
                Fold[list, 0, Fun[acc, Fun[x, Plus[acc, 1]]]]
            ];

        public static Expression ConcatImplementation =>
            Fun[list, Fun[list2,
                Fold[list2, list, Fun[acc, Fun[x,
                    Append[acc, x]
                ]]]
            ]];

        public static Expression CountItemImplementation =>
            Fun[list, Fun[x,
                Length[
                    Filter[list][Fun[y, Eq[x, y]]]
                ]
            ]];

        public static Expression ContainsImplementation =>
            Fun[list, Fun[x,
                Not[Eq[
                    CountItem[list][x],
                    0
                ]]
            ]];

        public static Expression DistinctImplementation =>
            Fun[list,
                Fold[list, EmptyList, Fun[acc, Fun[x,
                    If[
                        Contains[acc][x],
                        acc,
                        Append[acc, x]
                    ]
                ]]]
            ];

        public static Expression GroupImplementation =>
            Fun[list,
                Map[Distinct[list]][Fun[x,
                    List[x, CountItem[list][x]]]
                ]
            ];
    }
}