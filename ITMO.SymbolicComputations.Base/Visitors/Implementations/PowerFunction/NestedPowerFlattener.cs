using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.PowerFunction {
    public sealed class NestedPowerFlattener : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression power) {
            if (!Equals(power.Head, Power)) return power;

            var nestedPower = power.Arguments[0] as Expression;

            if (nestedPower == null || !Equals(nestedPower?.Head, Power)) return power;

            var x = nestedPower.Arguments[0];
            var firstScale = power.Arguments[1];
            var secondScale = nestedPower.Arguments[1];

            return Power[x, Times[firstScale, secondScale].Visit(new FullEvaluator()).Symbol];
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}