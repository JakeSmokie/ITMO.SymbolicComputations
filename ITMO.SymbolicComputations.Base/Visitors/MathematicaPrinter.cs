using System.Globalization;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public sealed class MathematicaPrinter : ISymbolVisitor<string> {
        public string VisitExpression(Expression expression) =>
            expression.Head.Visit(this) +
            $"[{string.Join(", ", expression.Arguments.Select(a => a.Visit(this)))}]";

        public string VisitSymbol(StringSymbol symbol) =>
            symbol.Name;

        public string VisitConstant(Constant constant) =>
            constant.Value.ToString(CultureInfo.InvariantCulture);
    }
}