using System;

namespace ITMO.SymbolicComputations.Base.Parsing {
    /// <summary>A formula error whose Position is a zero-based UTF-16 character offset.</summary>
    public sealed class FormulaParseException : FormatException {
        public FormulaParseException(string message, int position)
            : base($"{message} (позиция {position + 1}).") {
            Position = position;
        }

        public int Position { get; }
    }
}
