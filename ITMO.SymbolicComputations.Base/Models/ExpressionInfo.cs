namespace ITMO.SymbolicComputations.Base.Models {
    public class ExpressionInfo {
        public Expression Expression { get; }

        public ExpressionInfo(Expression expression) {
            Expression = expression;
        }
    }
}