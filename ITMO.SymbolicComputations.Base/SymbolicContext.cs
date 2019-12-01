using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base {
    public class SymbolicContext {
        private const int MaxIterations = 10;

        private readonly Symbol additionalContext;
        private readonly ImmutableList<Symbol> topLevelProcessors;

        public SymbolicContext(Symbol additionalContext = null, ImmutableList<Symbol> topLevelProcessors = null) {
            this.topLevelProcessors = topLevelProcessors ?? ImmutableList<Symbol>.Empty;
            this.additionalContext = additionalContext ?? "Null";
        }

        private static Expression DefaultContext => Seq[
            Set[ListPlus, ListPlusImplementation],
            Set[Map, MapImplementation],
            Set[Minus, MinusImplementation],
            Set[Power, PowerImplementation]
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
                    .Visit(fullEvaluator);

                if (Equals(newResult, symbol) && i > 0) {
                    return (steps, symbol);
                }

                steps = steps.AddRange(newSteps).WithoutDuplicates();
                symbol = newResult;

                if (i++ > MaxIterations) {
                    return (steps, Seq["Max iterations count reached", symbol]);
                }
            }
        }
    }
}