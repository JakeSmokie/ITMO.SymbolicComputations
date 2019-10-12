using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Attributes {
        public static readonly StringSymbol HoldAll = new StringSymbol("HoldAll");
        public static readonly StringSymbol Flat = new StringSymbol("Flat");
        public static readonly StringSymbol OneIdentity = new StringSymbol("OneIdentity");
    }
}