using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.Functions.Alphabet;
using static ITMO.SymbolicComputations.Base.Functions.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Functions.Functions;

namespace ITMO.SymbolicComputations.Base.Functions {
    public static class CastingFunctions {
        public static readonly StringSymbol AsConstant = new StringSymbol(nameof(AsConstant));
        public static readonly StringSymbol AsStringSymbol = new StringSymbol(nameof(AsStringSymbol));
        public static readonly StringSymbol AsExpressionArgs = new StringSymbol(nameof(AsExpressionArgs));

        public static readonly StringSymbol Null = new StringSymbol(nameof(Null));

        public static readonly Expression IsConstant =
            Fun[x,
                Not[Eq[AsConstant[x], Null]]
            ];

        public static readonly Expression IsStringSymbol =
            Fun[x,
                Not[Eq[AsStringSymbol[x], Null]]
            ];

        public static readonly Expression IsExpressionWithName =
            Fun["name'", Fun["expression'", 
                Not[Eq[AsExpressionArgs["name'", "expression'"], Null]]
            ]];
    }
}