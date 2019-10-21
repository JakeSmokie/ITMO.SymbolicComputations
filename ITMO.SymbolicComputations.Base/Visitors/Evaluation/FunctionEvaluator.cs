using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FunctionEvaluator : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!(expression.Head is Expression head)) {
                return expression;
            }

            if (!Equals(head.Head, Function)) {
                return expression;
            }

            var variableSymbol = head.Arguments[0];

            if (!(variableSymbol is StringSymbol variable)) {
                return expression;
            }

            var funcArgument = expression.Arguments[0];

            var function = head.Arguments[1];
            var substituted = function.Visit(new VariableReplacer(variable, funcArgument));

            return substituted;
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}