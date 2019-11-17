using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class ListFunctions {
        public static readonly StringSymbol List = new StringSymbol(nameof(List));
        public static readonly StringSymbol Part = new StringSymbol(nameof(Part));
        public static readonly StringSymbol Fold = new StringSymbol(nameof(Fold));
        public static readonly StringSymbol Append = new StringSymbol(nameof(Append));

        public static readonly Expression EmptyList = List[new Symbol[0]];

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
                        acc,
                        "Error"
                    ]
                ]]]
            ]];

        public static readonly Expression Length =
            Fun[list,
                Fold[list, 0, Fun[acc, Fun[x, BinaryPlus[acc, 1]]]]
            ];

        public static readonly Expression AppendList =
            Fun[list, Fun[list2,
                Fold[list2, list, Fun[acc, Fun[x,
                    Append[acc, x]
                ]]]
            ]];
    }
}