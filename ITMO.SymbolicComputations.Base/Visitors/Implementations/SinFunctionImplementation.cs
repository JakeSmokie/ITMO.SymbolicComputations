using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public sealed class SinFunctionImplementation : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, ArithmeticFunctions.Sin)) return expression;

            if (!(expression.Arguments[0] is Constant x)) throw new NotImplementedException();

            return (decimal) Math.Sin((double) x.Value);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}