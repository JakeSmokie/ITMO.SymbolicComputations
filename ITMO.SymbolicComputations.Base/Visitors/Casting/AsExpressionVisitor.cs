using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Casting {
    public sealed class AsExpressionVisitor : ISymbolVisitor<Expression>  {
        public Expression VisitFunction(Expression expression) => expression;

        public Expression VisitSymbol(StringSymbol symbol) => null;

        public Expression VisitConstant(Constant constant) => null;
        
        public static readonly AsExpressionVisitor Instance = new AsExpressionVisitor();
    }
}