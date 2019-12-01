using System.Collections.Generic;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Casting;

namespace ITMO.SymbolicComputations.Charts.Tests {
    public class ListOfListToDecimalTuples : ISymbolVisitor<IEnumerable<(decimal, decimal)>> {
        public IEnumerable<(decimal, decimal)> VisitExpression(Expression expression) {
            foreach (var argument in expression.Arguments) {
                var list = argument.Visit(AsExpressionVisitor.Instance);

                var x = list.Arguments[0].Visit(AsConstantVisitor.Instance).Value;
                var y = list.Arguments[1].Visit(AsConstantVisitor.Instance).Value;

                yield return (x, y);
            }
        }

        public IEnumerable<(decimal, decimal)> VisitSymbol(StringSymbol symbol) => throw new System.NotImplementedException();

        public IEnumerable<(decimal, decimal)> VisitConstant(Constant constant) => throw new System.NotImplementedException();
    }
}