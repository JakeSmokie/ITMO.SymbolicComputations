namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class ExpressionInfo {
        public ExpressionInfo(BaseSymbol baseSymbol) {
            BaseSymbol = baseSymbol;
        }

        public BaseSymbol BaseSymbol { get; }
    }
}