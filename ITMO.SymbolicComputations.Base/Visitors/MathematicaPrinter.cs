using System.Globalization;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public sealed class MathematicaPrinter : ISymbolVisitor<string> {
        public static readonly MathematicaPrinter Default = new MathematicaPrinter();
        
        public string VisitExpression(Expression expression) {
            var isSeq = Equals(expression.Head, Functions.Seq);
            
            var sep = isSeq ? "\n" : "";
            var tab = isSeq ? "    " : "";
            
            var args = string.Join(", " + sep, expression.Arguments.Select(a => tab + a.Visit(this)));

            return expression.Head.Visit(this) +
                $"[{sep}{args}{sep}]";
        }

        public string VisitSymbol(StringSymbol symbol) =>
            symbol.Name;

        public string VisitConstant(Constant constant) =>
            constant.Value.ToString(CultureInfo.InvariantCulture);
    }
}