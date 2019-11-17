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
            var index = expression.Arguments[1].Visit(AsConstantVisitor.Instance);

            if (index == null) {
                throw new ArgumentException("Syntax only constant as argument");
            }

            var indexValue = (int) index.Value;
            return items[indexValue];
        }
    }
}