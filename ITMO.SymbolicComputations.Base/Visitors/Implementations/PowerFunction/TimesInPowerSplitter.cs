using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.PowerFunction {
    public sealed class TimesInPowerSplitter : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, Power)) return expression;

            var times = expression.Arguments[0] as Expression;

            if (times == null || !Equals(times?.Head, Times)) return expression;

            var scale = expression.Arguments[1];

            var arguments = times.Arguments
                .Select(x => Power[x, scale].Visit(new FullEvaluator()).Symbol)
                .ToArray();

            return Times[arguments];
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}