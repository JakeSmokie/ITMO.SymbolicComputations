using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public sealed class Evaluator : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker HoldAllChecker =
            new HasAttributeChecker(Predefined.Attributes.HoldAll);

        private static readonly OneIdentityShrinker OneIdentityShrinker =
            new OneIdentityShrinker();

        public Symbol VisitFunction(Function function) {
            if (function.Head.Visit(HoldAllChecker)) {
                return function.Head == Functions.HoldForm 
                    ? function.Arguments[0] 
                    : function;
            }

            return function
                .Visit(OneIdentityShrinker);
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}