namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class BinaryOperation : Expression {
        public Expression First { get; }
        public Expression Second { get; }
        public string Name { get; }

        public BinaryOperation(Expression first, Expression second, string name) {
            First = first;
            Second = second;
            Name = name;
        }
    }
}