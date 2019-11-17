using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class BinaryPlusImplementation : AbstractFunctionImplementation {
        public BinaryPlusImplementation() : base(BinaryPlus) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var first = expression.Arguments[0].Visit(AsConstantVisitor.Instance);
            var second = expression.Arguments[1].Visit(AsConstantVisitor.Instance);

            if (first == null || second == null) {
                throw new ArgumentException("Syntax only constant as argument");
            }

            return first.Value + second.Value;
        }
    }
}