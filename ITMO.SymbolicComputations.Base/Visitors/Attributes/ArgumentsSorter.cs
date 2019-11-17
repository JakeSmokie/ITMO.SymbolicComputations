using System.Collections.Generic;
using ITMO.SymbolicComputations.Base.Comparers;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Attributes {
    public sealed class ArgumentsSorter : ISymbolVisitor<Symbol> {
        private static readonly IComparer<Symbol> Comparer = new SymbolComparer();

        private static readonly HasAttributeChecker OrderlessChecker =
            new HasAttributeChecker(Predefined.Attributes.Orderless);

        public Symbol VisitExpression(Expression expression) =>
            !expression.Head.Visit(OrderlessChecker)
                ? expression
                : new Expression(expression.Head, expression.Arguments.Sort(Comparer));

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;

        public Symbol VisitConstant(Constant constant) => constant;
    }
}