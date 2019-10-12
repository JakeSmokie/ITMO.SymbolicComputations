using System.Globalization;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public class MathematicaPrintingVisitor : ISymbolVisitor<string> {
        public string VisitFunction(Function function) =>
            function.Symbol.Visit(this) +
            $"[{string.Join(", ", function.Arguments.Select(a => a.Visit(this)))}]";

        public string VisitSymbol(StringSymbol symbol) =>
            symbol.Name;

        public string VisitConstant(Constant constant) =>
            constant.Value.ToString(CultureInfo.InvariantCulture);
    }
}