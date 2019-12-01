using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Casting {
    public sealed class AsExpressionVisitor : ISymbolVisitor<Expression>  {
        public static readonly AsExpressionVisitor Instance = new AsExpressionVisitor();
        public Expression VisitExpression(Expression expression) => expression;

        public Expression VisitSymbol(StringSymbol symbol) => null;

        public Expression VisitConstant(Constant constant) => null;
    }
}