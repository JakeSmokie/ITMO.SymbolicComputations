using System.Collections.Immutable;

namespace ITMO.SymbolicComputations.Base.Models {
    public class Function {
        public string Name { get; }
        public ImmutableList<Expression> Arguments { get; }

        public Function(string name, ImmutableList<Expression> arguments) {
            Name = name;
            Arguments = arguments;
        }
    }
}