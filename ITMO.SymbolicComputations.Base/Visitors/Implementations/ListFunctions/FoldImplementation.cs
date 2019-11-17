using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class FoldImplementation : AbstractListFunctionImplementation {
        public FoldImplementation() : base(Fold) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) {
            var f = expression.Arguments[2];
            return items.Aggregate(expression.Arguments[1],
                (acc, x) => f[acc][x]
                    .Visit(new FullEvaluator()).Symbol
            );
        }
    }
}