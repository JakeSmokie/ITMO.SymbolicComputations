using ITMO.SymbolicComputations.Base.Tools;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class ExpressionInfo {
        public BaseSymbol BaseSymbol { get; }

        public ExpressionInfo(BaseSymbol baseSymbol) {
            BaseSymbol = baseSymbol;
        }

        public ExpressionInfo Simplify() => 
            new ExpressionInfo(BaseSymbol.Simplify());
    }
}