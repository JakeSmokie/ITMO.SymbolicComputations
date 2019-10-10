using System;
using System.Collections.Immutable;
using System.Linq;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Function : BaseSymbol {
        public string Name { get; }
        public ImmutableList<BaseSymbol> Arguments { get; }

        public Function(string name, ImmutableList<BaseSymbol> arguments) {
            Name = name;
            Arguments = arguments;
        }

        public Function MapArguments(Func<BaseSymbol, BaseSymbol> selector) {
            return new Function(Name, Arguments.Select(selector).ToImmutableList());
        }
    }
}