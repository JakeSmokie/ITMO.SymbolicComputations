namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class UnaryOperation : BaseSymbol {
        public UnaryOperation(BaseSymbol baseSymbol, string name) {
            BaseSymbol = baseSymbol;
            Name = name;
        }

        public BaseSymbol BaseSymbol { get; }
        public string Name { get; }
    }
}