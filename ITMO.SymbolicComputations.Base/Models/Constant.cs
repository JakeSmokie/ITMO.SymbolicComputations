namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : BaseSymbol {
        public decimal Value { get; }

        public Constant(decimal value) {
            Value = value;
        }
    }
}