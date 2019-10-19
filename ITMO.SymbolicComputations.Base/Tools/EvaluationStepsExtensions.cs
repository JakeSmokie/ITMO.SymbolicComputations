using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class EvaluationStepsExtensions {
        public static ImmutableList<Symbol> WithoutDuplicates(this ImmutableList<Symbol> steps) {
            return steps.Count == 1
                ? steps
                : steps.Skip(1).Aggregate(
                    (Steps: ImmutableList<Symbol>.Empty.Add(steps.First()), steps.First()),
                    (tuple, symbol) => {
                        var (steps, last) = tuple;
                        return (symbol.Equals(last) ? steps : steps.Add(symbol), symbol);
                    }
                ).Steps;
        }
    }
}