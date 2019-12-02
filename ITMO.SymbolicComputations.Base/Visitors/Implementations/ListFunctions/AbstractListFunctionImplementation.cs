using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public abstract class AbstractListFunctionImplementation : AbstractFunctionImplementation {
        protected AbstractListFunctionImplementation(StringSymbol names) : base(names) {
        }

        protected abstract Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items);

        protected override Symbol Evaluate(Expression expression) {
            var list = expression.Arguments[0].Visit(AsExpressionVisitor.Instance);

            if (list == null || !Equals(list.Head, List)) {
//                throw new ArgumentException($"Invalid usage of {Name}: Argument is not a list: {expression.Arguments[0]}");
                return expression;
            }

            return EvaluateList(expression, list.Arguments);
        }
    }
}