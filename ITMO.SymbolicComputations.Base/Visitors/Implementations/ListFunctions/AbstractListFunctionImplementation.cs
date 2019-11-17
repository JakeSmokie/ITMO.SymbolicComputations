using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public abstract class AbstractListFunctionImplementation : AbstractFunctionImplementation {
        protected AbstractListFunctionImplementation(StringSymbol name) : base(name) {
        }

        protected abstract Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items); 

        protected override Symbol Evaluate(Expression expression) {
            var list = expression.Arguments[0].Visit(AsExpressionVisitor.Instance);

            if (list == null) {
                throw new ArgumentException("Syntax only constant as argument");
            }

            if (!Equals(list.Head, List)) {
                throw new ArgumentException($"Invalid usage of {_name}: Argument is not a list");
            }
            
            return EvaluateList(expression, list.Arguments);
        }
    }
}