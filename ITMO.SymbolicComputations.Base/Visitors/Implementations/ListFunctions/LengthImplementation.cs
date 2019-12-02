using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class LengthImplementation : AbstractListFunctionImplementation {
        public LengthImplementation() : base(Length) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) {
            return items.Count;
        }
    }
}