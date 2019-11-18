using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using ITMO.SymbolicComputations.Base.Visitors.Implementations;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.BooleanFunctions;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.Casting;
using ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FullEvaluator : ISymbolVisitor<(ImmutableList<Symbol> Steps, Symbol Symbol)> {
        private static readonly OneIdentityShrinker OneIdentityShrinker = new OneIdentityShrinker();
        private static readonly ArgumentsEvaluator ArgumentsEvaluator = new ArgumentsEvaluator();
        private static readonly HoldFormImplementation HoldFormImplementation = new HoldFormImplementation();
        private static readonly FlatFlattener FlatFlattener = new FlatFlattener();
        private static readonly ArgumentsSorter ArgumentsSorter = new ArgumentsSorter();
        private static readonly FunctionEvaluator FunctionEvaluator = new FunctionEvaluator();
        
        private static readonly BinaryPlusImplementation BinaryPlusImplementation = new BinaryPlusImplementation();
        private static readonly TimesImplementation TimesImplementation = new TimesImplementation();
        private static readonly SinFunctionImplementation SinFunctionImplementation = new SinFunctionImplementation();
        private static readonly IfImplementation IfImplementation = new IfImplementation();
        private static readonly PartImplementation PartImplementation = new PartImplementation();
        private static readonly FoldImplementation FoldImplementation = new FoldImplementation();
        private static readonly AppendImplementation AppendImplementation = new AppendImplementation();
        private static readonly EqImplementation EqImplementation = new EqImplementation();
        private static readonly CompareImplementation CompareImplementation = new CompareImplementation();
        
        private static readonly AsConstantImplementation AsConstant = new AsConstantImplementation();
        private static readonly AsStringSymbolImplementation AsStringSymbol = new AsStringSymbolImplementation();
        private static readonly AsExpressionArgsImplementation AsExpressionArgs = new AsExpressionArgsImplementation();

        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var visitors = new ISymbolVisitor<Symbol>[] {
                FlatFlattener,
                // Implementations
                BinaryPlusImplementation,
                TimesImplementation,
                SinFunctionImplementation,
                IfImplementation,
                EqImplementation,
                CompareImplementation,
                PartImplementation,
                FoldImplementation,
                AppendImplementation,
                AsConstant,
                AsStringSymbol,
                AsExpressionArgs,
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