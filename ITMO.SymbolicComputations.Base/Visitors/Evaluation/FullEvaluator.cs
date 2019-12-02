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
        private static readonly PowerImplementation PowerImplementation = new PowerImplementation();

        private static readonly AsConstantImplementation AsConstant = new AsConstantImplementation();
        private static readonly AsStringSymbolImplementation AsStringSymbol = new AsStringSymbolImplementation();
        private static readonly AsExpressionArgsImplementation AsExpressionArgs = new AsExpressionArgsImplementation();
        private static readonly ApplyListImplementation ApplyListImplementation = new ApplyListImplementation();
        private static readonly GenerateListImplementation GenerateList = new GenerateListImplementation();
        private static readonly DivideImplementation DivideImplementation = new DivideImplementation();
        private static readonly LengthImplementation LengthImplementation = new LengthImplementation();
        private static readonly DistinctImplementation DistinctImplementation = new DistinctImplementation();
        private static readonly GroupImplementation GroupImplementation = new GroupImplementation();

        private readonly ArgumentsEvaluator argumentsEvaluator;
        private readonly FunctionEvaluator functionEvaluator;
        private readonly ImmutableList<ISymbolVisitor<Symbol>> visitors;
        private ImmutableList<ISymbolVisitor<Symbol>> flow;

        public FullEvaluator(
            ImmutableList<ISymbolVisitor<Symbol>> visitors = null
        ) {
            this.visitors = visitors ?? ImmutableList<ISymbolVisitor<Symbol>>.Empty;

            argumentsEvaluator = new ArgumentsEvaluator(this);
            functionEvaluator = new FunctionEvaluator(this);
        }

        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var visitors = GetFlow();

            var (argSteps, argSymbol) = expression.Visit(argumentsEvaluator);
            var (funcSteps, funcSymbol) = argSymbol.Visit(functionEvaluator);

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
            if (flow != null) {
                return flow;
            }
            
            flow = visitors.AddRange(new ISymbolVisitor<Symbol>[] {
                FlatFlattener,
                ArgumentsSorter,
                OneIdentityShrinker,
                // Implementations
                PlusImplementation,
                TimesImplementation,
                DivideImplementation,
                SinFunctionImplementation,
                IfImplementation,
                EqImplementation,
                CompareImplementation,
                PartImplementation,
                AppendImplementation,
                AsConstant,
                AsStringSymbol,
                AsExpressionArgs,
                ApplyListImplementation,
                PowerImplementation,
                LengthImplementation,
                DistinctImplementation,
                GroupImplementation,
                GenerateList
                // Last
            });

            return flow;
        }
    }
}