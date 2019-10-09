namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class BinaryOperation : Expression {
        public string Name { get; }
        public Expression First { get; }
        public Expression Second { get; }

        public BinaryOperation(Expression first, Expression second, string name) {
            First = first;
            Second = second;
            Name = name;
        }
    }
}