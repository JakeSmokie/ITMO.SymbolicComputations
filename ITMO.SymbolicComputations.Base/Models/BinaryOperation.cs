namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class BinaryOperation : BaseSymbol {
        public string Name { get; }
        public BaseSymbol First { get; }
        public BaseSymbol Second { get; }

        public BinaryOperation(BaseSymbol first, BaseSymbol second, string name) {
            First = first;
            Second = second;
            Name = name;
        }
    }
}