using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public class FunctionPreprocessor : ISymbolVisitor<Symbol> {
        private readonly FullEvaluator _fullEvaluator;
        private readonly Expression _function;

        public FunctionPreprocessor(FullEvaluator fullEvaluator, Expression function) {
            _fullEvaluator = fullEvaluator;
            _function = function;
        }

        public Symbol VisitExpression(Expression expression) {
            return _function[expression].Visit(_fullEvaluator).Symbol;
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}