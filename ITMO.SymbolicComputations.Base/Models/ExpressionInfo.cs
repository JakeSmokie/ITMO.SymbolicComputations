namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class ExpressionInfo {
        public ExpressionInfo(ISymbol symbol) {
            Symbol = symbol;
        }

        public ISymbol Symbol { get; }
    }
}