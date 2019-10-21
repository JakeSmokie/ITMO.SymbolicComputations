using System;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.PowerFunction {
    public sealed class ConstantsPowerEvaluator : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression power) {
            if (!Equals(power.Head, Power)) {
                return power;
            }

            if (!(power.Arguments[0] is Constant x) || !(power.Arguments[1] is Constant scale)) {
                return power;
            }

            return (decimal) Math.Pow((double) x.Value, (double) scale.Value);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}