using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using ITMO.SymbolicComputations.Base.Visitors.Implementations;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FullEvaluator : ISymbolVisitor<(ImmutableList<Symbol> Steps, Symbol Symbol)> {
        private static readonly OneIdentityShrinker OneIdentityShrinker = new OneIdentityShrinker();
        private static readonly ArgumentsEvaluator ArgumentsEvaluator = new ArgumentsEvaluator();
        private static readonly HoldFormImplementation HoldFormImplementation = new HoldFormImplementation();
        private static readonly FlatFlattener FlatFlattener = new FlatFlattener();
        private static readonly ArgumentsSorter ArgumentsSorter = new ArgumentsSorter();

        public (ImmutableList<Symbol>, Symbol) VisitFunction(Expression expression) {
            var visitors = new ISymbolVisitor<Symbol>[] {
                FlatFlattener,
                ArgumentsSorter,
                OneIdentityShrinker,
                HoldFormImplementation
            };

            var (argSteps, argSymbol) = expression.Visit(ArgumentsEvaluator);

            return visitors.Aggregate(
                (argSteps.Add(argSymbol), argSymbol),
                (state, visitor) => {
                    var (steps, symbol) = state;
                    var visited = symbol.Visit(visitor);

                    return (visited == symbol ? steps : steps.Add(visited), visited);
                });
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) => (ImmutableList<Symbol>.Empty, symbol);
        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) => (ImmutableList<Symbol>.Empty, constant);
    }
}