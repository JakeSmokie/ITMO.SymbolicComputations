namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class UnaryOperation : BaseSymbol {
        public BaseSymbol BaseSymbol { get; }
        public string Name { get; }

        public UnaryOperation(BaseSymbol baseSymbol, string name) {
            BaseSymbol = baseSymbol;
            Name = name;
        }
    }
}