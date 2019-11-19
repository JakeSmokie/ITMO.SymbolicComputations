using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class Alphabet {
        public static readonly StringSymbol x = new StringSymbol("x'");
        public static readonly StringSymbol y = new StringSymbol("y'");

        public static readonly StringSymbol pred = new StringSymbol("pred'");
        public static readonly StringSymbol body = new StringSymbol("body'");

        public static readonly StringSymbol expr = new StringSymbol("expr'");
        
        public static readonly StringSymbol list = new StringSymbol("list'");
        public static readonly StringSymbol list2 = new StringSymbol("list2'");
        
        public static readonly StringSymbol tuple = new StringSymbol("tuple'");
        
        public static readonly StringSymbol f = new StringSymbol("f'");
        public static readonly StringSymbol g = new StringSymbol("g'");
        public static readonly StringSymbol acc = new StringSymbol("acc'");
    }
}