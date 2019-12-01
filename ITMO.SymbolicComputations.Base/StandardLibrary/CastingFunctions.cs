using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.StandardLibrary {
    public static class CastingFunctions {
        public static readonly StringSymbol AsConstant = new StringSymbol(nameof(AsConstant));
        public static readonly StringSymbol AsStringSymbol = new StringSymbol(nameof(AsStringSymbol));
        public static readonly StringSymbol AsExpressionArgs = new StringSymbol(nameof(AsExpressionArgs));

        public static readonly StringSymbol Null = new StringSymbol(nameof(Null));

        public static readonly StringSymbol IsConstant = new StringSymbol(nameof(IsConstant));
        public static readonly StringSymbol IsStringSymbol = new StringSymbol(nameof(IsStringSymbol));
        public static readonly StringSymbol IsExpressionWithName = new StringSymbol(nameof(IsExpressionWithName));
        public static readonly StringSymbol DefaultValue = new StringSymbol(nameof(DefaultValue));

        public static Expression IsConstantImplementation =>
            Fun[x,
                Not[Eq[AsConstant[x], Null]]
            ];

        public static Expression IsStringSymbolImplementation =>
            Fun[x,
                Not[Eq[AsStringSymbol[x], Null]]
            ];

        public static Expression IsExpressionWithNameImplementation =>
            Fun["name'", Fun["expression'", 
                Not[Eq[AsExpressionArgs["name'", "expression'"], Null]]
            ]];

        public static Expression DefaultValueImplementation =>
            Fun[x, Fun[y, 
                If[Eq[x, Null], y, x]
            ]];
    }
}