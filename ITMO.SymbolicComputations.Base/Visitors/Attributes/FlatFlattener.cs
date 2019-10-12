using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Attributes {
    public sealed class FlatFlattener : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker FlatChecker =
            new HasAttributeChecker(Predefined.Attributes.Flat);

        public Symbol VisitFunction(Expression expression) {
            if (!expression.Head.Visit(FlatChecker)) {
                return expression;
            }

            return new Expression(expression.Head,
                expression.Arguments.SelectMany(a =>
                    a is Expression e && e.Head == expression.Head
                        ? e.Arguments.AsEnumerable()
                        : Enumerable.Repeat(a, 1)
                ).ToImmutableList()
            );
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}