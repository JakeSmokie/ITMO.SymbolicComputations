namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : IBaseSymbol {
        public Constant(decimal value) {
            Value = value;
        }

        public decimal Value { get; }
    }
}