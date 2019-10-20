using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.PlusFunction
{
    public sealed class PlusConstantsReducer : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, ArithmeticFunctions.Plus)) {
                return expression;
            }

            var constants = expression.Arguments
                .OfType<Constant>()
                .ToList();

            if (constants.Count == 0) {
                return expression;
            }

            var others = expression.Arguments
                .Where(x => !(x is Constant));

            var reduce = constants
                .Select(x => x.Value)
                .Aggregate((acc, x) => acc + x);
            
            return new Expression(expression.Head, others.Append(reduce).ToImmutableList());
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}