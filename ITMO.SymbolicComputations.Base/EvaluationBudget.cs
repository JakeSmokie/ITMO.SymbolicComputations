using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base {
    internal sealed class EvaluationBudget : IDisposable {
        private static readonly AsyncLocal<EvaluationBudget?> Active = new AsyncLocal<EvaluationBudget?>();
        private readonly EvaluationBudget? previous;
        private readonly CancellationToken cancellationToken;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private readonly int maxDepth;
        private readonly long maxNodeVisits;
        private readonly long maxSteps;
        private readonly int maxListLength;
        private readonly TimeSpan maxDuration;
        private int depth;
        private long visits;
        private long steps;

        private EvaluationBudget(EvaluationOptions options, CancellationToken cancellationToken) {
            if (options.MaxDepth <= 0 || options.MaxNodeVisits <= 0 || options.MaxSteps <= 0 ||
                options.MaxListLength <= 0 || options.MaxDuration <= TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException(nameof(options), "All evaluation limits must be positive.");
            }

            // Snapshot mutable options so another request cannot change an evaluation in progress.
            maxDepth = options.MaxDepth;
            maxNodeVisits = options.MaxNodeVisits;
            maxSteps = options.MaxSteps;
            maxListLength = options.MaxListLength;
            maxDuration = options.MaxDuration;
            this.cancellationToken = cancellationToken;
            previous = Active.Value;
            Active.Value = this;
        }

        internal static EvaluationBudget? Current => Active.Value;

        internal static EvaluationBudget Begin(EvaluationOptions options, CancellationToken cancellationToken) =>
            new EvaluationBudget(options ?? throw new ArgumentNullException(nameof(options)), cancellationToken);

        internal void Checkpoint() {
            cancellationToken.ThrowIfCancellationRequested();
            if (stopwatch.Elapsed > maxDuration) {
                throw new EvaluationLimitException("time", "Evaluation exceeded its time limit.", (long) maxDuration.TotalMilliseconds);
            }
        }

        internal void EnterVisit() {
            Checkpoint();
            if (++visits > maxNodeVisits) {
                throw new EvaluationLimitException("nodeVisits", "Evaluation exceeded its node visit limit.", maxNodeVisits);
            }
            if (depth >= maxDepth) {
                throw new EvaluationLimitException("depth", "Evaluation exceeded its recursion depth limit.", maxDepth);
            }
            depth++;
        }

        internal void ExitVisit() => depth--;

        internal void RecordStep() {
            Checkpoint();
            if (++steps > maxSteps) {
                throw new EvaluationLimitException("steps", "Evaluation exceeded its intermediate step limit.", maxSteps);
            }
        }

        internal void CheckCollectionSize(int count) {
            Checkpoint();
            if (count > maxListLength) {
                throw new EvaluationLimitException("listLength", "Expression or generated list exceeded its item limit.", maxListLength);
            }
        }

        internal int ValidateGeneratedCount(decimal count, string function) {
            Checkpoint();
            if (count < 0 || decimal.Truncate(count) != count) {
                throw new ArgumentOutOfRangeException(nameof(count), $"{function} requires a non-negative integer item count.");
            }
            if (count > maxListLength) {
                throw new EvaluationLimitException("listLength", $"{function} exceeded its generated item limit.", maxListLength);
            }
            return (int) count;
        }

        // Inspect the initial AST iteratively, before recursive equality, visitors or printing can run.
        internal void ValidateInput(Symbol symbol) {
            var pending = new Stack<(Symbol Symbol, int Depth)>();
            pending.Push((symbol ?? throw new ArgumentNullException(nameof(symbol)), 1));
            while (pending.Count != 0) {
                Checkpoint();
                if (++visits > maxNodeVisits) {
                    throw new EvaluationLimitException("nodeVisits", "Input exceeded its node visit limit.", maxNodeVisits);
                }
                var (current, inputDepth) = pending.Pop();
                if (inputDepth > maxDepth) {
                    throw new EvaluationLimitException("depth", "Input exceeded its expression depth limit.", maxDepth);
                }
                if (current is Expression expression) {
                    CheckCollectionSize(expression.Arguments.Count);
                    pending.Push((expression.Head, inputDepth + 1));
                    foreach (var argument in expression.Arguments) {
                        pending.Push((argument, inputDepth + 1));
                    }
                }
            }
        }

        public void Dispose() {
            Active.Value = previous;
            stopwatch.Stop();
        }
    }
}
