using System.Collections.Generic;
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
        public static readonly FullEvaluator Default = new FullEvaluator();
        
        private static readonly OneIdentityShrinker OneIdentityShrinker = new OneIdentityShrinker();
        private static readonly HoldFormImplementation HoldFormImplementation = new HoldFormImplementation();
        private static readonly FlatFlattener FlatFlattener = new FlatFlattener();
        private static readonly ArgumentsSorter ArgumentsSorter = new ArgumentsSorter();
        
        private static readonly PlusImplementation PlusImplementation = new PlusImplementation();
        private static readonly TimesImplementation TimesImplementation = new TimesImplementation();
        private static readonly SinFunctionImplementation SinFunctionImplementation = new SinFunctionImplementation();
        private static readonly IfImplementation IfImplementation = new IfImplementation();
        private static readonly PartImplementation PartImplementation = new PartImplementation();
        private static readonly AppendImplementation AppendImplementation = new AppendImplementation();
        private static readonly EqImplementation EqImplementation = new EqImplementation();
        private static readonly CompareImplementation CompareImplementation = new CompareImplementation();
        
        private static readonly AsConstantImplementation AsConstant = new AsConstantImplementation();
        private static readonly AsStringSymbolImplementation AsStringSymbol = new AsStringSymbolImplementation();
        private static readonly AsExpressionArgsImplementation AsExpressionArgs = new AsExpressionArgsImplementation();
        private static readonly ApplyListImplementation ApplyListImplementation = new ApplyListImplementation();

        private readonly ArgumentsEvaluator _argumentsEvaluator;
        private readonly FunctionEvaluator _functionEvaluator;
        private readonly ImmutableList<Expression> _preprocessors;

        public FullEvaluator(ImmutableList<Expression> preprocessors = null) {
            _preprocessors = preprocessors ?? ImmutableList<Expression>.Empty;
            _argumentsEvaluator = new ArgumentsEvaluator(this);
            _functionEvaluator = new FunctionEvaluator(this);
        }

        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var visitors = GetFlow();
            
            var (argSteps, argSymbol) = expression.Visit(_argumentsEvaluator);
            var (funcSteps, funcSymbol) = argSymbol.Visit(_functionEvaluator);

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

        private ImmutableList<ISymbolVisitor<Symbol>> GetFlow() {
            var foldImplementation = new FoldImplementation(this);
            var flow = new ISymbolVisitor<Symbol>[] {
                FlatFlattener,
                // Implementations
                PlusImplementation,
                TimesImplementation,
                SinFunctionImplementation,
                IfImplementation,
                EqImplementation,
                CompareImplementation,
                PartImplementation,
                foldImplementation,
                AppendImplementation,
                AsConstant,
                AsStringSymbol,
                AsExpressionArgs,
                ApplyListImplementation,
                // Last
                ArgumentsSorter,
                OneIdentityShrinker,
                HoldFormImplementation
            }.ToImmutableList();
            
            return flow;
        }
    }
}