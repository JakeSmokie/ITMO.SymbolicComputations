using System;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class BinaryTimesImplementation : AbstractFunctionImplementation {
        public BinaryTimesImplementation() : base(BinaryTimes) {
        }

        protected override Symbol Evaluate(Expression expression) {
            if (!(expression.Arguments[0] is Constant first) || !(expression.Arguments[1] is Constant second)) {
                return expression;
            }

            return first.Value * second.Value;
        }
    }
}