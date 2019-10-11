namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Symbol : BaseSymbol {
        public Symbol(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}