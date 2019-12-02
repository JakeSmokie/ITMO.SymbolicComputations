using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class FastMapImplementation : AbstractListFunctionImplementation {
        public FastMapImplementation() : base(FastMap) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) {
            var f = expression.Arguments[1];

            return List[
                items
                    .Select(x => (Symbol) f[x])
                    .ToArray()
            ];
        }
    }
}