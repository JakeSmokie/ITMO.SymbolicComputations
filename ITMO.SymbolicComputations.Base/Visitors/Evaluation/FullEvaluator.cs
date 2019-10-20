using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using ITMO.SymbolicComputations.Base.Visitors.Implementations;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.PlusFunction;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.TimesFunction;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FullEvaluator : ISymbolVisitor<(ImmutableList<Symbol> Steps, Symbol Symbol)> {
        private static readonly OneIdentityShrinker OneIdentityShrinker = new OneIdentityShrinker();
        private static readonly ArgumentsEvaluator ArgumentsEvaluator = new ArgumentsEvaluator();
        private static readonly HoldFormImplementation HoldFormImplementation = new HoldFormImplementation();
        private static readonly FlatFlattener FlatFlattener = new FlatFlattener();
        private static readonly ArgumentsSorter ArgumentsSorter = new ArgumentsSorter();
        private static readonly TimesConstantsReducer TimesConstantsReducer = new TimesConstantsReducer();
        private static readonly TimesSymbolsReducer TimesSymbolsReducer = new TimesSymbolsReducer();
        private static readonly PlusConstantsReducer PlusConstantsReducer = new PlusConstantsReducer();
        private static readonly PlusSymbolsReducer PlusSymbolsReducer = new PlusSymbolsReducer();

        public (ImmutableList<Symbol>, Symbol) VisitFunction(Expression expression) {
            var visitors = new ISymbolVisitor<Symbol>[] {
                FlatFlattener,
                TimesConstantsReducer,
                TimesSymbolsReducer,
                PlusConstantsReducer,
                PlusSymbolsReducer,
                // Last
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

                    return (steps.Add(visited), visited);
                });
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) => (ImmutableList<Symbol>.Empty, symbol);
        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) => (ImmutableList<Symbol>.Empty, constant);
    }
}