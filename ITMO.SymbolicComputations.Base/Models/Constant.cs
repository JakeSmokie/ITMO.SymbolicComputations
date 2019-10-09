namespace ITMO.SymbolicComputations.Base.Models {
    public class Constant : Expression {
        public string Value { get; }

        public Constant(string value) {
            Value = value;
        }
    }
}