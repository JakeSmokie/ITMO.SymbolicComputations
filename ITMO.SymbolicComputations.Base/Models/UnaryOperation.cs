namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class UnaryOperation : Expression {
        public Expression Expression { get; }
        public string Name { get; }

        public UnaryOperation(Expression expression, string name) {
            Expression = expression;
            Name = name;
        }
    }
}