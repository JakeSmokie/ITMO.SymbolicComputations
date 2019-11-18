using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using ITMO.SymbolicComputations.Base.Visitors.Casting;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public sealed class SinFunctionImplementation : AbstractFunctionImplementation {
        public SinFunctionImplementation() : base(ArithmeticFunctions.Sin) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var variable = expression.Arguments[0].Visit(AsConstantVisitor.Instance);

            if (variable == null) {
                throw new ArgumentException("Syntax only constant as argument");
            }

            return (decimal) Math.Sin((double) variable.Value);
        }
    }
}