using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.PlusFunction
{
    public sealed class PlusSymbolsReducer : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, Plus)) {
                return expression;
            }

            var stringSymbols = expression.Arguments
                .OfType<StringSymbol>()
                .ToList();

            if (stringSymbols.Count == 0) {
                return expression;
            }

            var others = expression.Arguments
                .Where(x => !(x is StringSymbol));

            var times = stringSymbols
                .GroupBy(x => x.Name)
                .Select(group =>
                    group.Count() == 1
                        ? (Symbol) group.Key
                        : Times[group.Key, group.Count()]
                );

            return new Expression(expression.Head, others.Concat(times).ToImmutableList());
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;

        public Symbol VisitConstant(Constant constant) => constant;
    }
}