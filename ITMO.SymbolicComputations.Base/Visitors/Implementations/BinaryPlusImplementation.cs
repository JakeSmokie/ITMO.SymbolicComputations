using System;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class BinaryPlusImplementation : AbstractFunctionImplementation {
        public BinaryPlusImplementation() : base(BinaryPlus) {
        }

        protected override Symbol Evaluate(Expression expression) {
            if (!(expression.Arguments[0] is Constant first) || !(expression.Arguments[1] is Constant second)) {
                throw new ArgumentException();
            }

            return first.Value + second.Value;
        }
    }
}