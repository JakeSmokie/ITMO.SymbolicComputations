namespace ITMO.SymbolicComputations.Base.Parsing {
    /// <summary>Bounds both parser recursion and the depth/size of the resulting AST.</summary>
    public sealed class FormulaParserLimits {
        public int MaxInputLength { get; set; } = 16_384;
        public int MaxDepth { get; set; } = 64;
        public int MaxNodes { get; set; } = 4_096;
    }
}
