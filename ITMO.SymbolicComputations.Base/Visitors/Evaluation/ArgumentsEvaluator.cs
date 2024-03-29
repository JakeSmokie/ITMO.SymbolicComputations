﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class ArgumentsEvaluator : ISymbolVisitor<(ImmutableList<Symbol> Steps, Symbol Symbol)> {
        private static readonly HasAttributeChecker HoldAllCompleteChecker = new HasAttributeChecker(HoldAllComplete);
        private static readonly HasAttributeChecker HoldAllChecker = new HasAttributeChecker(HoldAll);
        private static readonly HasAttributeChecker HoldRestChecker = new HasAttributeChecker(HoldRest);
        private static readonly HasAttributeChecker HoldFirstChecker = new HasAttributeChecker(HoldFirst);

        private readonly FullEvaluator fullEvaluator;

        public ArgumentsEvaluator(FullEvaluator fullEvaluator) {
            this.fullEvaluator = fullEvaluator;
        }

        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var (headSteps, head) = expression.Head.Visit(fullEvaluator);
            var args = EvaluateArguments(head, expression.Arguments);

            var argSteps = args.SelectMany(a => a.Steps).ToImmutableList();
            var arguments = args.Select(a => a.Symbol).ToImmutableList();

            return (headSteps.AddRange(argSteps), new Expression(head, arguments));
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) =>
            (ImmutableList<Symbol>.Empty, symbol);

        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) =>
            (ImmutableList<Symbol>.Empty, constant);

        private ImmutableList<(ImmutableList<Symbol> Steps, Symbol Symbol)> EvaluateArguments(
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
                    .Add(arguments.First().Visit(fullEvaluator))
                    .AddRange(EvaluateEagerly(arguments.Skip(1)));
            }

            if (head.Visit(HoldFirstChecker)) {
                return ImmutableList<(ImmutableList<Symbol>, Symbol)>.Empty
                    .AddRange(EvaluateEagerly(arguments.Take(1)))
                    .AddRange(arguments.Skip(1).Select(a => a.Visit(fullEvaluator)));
            }

            return arguments
                .Select(a => a.Visit(fullEvaluator))
                .ToImmutableList();
        }

        private IEnumerable<(ImmutableList<Symbol>, Symbol)> EvaluateEagerly(IEnumerable<Symbol> symbols) =>
            symbols.SelectMany(s => {
                var e = s.Visit(AsExpressionVisitor.Instance);
                
                return e != null && Equals(e.Head, Functions.Evaluate)
                    ? e.Arguments.Select(a => a.Visit(fullEvaluator))
                    : new[] {(ImmutableList<Symbol>.Empty, s)};
            });
    }
}