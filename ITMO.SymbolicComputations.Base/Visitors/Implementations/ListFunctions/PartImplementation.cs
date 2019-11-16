using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class PartImplementation : AbstractListFunctionImplementation {
        public PartImplementation() : base(Part) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) {
            if (!(expression.Arguments[1] is Constant index)) {
                throw new ArgumentException();
            }

            var indexValue = (int) index.Value;
            return items[indexValue];
        }
    }
}