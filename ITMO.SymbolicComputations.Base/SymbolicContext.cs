using System;
using System.Collections.Immutable;
using System.Threading;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ChartFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base {
    public class SymbolicContext {
        private const int MaxIterations = 1000;

        private readonly Symbol additionalContext;
        private readonly ImmutableList<Symbol> topLevelProcessors;
        private readonly int maxIterations;

        public SymbolicContext(
            Symbol additionalContext = null,
            ImmutableList<Symbol> topLevelProcessors = null,
            int? maxIterations = null
        ) {
            this.topLevelProcessors = topLevelProcessors ?? ImmutableList<Symbol>.Empty;
            this.additionalContext = additionalContext ?? "Null";

            this.maxIterations = maxIterations ?? MaxIterations;
        }

        private static Expression DefaultContext => Seq[
            SetDelayed[Plot, PlotImplementation],
            SetDelayed[IsConstant, IsConstantImplementation],
            SetDelayed[IsStringSymbol, IsStringSymbolImplementation],
            SetDelayed[IsExpressionWithName, IsExpressionWithNameImplementation],
            SetDelayed[DefaultValue, DefaultValueImplementation],
            //
            SetDelayed[Contains, ContainsImplementation],
            SetDelayed[Concat, ConcatImplementation],
            SetDelayed[CountItem, CountItemImplementation],
            SetDelayed[Filter, FilterImplementation],
            SetDelayed[Map, MapImplementation],
            SetDelayed[Fold, FoldImplementation],
            //
            SetDelayed[Factorial, FactorialImplementation],
            SetDelayed[TaylorSin, TaylorSinImplementation],
            SetDelayed[ListTimes, ListTimesImplementation],
            SetDelayed[ListPlus, ListPlusImplementation],
            //
            SetDelayed[Minus, MinusImplementation],
            SetDelayed[Or, OrImplementation],
            SetDelayed[And, AndImplementation],
            SetDelayed[More, MoreImplementation],
            SetDelayed[Less, LessImplementation],
            SetDelayed[Not, NotImplementation],
            SetDelayed[While, WhileImplementation]
        ];

        public (ImmutableList<Symbol>, Symbol) Run(Symbol symbol) =>
            Run(symbol, new EvaluationOptions());

        public (ImmutableList<Symbol>, Symbol) Run(
            Symbol symbol,
            EvaluationOptions options,
            CancellationToken cancellationToken = default
        ) {
            using var budget = EvaluationBudget.Begin(options, cancellationToken);
            budget.ValidateInput(symbol);
            budget.ValidateInput(additionalContext);
            var variableAssigner = new VariableAssigner();
            var globalVariablesReplacer = new GlobalVariablesReplacer(variableAssigner);

            var fullEvaluator = new FullEvaluator(
                ImmutableList<ISymbolVisitor<Symbol>>.Empty
                    .Add(variableAssigner)
            );

            var context = Seq[DefaultContext, additionalContext].Visit(fullEvaluator).Symbol;
//            symbol = Seq[context, symbol];

            budget.RecordStep();
            var steps = ImmutableList<Symbol>.Empty.Add(symbol);
            var i = 0;

            while (true) {
                budget.Checkpoint();
                if (i >= maxIterations) {
                    throw new EvaluationLimitException("iterations", "Evaluation exceeded its iteration limit.", maxIterations);
                }
                var (newSteps, newResult) = symbol
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(fullEvaluator);

                if (Equals(newResult, symbol) && i > 0) {
                    return (steps, symbol);
                }

                steps = steps.AddRange(newSteps).WithoutDuplicates();
                symbol = newResult;

                try {
                    if (Logger.Log != null) {
                        Logger.Log($"Iteration: {symbol}");
                    }
                }
                catch (Exception error) when (!(error is EvaluationLimitException) && !(error is OperationCanceledException)) {
                    // ignored
                }

                i++;
            }
        }
    }
}
