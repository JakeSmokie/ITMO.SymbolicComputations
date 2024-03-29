using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class ListFunctions {
        public static readonly StringSymbol List = new StringSymbol(nameof(List));
        public static readonly StringSymbol KindaList = new StringSymbol("");
        public static readonly StringSymbol GenerateList = new StringSymbol(nameof(GenerateList));

        public static readonly StringSymbol Part = new StringSymbol(nameof(Part));
        public static readonly StringSymbol Fold = new StringSymbol(nameof(Fold));
        public static readonly StringSymbol Append = new StringSymbol(nameof(Append));

        public static readonly Expression EmptyList = List[new Symbol[0]];

        public static readonly StringSymbol Map = new StringSymbol(nameof(Map));
        public static readonly StringSymbol FastMap = new StringSymbol(nameof(FastMap));
        public static readonly StringSymbol Filter = new StringSymbol(nameof(Filter));
        public static readonly StringSymbol Length = new StringSymbol(nameof(Length));
        public static readonly StringSymbol Concat = new StringSymbol(nameof(Concat));
        public static readonly StringSymbol CountItem = new StringSymbol(nameof(CountItem));
        public static readonly StringSymbol Contains = new StringSymbol(nameof(Contains));
        public static readonly StringSymbol Distinct = new StringSymbol(nameof(Distinct));
        public static readonly StringSymbol Group = new StringSymbol(nameof(Group));

        public static readonly StringSymbol Range = new StringSymbol(nameof(Range));

        public static Expression MapImplementation =>
            Fun[list, Fun[f,
                Fold[list][EmptyList][Fun[acc, Fun[x,
                    Append[acc, f[x]]
                ]]]
            ]];
        
        public static Expression FilterImplementation =>
            Fun[list, Fun[f,
                Fold[list][EmptyList][Fun[acc, Fun[x,
                    If[f[x],
                        Append[acc, x],
                        acc
                    ]
                ]]]
            ]];

        public static Expression ConcatImplementation =>
            Fun[list, Fun[list2,
                Fold[list2][list][Fun[acc, Fun[x,
                    Append[acc, x]
                ]]]
            ]];

        public static Expression CountItemImplementation =>
            Fun[list, Fun[x,
                Fold[list][0][Fun[acc, Fun[y,
                    If[Eq[x, y], Plus[acc, 1], acc]
                ]]]
            ]];

        public static Expression ContainsImplementation =>
            Fun[list, Fun[x,
                Fold[list][False][Fun[acc, Fun[y,
                    If[acc, acc, Eq[x, y]]
                ]]]
            ]];
        
        public static Expression FoldImplementation =>
            Fun[list, Fun["initialState'", Fun[f,
                Fun[n,
                    Part[
                        While[
                            List[0, "initialState'"]
                        ][Fun[x,
                            Less[Part[x, 0]][n]
                        ]][Fun[x,
                            List[
                                Plus[Part[x, 0], 1],
                                f[Part[x, 1]][
                                    Part[list, Part[x, 0]]
                                ]
                            ]
                        ]],
                        1
                    ]
                ][Length[list]]
            ]]];
    }
}