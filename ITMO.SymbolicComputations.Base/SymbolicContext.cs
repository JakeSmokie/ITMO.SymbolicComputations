using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base {
    public class SymbolicContext {
        private const int MaxIterations = 10;

        private static readonly Expression DefaultContext = Seq[
            Set[Minus, MinusImplementation],
            Set[ListPlus, ListPlusImplementation],
            Set[Power, PowerImplementation]
        ];

        public (ImmutableList<Symbol>, Symbol) Run(Symbol symbol) {
            var variableAssigner = new VariableAssigner();
            var globalVariablesReplacer = new GlobalVariablesReplacer(variableAssigner);

            var fullEvaluator = new FullEvaluator(
                ImmutableList<ISymbolVisitor<Symbol>>.Empty
                    .Add(variableAssigner)
            );

            var context = DefaultContext.Visit(fullEvaluator).Symbol;
//            symbol = Seq[context, symbol];
            
            var steps = ImmutableList<Symbol>.Empty.Add(symbol);
            var i = 0;

            while (true) {
                symbol = symbol.Visit(globalVariablesReplacer);
                var (newSteps, newResult) = symbol.Visit(fullEvaluator);

                if (Equals(newResult, symbol)) {
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