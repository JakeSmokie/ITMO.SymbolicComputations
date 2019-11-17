using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using Xunit.Abstractions;

namespace Tests.Base.Tools {
    public static class StepsPrintingExtensions {
        private static readonly MathematicaPrinter MathematicaPrinter = new MathematicaPrinter();

        public static void Print(this ImmutableList<Symbol> steps, ITestOutputHelper output) {
            steps.WithoutDuplicates()
                .ForEach(e => output.WriteLine(e.Visit(MathematicaPrinter)));
        }
    }
}