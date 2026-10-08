using System;
using System.Threading;
using System.Threading.Tasks;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors;
using Xunit;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class EvaluationLimitsTests {
        [Fact]
        public void RecursionInsideOneVisitorPassUsesTheSharedBudget() {
            Symbol x = "x";
            var selfApplication = Fun[x, x[x]];
            var error = Assert.Throws<EvaluationLimitException>(() => new SymbolicContext().Run(
                selfApplication[selfApplication], new EvaluationOptions {MaxDepth = 96}));
            Assert.Equal("depth", error.Limit);
        }

        [Fact]
        public void DeepInputIsRejectedBeforeRecursiveTraversal() {
            Symbol input = 1;
            Symbol f = "f";
            for (var i = 0; i < 2000; i++) {
                input = f[input];
            }
            var error = Assert.Throws<EvaluationLimitException>(() => new SymbolicContext().Run(input));
            Assert.Equal("depth", error.Limit);
        }

        [Fact]
        public void NodeVisitsAreBoundedAcrossVisitors() {
            var error = Assert.Throws<EvaluationLimitException>(() => new SymbolicContext().Run(
                Plus[1, 2], new EvaluationOptions {MaxNodeVisits = 32}));
            Assert.Equal("nodeVisits", error.Limit);
        }

        [Fact]
        public void IntermediateStepsAreBounded() {
            var error = Assert.Throws<EvaluationLimitException>(() => new SymbolicContext().Run(
                Plus[1, 2], new EvaluationOptions {MaxSteps = 1}));
            Assert.Equal("steps", error.Limit);
        }

        [Fact]
        public void HugeGeneratedListsAreRejectedBeforeMaterialization() {
            var options = new EvaluationOptions {MaxListLength = 256};
            var context = new SymbolicContext();
            var rangeError = Assert.Throws<EvaluationLimitException>(() => context.Run(StandardLibrary.ListFunctions.Range[0, 1, decimal.MaxValue], options));
            var listError = Assert.Throws<EvaluationLimitException>(() => context.Run(GenerateList[decimal.MaxValue], options));
            Assert.Equal("listLength", rangeError.Limit);
            Assert.Equal("listLength", listError.Limit);
        }

        [Fact]
        public void CancellationIsObservedBeforeEvaluation() {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            Assert.Throws<OperationCanceledException>(() => new SymbolicContext().Run(
                Plus[1, 2], new EvaluationOptions(), cancellation.Token));
        }

        [Fact]
        public void CancellationDuringVisitorTraversalStopsTheSameRun() {
            using var cancellation = new CancellationTokenSource();
            var input = new CancelOnVisit(cancellation);
            Assert.Throws<OperationCanceledException>(() => new SymbolicContext().Run(
                input, new EvaluationOptions(), cancellation.Token));
            Assert.True(input.WasVisited);
        }

        [Fact]
        public void ElapsedTimeIsBounded() {
            var error = Assert.Throws<EvaluationLimitException>(() => new SymbolicContext().Run(
                Plus[1, 2], new EvaluationOptions {MaxDuration = TimeSpan.FromTicks(1)}));
            Assert.Equal("time", error.Limit);
        }

        [Fact]
        public async Task ConcurrentRunsHaveIndependentBudgetsAndFailureDoesNotLeak() {
            var failing = Task.Run(() => Assert.Throws<EvaluationLimitException>(() => new SymbolicContext().Run(
                Plus[1, 2], new EvaluationOptions {MaxSteps = 1})));
            var successful = Task.Run(() => new SymbolicContext().Run(Plus[1, 2]).Item2);
            await failing;
            Assert.Equal((Symbol) 3, await successful);
            Assert.Equal((Symbol) 7, new SymbolicContext().Run(Plus[3, 4]).Item2);
        }

        private sealed class CancelOnVisit : Symbol {
            private readonly CancellationTokenSource cancellation;
            internal bool WasVisited { get; private set; }

            internal CancelOnVisit(CancellationTokenSource cancellation) => this.cancellation = cancellation;

            protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) {
                WasVisited = true;
                cancellation.Cancel();
                return new Constant(1).Visit(visitor);
            }
        }
    }
}
