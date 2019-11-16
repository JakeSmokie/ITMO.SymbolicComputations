using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Boolean;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class ListFunctions {
        public static readonly StringSymbol List = new StringSymbol(nameof(List));
        public static readonly StringSymbol Part = new StringSymbol(nameof(Part));
        public static readonly StringSymbol Fold = new StringSymbol(nameof(Fold));
        public static readonly StringSymbol Append = new StringSymbol(nameof(Append));

        public static readonly Expression EmptyList = List[new Symbol[0]];

        public static Expression Map {
            get {
                Symbol list = "list";
                Symbol f = "f";

                Symbol acc = "acc";
                Symbol x = "map_x";

                return Fun[list, Fun[f,
                    Fold[list, EmptyList, Fun[acc, Fun[x,
                        Append[acc, f[x]]
                    ]]]
                ]];
            }
        }

        public static Expression Filter {
            get {
                Symbol list = "list";
                Symbol f = "f";

                Symbol acc = "acc";
                Symbol x = "map_x";

                return Fun[list, Fun[f,
                    Fold[list, EmptyList, Fun[acc, Fun[x,
                        If[f[x], 
                            Append[acc, x],
                            acc,
                            "Error"
                        ]
                    ]]]
                ]];
            }
        }
    }
}