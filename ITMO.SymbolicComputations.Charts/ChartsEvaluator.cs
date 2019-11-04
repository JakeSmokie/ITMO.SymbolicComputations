using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;

namespace ITMO.SymbolicComputations.Charts {
    public sealed class ChartsEvaluator : ISymbolVisitor<ImmutableList<(decimal, decimal)>> {
        private static readonly FullEvaluator FullEvaluator = new FullEvaluator();

        public ImmutableList<(decimal, decimal)> VisitFunction(Expression expression) {
            if (!Equals(expression.Head, Charts.Chart2D)) throw new NotImplementedException();

            if (!(expression.Arguments[1] is Expression range)) throw new NotImplementedException();

            if (!Equals(range.Head, Functions.List)) throw new NotImplementedException();

            if (!(range.Arguments[0] is Constant from) ||
                !(range.Arguments[1] is Constant step) ||
                !(range.Arguments[2] is Constant to))
                throw new NotImplementedException();

            var func = expression.Arguments[0];
            return TabulateFunction().ToImmutableList();

            IEnumerable<(decimal, decimal)> TabulateFunction() {
                for (var i = from.Value; i <= to.Value; i += step.Value)
                    yield return (i,
                        (func[i].Visit(FullEvaluator).Symbol as Constant)?.Value ?? throw new ArgumentException());
            }
        }

        public ImmutableList<(decimal, decimal)> VisitSymbol(StringSymbol symbol) =>
            throw new NotImplementedException();

        public ImmutableList<(decimal, decimal)> VisitConstant(Constant constant) =>
            throw new NotImplementedException();
    }
}