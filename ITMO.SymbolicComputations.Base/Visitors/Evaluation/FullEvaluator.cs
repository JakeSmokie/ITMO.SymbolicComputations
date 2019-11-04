using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using ITMO.SymbolicComputations.Base.Visitors.Implementations;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.PlusFunction;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.PowerFunction;
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
        private static readonly ConstantsPowerEvaluator ConstantsPowerEvaluator = new ConstantsPowerEvaluator();
        private static readonly PlusSymbolsReducer PlusSymbolsReducer = new PlusSymbolsReducer();
        private static readonly TimesInPowerSplitter TimesInPowerSplitter = new TimesInPowerSplitter();
        private static readonly NestedPowerFlattener NestedPowerFlattener = new NestedPowerFlattener();
        private static readonly TimesPowersReducer TimesPowersReducer = new TimesPowersReducer();
        private static readonly FunctionEvaluator FunctionEvaluator = new FunctionEvaluator();
        private static readonly SinFunctionImplementation SinFunctionImplementation = new SinFunctionImplementation();

        public (ImmutableList<Symbol>, Symbol) VisitFunction(Expression expression) {
            var visitors = new ISymbolVisitor<Symbol>[] {
                FlatFlattener,
                // Implementations
                TimesConstantsReducer,
                TimesSymbolsReducer,
                PlusConstantsReducer,
                PlusSymbolsReducer,
                TimesInPowerSplitter,
                NestedPowerFlattener,
                TimesPowersReducer,
                ConstantsPowerEvaluator,
                SinFunctionImplementation,
                // Last
                ArgumentsSorter,
                OneIdentityShrinker,
                HoldFormImplementation
            };

            var (argSteps, argSymbol) = expression.Visit(ArgumentsEvaluator);
            var (funcSteps, funcSymbol) = argSymbol.Visit(FunctionEvaluator);

            var steps = ImmutableList<Symbol>.Empty
                .AddRange(argSteps)
                .Add(argSymbol)
                .AddRange(funcSteps);

            return visitors.Aggregate(
                (steps, funcSymbol),
                (state, visitor) => {
                    var (steps, symbol) = state;
                    var visited = symbol.Visit(visitor);

                    return (steps.Add(visited), visited);
                });
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) =>
            (ImmutableList<Symbol>.Empty, symbol);

        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) =>
            (ImmutableList<Symbol>.Empty, constant);
    }
}