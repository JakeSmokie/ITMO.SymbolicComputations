using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class ArithmeticFunctions {
        public static readonly StringSymbol BinaryPlus = new StringSymbol(nameof(BinaryPlus));
        public static readonly StringSymbol BinaryTimes = new StringSymbol(nameof(BinaryTimes));
        public static readonly StringSymbol Power = new StringSymbol(nameof(Power));
        
        public static readonly StringSymbol Sin = new StringSymbol(nameof(Sin));

        public static readonly Expression ListPlus =
            Fun[list,
                Fold[list, 0, Fun[acc, Fun[x, BinaryPlus[acc, x]]]]
            ];

        public static readonly Expression ListTimes =
            Fun[list,
                Fold[list, 1, Fun[acc, Fun[x, BinaryTimes[acc, x]]]]
            ];
    }
}