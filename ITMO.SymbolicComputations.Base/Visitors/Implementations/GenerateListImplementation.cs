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

            return List[
                Enumerable.Range(0, (int) count.Value)
                    .Select(x => new Constant(x))
                    .OfType<Symbol>()
                    .ToArray()
            ];
        }
    }
}