using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.TimesFunction {
    public sealed class TimesConstantsReducer : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, ArithmeticFunctions.Times)) return expression;

            var constants = expression.Arguments
                .OfType<Constant>()
                .ToList();

            if (constants.Count == 0) return expression;

            if (constants.Any(x => x.Value == 0)) return 0;

            var others = expression.Arguments
                .Where(x => !(x is Constant));

            var reduce = constants
                .Select(x => x.Value)
                .Aggregate(1m, (acc, x) => acc * x);

            return new Expression(expression.Head, others.Append(reduce).ToImmutableList());
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}