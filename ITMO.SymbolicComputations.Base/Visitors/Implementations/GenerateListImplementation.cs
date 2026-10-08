using System;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class GenerateListImplementation : AbstractFunctionImplementation {
        public GenerateListImplementation() : base(GenerateList) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var count = expression.Arguments[0].Visit(AsConstantVisitor.Instance);
            if (count == null) {
                return expression;
            }
            var length = EvaluationBudget.Current != null
                ? EvaluationBudget.Current.ValidateGeneratedCount(count.Value, "GenerateList")
                : ValidateCount(count.Value);

            return List[
                Enumerable.Range(0, length)
                    .Select(x => {
                        EvaluationBudget.Current?.Checkpoint();
                        return new Constant(x);
                    })
                    .OfType<Symbol>()
                    .ToArray()
            ];
        }

        private static int ValidateCount(decimal count) {
            if (count < 0 || decimal.Truncate(count) != count || count > int.MaxValue) {
                throw new ArgumentOutOfRangeException(nameof(count), "GenerateList requires a non-negative integer item count.");
            }
            return (int) count;
        }
    }
}
