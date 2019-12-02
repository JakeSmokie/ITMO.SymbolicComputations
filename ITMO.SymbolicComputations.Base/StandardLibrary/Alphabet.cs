using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class Alphabet {
        public static StringSymbol x => new StringSymbol("x'");
        public static StringSymbol y => new StringSymbol("y'");
        public static StringSymbol n => new StringSymbol("n'");
        public static StringSymbol expr => new StringSymbol("expr'");
        public static StringSymbol list => new StringSymbol("list'");
        public static StringSymbol list2 => new StringSymbol("list2'");
        public static StringSymbol tuple => new StringSymbol("tuple'");
        public static StringSymbol f => new StringSymbol("f'");
        public static StringSymbol acc => new StringSymbol("acc'");
        public static StringSymbol pred => new StringSymbol("pred'");
        public static StringSymbol body => new StringSymbol("body'");
    }
}