namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class StringSymbol : IBaseSymbol {
        public StringSymbol(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}