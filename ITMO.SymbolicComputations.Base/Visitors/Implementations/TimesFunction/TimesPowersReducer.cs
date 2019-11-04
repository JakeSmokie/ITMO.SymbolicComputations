using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.TimesFunction {
    public sealed class TimesPowersReducer : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, Times)) return expression;

            var powers = expression.Arguments
                .Where(IsPower)
                .OfType<Expression>()
                .ToList();

            var others = expression.Arguments
                .Where(x => !IsPower(x))
                .Select(x => Power[x, 1])
                .ToList();

            var grouped = powers.Concat(others)
                .GroupBy(x => x.Arguments[0]);

            var newPowers = grouped.Select(x => {
                var scale = Plus[x.Select(y => y.Arguments[1]).ToArray()]
                    .Visit(new FullEvaluator()).Symbol;

                if (Equals(scale, new Constant(0))) return 1;

                if (Equals(scale, new Constant(1))) return x.Key;

                return Power[
                    x.Key,
                    scale
                ];
            });

            return Times[newPowers.OfType<Symbol>().ToArray()];

            bool IsPower(Symbol symbol) =>
                symbol is Expression e
                && Equals(e.Head, Power);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}