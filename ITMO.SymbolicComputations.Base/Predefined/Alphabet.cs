using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Alphabet {
        public static readonly StringSymbol x = new StringSymbol(nameof(x));
        public static readonly StringSymbol y = new StringSymbol(nameof(y));
    }
}