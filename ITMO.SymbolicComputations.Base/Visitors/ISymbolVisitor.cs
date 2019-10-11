using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public interface ISymbolVisitor<out T> {
        T VisitFunction(Function function);
        T VisitSymbol(StringSymbol symbol);
        T VisitConstant(Constant constant);
    }
}