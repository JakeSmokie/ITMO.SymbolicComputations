using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public abstract class AbstractListFunctionImplementation : AbstractFunctionImplementation {
        protected AbstractListFunctionImplementation(StringSymbol name) : base(name) {
        }

        protected abstract Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items); 

        protected override Symbol Evaluate(Expression expression) {
            if (!(expression.Arguments[0] is Expression list)) {
                throw new ArgumentException();
            }

            if (!Equals(list.Head, List)) {
                throw new ArgumentException();
            }
            
            return EvaluateList(expression, list.Arguments);
        }
    }
}