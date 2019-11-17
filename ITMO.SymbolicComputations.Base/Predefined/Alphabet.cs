using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Alphabet {
        public static readonly StringSymbol x = new StringSymbol(nameof(x));
        public static readonly StringSymbol y = new StringSymbol(nameof(y));
        
        public static readonly StringSymbol list = new StringSymbol(nameof(list));
        public static readonly StringSymbol f = new StringSymbol(nameof(f));
        public static readonly StringSymbol acc = new StringSymbol(nameof(acc));
    }
}