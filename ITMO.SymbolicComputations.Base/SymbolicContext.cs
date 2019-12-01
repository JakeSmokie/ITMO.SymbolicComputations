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
        private const int MaxIterations = 10;

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
            Set[IsConstant, IsConstantImplementation],
            Set[IsStringSymbol, IsStringSymbolImplementation],
            Set[IsExpressionWithName, IsExpressionWithNameImplementation],
            Set[DefaultValue, DefaultValueImplementation],
            //
            Set[Group, GroupImplementation],
            Set[Distinct, DistinctImplementation],
            Set[Contains, ContainsImplementation],
            Set[Concat, ConcatImplementation],
            Set[CountItem, CountItemImplementation],
            Set[Length, LengthImplementation],
            Set[Filter, FilterImplementation],
            Set[Map, MapImplementation],
            //
            Set[Factorial, FactorialImplementation],
            Set[TaylorSin, TaylorSinImplementation],
            Set[ListTimes, ListTimesImplementation],
            Set[ListPlus, ListPlusImplementation],
            //
            Set[Minus, MinusImplementation],
            Set[Or, OrImplementation],
            Set[And, AndImplementation],
            Set[More, MoreImplementation],
            Set[Less, LessImplementation],
            Set[Not, NotImplementation]
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

                if (Equals(newResult, symbol) && i > 0) {
                    return (steps, symbol);
                }
                
                steps = steps.AddRange(newSteps).WithoutDuplicates();
                symbol = newResult;


                if (i++ > maxIterations) {
                    return (steps, Seq["Max iterations count reached", symbol]);
                }
            }
        }
    }
}