using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ChartFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Web.Visitors {
    public class FormInputReader : ISymbolVisitor<(Symbol context, ImmutableArray<(string, decimal)> inputs)> {
        public (Symbol context, ImmutableArray<(string, decimal)> inputs) VisitExpression(Expression expression) {
            var expressions = expression.Arguments
                .OfType<Expression>()
                .Where(x => Equals(x.Head, FormInput))
                .ToArray();
            
            var assignments = expressions
                .Select(x => Set[x.Arguments[0], x.Arguments[1]])
                .Cast<Symbol>()
                .ToArray();

            var variables = expressions
                .GroupBy(x => x.Arguments[0].ToString())
                .Select(x => x.First())
                .Select(x => (x.Arguments[0].ToString(), (x.Arguments[1] as Constant)?.Value ?? 0))
                .ToImmutableArray();

            return (Seq[assignments], variables);
        }

        public (Symbol context, ImmutableArray<(string, decimal)> inputs) VisitConstant(Constant constant) =>
            throw new System.NotImplementedException();

        public (Symbol context, ImmutableArray<(string, decimal)> inputs) VisitSymbol(StringSymbol symbol) =>
            throw new System.NotImplementedException();
    }
}