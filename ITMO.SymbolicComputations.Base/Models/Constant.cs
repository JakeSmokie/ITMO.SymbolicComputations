using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : Symbol {
        public Constant(decimal value) {
            Value = value;
        }

        public decimal Value { get; }
        
        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) => 
            visitor.VisitConstant(this);
    }
}