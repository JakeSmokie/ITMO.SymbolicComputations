using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class Attributes {
        public static readonly StringSymbol HoldAll = new StringSymbol(nameof(HoldAll));
        public static readonly StringSymbol HoldRest = new StringSymbol(nameof(HoldRest));
        public static readonly StringSymbol HoldFirst = new StringSymbol(nameof(HoldFirst));
        public static readonly StringSymbol HoldAllComplete = new StringSymbol(nameof(HoldAllComplete));

        public static readonly StringSymbol Flat = new StringSymbol(nameof(Flat));
        public static readonly StringSymbol OneIdentity = new StringSymbol(nameof(OneIdentity));
        public static readonly StringSymbol Orderless = new StringSymbol(nameof(Orderless));
    }
}