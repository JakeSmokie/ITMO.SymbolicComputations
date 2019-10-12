using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class AttributesEvaluator : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker HoldAllCompleteChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldAllComplete);

        private static readonly HasAttributeChecker HoldAllChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldAll);

        private static readonly HasAttributeChecker HoldRestChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldRest);

        private static readonly HasAttributeChecker HoldFirstChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldFirst);

        private static readonly FullEvaluator FullEvaluator =
            new FullEvaluator();

        public Symbol VisitFunction(Expression expression) {
            var head = expression.Head.Visit(FullEvaluator);
            var arguments = EvaluateArguments(head, expression.Arguments);

            return new Expression(head, arguments);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;

        private static ImmutableList<Symbol> EvaluateArguments(Symbol head, ImmutableList<Symbol> arguments) {
            if (head.Visit(HoldAllCompleteChecker)) {
                return arguments;
            }

            if (head.Visit(HoldAllChecker)) {
                return EvaluateEagerly(arguments)
                    .ToImmutableList();
            }

            if (head.Visit(HoldRestChecker)) {
                return ImmutableList<Symbol>.Empty
                    .Add(arguments.First().Visit(FullEvaluator))
                    .AddRange(EvaluateEagerly(arguments.Skip(1)));
            }

            if (head.Visit(HoldFirstChecker)) {
                return ImmutableList<Symbol>.Empty
                    .AddRange(EvaluateEagerly(arguments.Take(1)))
                    .AddRange(arguments.Skip(1).Select(a => a.Visit(FullEvaluator)));
            }

            return arguments
                .Select(a => a.Visit(FullEvaluator))
                .ToImmutableList();
        }

        private static IEnumerable<Symbol> EvaluateEagerly(IEnumerable<Symbol> symbols) =>
            symbols.OfType<Expression>().SelectMany(e =>
                e.Head == Functions.Evaluate
                    ? e.Arguments.Select(a => a.Visit(FullEvaluator))
                    : new[] {e}
            );
    }
}