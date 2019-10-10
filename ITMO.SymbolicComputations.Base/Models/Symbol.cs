namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Symbol : BaseSymbol {
        public string Name { get; }

        public Symbol(string name) {
            Name = name;
        }
    }
}