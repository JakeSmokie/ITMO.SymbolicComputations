using System;
using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FunctionEvaluator : ISymbolVisitor<(ImmutableList<Symbol>, Symbol)> {
        private readonly FullEvaluator fullEvaluator;

        public FunctionEvaluator(FullEvaluator fullEvaluator) {
            this.fullEvaluator = fullEvaluator;
        }

        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var funcHead = expression.Head.Visit(AsExpressionVisitor.Instance);

            if (funcHead == null) {
                return (ImmutableList<Symbol>.Empty, expression);
            }

            if (!Equals(funcHead.Head, Fun)) {
                return (ImmutableList<Symbol>.Empty, expression);
            }

            if (funcHead.Arguments.Count != 2) {
                throw new ArgumentException("Function declaration should contain only 2 arguments");
            }

            var funParameter = funcHead.Arguments[0];
            var funBody = funcHead.Arguments[1];

            var listParameters = funParameter.Visit(AsExpressionVisitor.Instance);
            if (listParameters != null && Equals(listParameters.Head, List)) {
                // Replace list
                return listParameters.Arguments
                    .Zip(expression.Arguments)
                    .Aggregate(
                        funBody,
                        (acc, x) => acc.Visit(new VariableReplacer(x.First, x.Second, true))
                    ).Visit(fullEvaluator);
            }

            var variable = funParameter.Visit(AsStringSymbolVisitor.Instance);

            if (variable == null) {
                throw new ArgumentException("Fun parameter can be only StringSymbol or List. Something gone wrong");
            }

            var functionArgument = expression.Arguments[0];
            var substituted = funBody.Visit(new VariableReplacer(variable, functionArgument, true));

            return substituted.Visit(fullEvaluator);
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) =>
            (ImmutableList<Symbol>.Empty, symbol);

        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) =>
            (ImmutableList<Symbol>.Empty, constant);
    }
}