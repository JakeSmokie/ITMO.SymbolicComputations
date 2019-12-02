using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
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
            SetDelayed[IsConstant, IsConstantImplementation],
            SetDelayed[IsStringSymbol, IsStringSymbolImplementation],
            SetDelayed[IsExpressionWithName, IsExpressionWithNameImplementation],
            SetDelayed[DefaultValue, DefaultValueImplementation],
            //
            SetDelayed[Range, RangeImplementation],
            SetDelayed[Group, GroupImplementation],
            SetDelayed[Distinct, DistinctImplementation],
            SetDelayed[Contains, ContainsImplementation],
            SetDelayed[Concat, ConcatImplementation],
            SetDelayed[CountItem, CountItemImplementation],
            SetDelayed[Length, LengthImplementation],
            SetDelayed[Filter, FilterImplementation],
            SetDelayed[Map, MapImplementation],
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

        public (ImmutableList<Symbol>, Symbol) Run(Symbol symbol) {
            var variableAssigner = new VariableAssigner();
            var globalVariablesReplacer = new GlobalVariablesReplacer(variableAssigner);

            var fullEvaluator = new FullEvaluator(
                ImmutableList<ISymbolVisitor<Symbol>>.Empty
                    .Add(variableAssigner)
            );

            var context = Seq[DefaultContext, additionalContext].Visit(fullEvaluator).Symbol;
//            symbol = Seq[context, symbol];
            
            var steps = ImmutableList<Symbol>.Empty.Add(symbol);
            var i = 0;

            while (true) {
                var (newSteps, newResult) = symbol
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(globalVariablesReplacer)
                    .Visit(fullEvaluator);

                if (Equals(newResult, symbol) && i > 5) {
                    return (steps, symbol);
                }
                
                steps = steps.AddRange(newSteps).WithoutDuplicates();
                symbol = newResult;

                Logger.Log($"Iteration: {symbol}");
                
                if (i++ > maxIterations) {
                    return (steps, Seq["Max iterations count reached", symbol]);
                }
            }
        }
    }
}