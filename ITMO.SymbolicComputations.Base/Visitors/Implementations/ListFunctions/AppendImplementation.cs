using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class AppendImplementation : AbstractListFunctionImplementation {
        public AppendImplementation() : base(Append) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) => 
            List[items.AddRange(expression.Arguments.Skip(1)).ToArray()];
    }
}