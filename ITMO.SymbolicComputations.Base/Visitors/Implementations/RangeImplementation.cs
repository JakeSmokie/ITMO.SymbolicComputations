using System;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class RangeImplementation : AbstractFunctionImplementation {
        public RangeImplementation() : base(StandardLibrary.ListFunctions.Range) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var from = expression.Arguments[0].Visit(AsConstantVisitor.Instance);
            var to = expression.Arguments[1].Visit(AsConstantVisitor.Instance);
            var amount = expression.Arguments[2].Visit(AsConstantVisitor.Instance);

            if (from == null || to == null || amount == null) {
                return expression;
            }

            var count = EvaluationBudget.Current != null
                ? EvaluationBudget.Current.ValidateGeneratedCount(amount.Value, "Range")
                : ValidateCount(amount.Value);
            if (count == 0) {
                return List[Array.Empty<Symbol>()];
            }
            var step = (to.Value - from.Value) / count;

            return List[
                Enumerable.Range(0, count)
                    .Select(i => {
                        EvaluationBudget.Current?.Checkpoint();
                        return from.Value + i * step;
                    })
                    .Select(x => new Constant(x))
                    .OfType<Symbol>()
                    .ToArray()
            ];
        }

        private static int ValidateCount(decimal count) {
            if (count < 0 || decimal.Truncate(count) != count || count > int.MaxValue) {
                throw new ArgumentOutOfRangeException(nameof(count), "Range requires a non-negative integer item count.");
            }
            return (int) count;
        }
    }
}
