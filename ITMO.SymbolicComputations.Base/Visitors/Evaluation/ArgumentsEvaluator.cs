using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class ArgumentsEvaluator : ISymbolVisitor<(ImmutableList<Symbol> Steps, Symbol Symbol)> {
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

        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var (headSteps, head) = expression.Head.Visit(FullEvaluator);
            var args = EvaluateArguments(head, expression.Arguments);

            var argSteps = args.SelectMany(a => a.Steps).ToImmutableList();
            var arguments = args.Select(a => a.Symbol).ToImmutableList();

            return (headSteps.AddRange(argSteps), new Expression(head, arguments));
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) =>
            (ImmutableList<Symbol>.Empty, symbol);

        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) =>
            (ImmutableList<Symbol>.Empty, constant);

        private static ImmutableList<(ImmutableList<Symbol> Steps, Symbol Symbol)> EvaluateArguments(
            Symbol head,
            ImmutableList<Symbol> arguments
        ) {
            if (head.Visit(HoldAllCompleteChecker)) {
                return arguments
                    .Select(a => (ImmutableList<Symbol>.Empty, a))
                    .ToImmutableList();
            }

            if (head.Visit(HoldAllChecker)) {
                return EvaluateEagerly(arguments)
                    .ToImmutableList();
            }

            if (head.Visit(HoldRestChecker)) {
                return ImmutableList<(ImmutableList<Symbol>, Symbol)>.Empty
                    .Add(arguments.First().Visit(FullEvaluator))
                    .AddRange(EvaluateEagerly(arguments.Skip(1)));
            }

            if (head.Visit(HoldFirstChecker)) {
                return ImmutableList<(ImmutableList<Symbol>, Symbol)>.Empty
                    .AddRange(EvaluateEagerly(arguments.Take(1)))
                    .AddRange(arguments.Skip(1).Select(a => a.Visit(FullEvaluator)));
            }

            return arguments
                .Select(a => a.Visit(FullEvaluator))
                .ToImmutableList();
        }

        private static IEnumerable<(ImmutableList<Symbol>, Symbol)> EvaluateEagerly(IEnumerable<Symbol> symbols) =>
            symbols.SelectMany(s =>
                s is Expression e && Equals(e.Head, Functions.Evaluate)
                    ? e.Arguments.Select(a => a.Visit(FullEvaluator))
                    : new[] {(ImmutableList<Symbol>.Empty, s)}
            );
    }
}