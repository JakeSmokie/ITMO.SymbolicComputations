using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public static class ArithmeticFunctions {
        public static readonly StringSymbol BinaryPlus = new StringSymbol(nameof(BinaryPlus));
        public static readonly StringSymbol BinaryTimes = new StringSymbol(nameof(BinaryTimes));
        public static readonly StringSymbol Power = new StringSymbol(nameof(Power));
        
        public static readonly StringSymbol Sin = new StringSymbol(nameof(Sin));
    }
}