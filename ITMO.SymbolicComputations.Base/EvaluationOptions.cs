using System;

namespace ITMO.SymbolicComputations.Base {
    /// <summary>Resource limits shared by every visitor participating in one evaluation.</summary>
    public sealed class EvaluationOptions {
        public int MaxDepth { get; set; } = 256;
        public long MaxNodeVisits { get; set; } = 10_000_000;
        public long MaxSteps { get; set; } = 250_000;
        public int MaxListLength { get; set; } = 10_000;
        public TimeSpan MaxDuration { get; set; } = TimeSpan.FromSeconds(10);
    }

    public sealed class EvaluationLimitException : Exception {
        public EvaluationLimitException(string limit, string message, long? maximum = null) : base(message) {
            Limit = limit;
            Maximum = maximum;
        }

        public string Limit { get; }
        public long? Maximum { get; }
    }
}
