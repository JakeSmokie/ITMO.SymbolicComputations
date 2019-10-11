namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : ISymbol {
        public Constant(decimal value) {
            Value = value;
        }

        public decimal Value { get; }
    }
}