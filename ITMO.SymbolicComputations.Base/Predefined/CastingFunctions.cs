using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class CastingFunctions {
        public static readonly StringSymbol AsConstant = new StringSymbol(nameof(AsConstant));
        public static readonly StringSymbol AsStringSymbol = new StringSymbol(nameof(AsStringSymbol));
        
        public static readonly StringSymbol Null = new StringSymbol(nameof(Null));
    }
}