using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Predefined {
    public class Boolean {
        public static readonly StringSymbol True = new StringSymbol(nameof(True));
        public static readonly StringSymbol False = new StringSymbol(nameof(False));
    }
}