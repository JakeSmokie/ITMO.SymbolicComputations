using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Casting {
    public sealed class AsConstantVisitor : ISymbolVisitor<Constant>  {
        public Constant VisitFunction(Expression expression) => null;

        public Constant VisitSymbol(StringSymbol symbol) => null;

        public Constant VisitConstant(Constant constant) => constant;
    }
}