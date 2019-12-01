using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class Alphabet {
        public static StringSymbol x => new StringSymbol("x_");
        public static StringSymbol y => new StringSymbol("y_");
        public static StringSymbol n => new StringSymbol("n_");
        public static StringSymbol expr => new StringSymbol("expr_");
        public static StringSymbol list => new StringSymbol("list_");
        public static StringSymbol list2 => new StringSymbol("list2_");
        public static StringSymbol tuple => new StringSymbol("tuple_");
        public static StringSymbol f => new StringSymbol("f_");
        public static StringSymbol acc => new StringSymbol("acc_");
    }
}