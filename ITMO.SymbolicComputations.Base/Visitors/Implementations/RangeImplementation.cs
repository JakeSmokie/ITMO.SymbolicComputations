using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class RangeImplementation : AbstractFunctionImplementation {
        public RangeImplementation() : base(Range) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var from = expression.Arguments[0].Visit(AsConstantVisitor.Instance);
            var to = expression.Arguments[1].Visit(AsConstantVisitor.Instance);
            var amount = expression.Arguments[2].Visit(AsConstantVisitor.Instance);

            if (from == null || to == null || amount == null) {
                return expression;
            }

            var step = (to.Value - @from.Value) / amount.Value;

            return List[
                Enumerable.Range(0, (int) amount.Value)
                    .Select(i => from.Value + i * step)
                    .Select(x => new Constant(x))
                    .OfType<Symbol>()
                    .ToArray()
            ];
        }
    }
}