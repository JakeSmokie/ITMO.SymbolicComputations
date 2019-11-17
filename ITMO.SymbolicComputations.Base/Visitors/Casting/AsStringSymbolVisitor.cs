using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Casting {
    public sealed class AsStringSymbolVisitor : ISymbolVisitor<StringSymbol>  {
        public StringSymbol VisitFunction(Expression expression) => null;

        public StringSymbol VisitSymbol(StringSymbol symbol) => symbol;

        public StringSymbol VisitConstant(Constant constant) => null;
    }
}