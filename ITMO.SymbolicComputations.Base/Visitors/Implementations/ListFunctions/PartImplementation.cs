using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class PartImplementation : AbstractListFunctionImplementation {
        public PartImplementation() : base(Part) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) {
            var variable = expression.Arguments[0].Visit(AsConstantVisitor.Instance);

            if (variable == null) {
                throw new ArgumentException("Syntax only constant as argument");
            }
            
//            if (!(expression.Arguments[1] is Constant index)) {
//                throw new ArgumentException();
//            }
//
            var indexValue = (int) variable.Value;
            return items[indexValue];
        }
    }
}