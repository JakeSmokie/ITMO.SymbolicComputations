namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class ExpressionInfo {
        public ExpressionInfo(Symbol symbol) {
            Symbol = symbol;
        }

        public Symbol Symbol { get; }
    }
}