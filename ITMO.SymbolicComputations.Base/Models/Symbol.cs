namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Symbol : Expression {
        public string Name { get; }

        public Symbol(string name) {
            Name = name;
        }
    }
}