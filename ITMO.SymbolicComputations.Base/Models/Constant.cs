namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : Expression {
        public decimal Value { get; }

        public Constant(decimal value) {
            Value = value;
        }
    }
}