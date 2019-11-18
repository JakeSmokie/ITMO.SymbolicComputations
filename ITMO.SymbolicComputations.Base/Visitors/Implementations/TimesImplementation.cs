using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class TimesImplementation : AbstractFunctionImplementation {
        public TimesImplementation() : base(Times) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var constants = expression.Arguments
                .Select(x => x.Visit(AsConstantVisitor.Instance))
                .ToList();

            if (constants.Any(x => x == null)) {
                return expression;
            }

            return constants
                .Where(x => x != null)
                .Select(x => x.Value)
                .Aggregate(1m, (acc, x) => acc * x);
        }
    }
}