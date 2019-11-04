using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FunctionEvaluator : ISymbolVisitor<(ImmutableList<Symbol>, Symbol)> {
        public (ImmutableList<Symbol>, Symbol) VisitFunction(Expression expression) {
            if (!(expression.Head is Expression head)) return (ImmutableList<Symbol>.Empty, expression);

            if (!Equals(head.Head, Function)) return (ImmutableList<Symbol>.Empty, expression);

            var variableSymbol = head.Arguments[0];

            if (!(variableSymbol is StringSymbol variable)) return (ImmutableList<Symbol>.Empty, expression);

            var funcArgument = expression.Arguments[0];

            var function = head.Arguments[1];
            var substituted = function.Visit(new VariableReplacer(variable, funcArgument));

            return substituted.Visit(new FullEvaluator());
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) =>
            (ImmutableList<Symbol>.Empty, symbol);

        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) =>
            (ImmutableList<Symbol>.Empty, constant);
    }
}