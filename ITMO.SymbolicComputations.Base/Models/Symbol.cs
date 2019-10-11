namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Symbol : IBaseSymbol {
        public Symbol(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}