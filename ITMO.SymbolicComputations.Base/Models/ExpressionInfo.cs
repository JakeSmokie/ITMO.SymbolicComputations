namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class ExpressionInfo {
        public ExpressionInfo(IBaseSymbol baseSymbol) {
            BaseSymbol = baseSymbol;
        }

        public IBaseSymbol BaseSymbol { get; }
    }
}