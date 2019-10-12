﻿using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public interface ISymbolVisitor<out T> {
        T VisitFunction(Expression expression);
        T VisitSymbol(StringSymbol symbol);
        T VisitConstant(Constant constant);
    }
}