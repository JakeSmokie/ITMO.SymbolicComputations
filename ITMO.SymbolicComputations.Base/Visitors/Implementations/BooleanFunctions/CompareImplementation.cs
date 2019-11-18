using ITMO.SymbolicComputations.Base.Comparers;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.BooleanFunctions {
    public class CompareImplementation : AbstractFunctionImplementation {
        private static readonly SymbolComparer SymbolComparer = new SymbolComparer();

        public CompareImplementation() : base(Compare) {
        }

        protected override Symbol Evaluate(Expression expression) =>
            SymbolComparer.Compare(expression.Arguments[0], expression.Arguments[1]);
    }
}