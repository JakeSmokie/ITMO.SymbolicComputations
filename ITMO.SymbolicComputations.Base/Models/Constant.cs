namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : BaseSymbol {
        public Constant(decimal value) {
            Value = value;
        }

        public decimal Value { get; }
    }
}