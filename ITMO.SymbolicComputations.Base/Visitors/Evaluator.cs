using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public sealed class Evaluator : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker HoldAllChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldAll);

        private static readonly HasAttributeChecker HoldRestChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldRest);

        private static readonly HasAttributeChecker HoldFirstChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldFirst);

        private static readonly OneIdentityShrinker OneIdentityShrinker =
            new OneIdentityShrinker();

        public Symbol VisitFunction(Expression expression) {
            var head = expression.Head.Visit(this);
            var arguments = EvaluateArguments(head, expression.Arguments);

            return new Expression(head, arguments)
                .Visit(OneIdentityShrinker);
        }

        private ImmutableList<Symbol> EvaluateArguments(Symbol head, ImmutableList<Symbol> arguments) {
            if (head.Visit(HoldAllChecker)) {
                return arguments;
            }

            if (head.Visit(HoldRestChecker)) {
                return ImmutableList<Symbol>.Empty
                    .Add(arguments.First().Visit(this))
                    .AddRange(arguments.Skip(1));
            }

            if (head.Visit(HoldFirstChecker)) {
                return ImmutableList<Symbol>.Empty
                    .Add(arguments.First())
                    .AddRange(arguments.Skip(1).Select(a => a.Visit(this)));
            }

            return arguments
                .Select(a => a.Visit(this))
                .ToImmutableList();
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}