namespace ITMO.SymbolicComputations.Base.Models {
    public class Symbol : Expression {
        public string Name { get; }

        public Symbol(string name) {
            Name = name;
        }
    }
}