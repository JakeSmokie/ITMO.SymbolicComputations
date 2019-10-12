using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class AttributesEvaluator : ISymbolVisitor<Symbol>  {
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
        
        private static ImmutableList<Symbol> EvaluateArguments(Symbol head, ImmutableList<Symbol> arguments) {
            if (head.Visit(HoldAllChecker)) {
                return arguments;
            }

            if (head.Visit(HoldRestChecker)) {
                return ImmutableList<Symbol>.Empty
                    .Add(arguments.First().Visit(FullEvaluator))
                    .AddRange(arguments.Skip(1));
            }

            if (head.Visit(HoldFirstChecker)) {
                return ImmutableList<Symbol>.Empty
                    .Add(arguments.First())
                    .AddRange(arguments.Skip(1).Select(a => a.Visit(FullEvaluator)));
            }

            return arguments
                .Select(a => a.Visit(FullEvaluator))
                .ToImmutableList();
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}