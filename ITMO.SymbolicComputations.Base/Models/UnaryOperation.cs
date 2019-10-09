namespace ITMO.SymbolicComputations.Base.Models {
    public class UnaryOperation {
        public Expression Expression { get; }
        public string Name { get; }

        public UnaryOperation(Expression expression, string name) {
            Expression = expression;
            Name = name;
        }
    }
}