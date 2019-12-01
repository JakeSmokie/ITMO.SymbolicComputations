using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public class FunctionPreprocessor : ISymbolVisitor<Symbol> {
        private readonly FullEvaluator fullEvaluator;
        private readonly Expression function;

        public FunctionPreprocessor(FullEvaluator fullEvaluator, Expression function) {
            this.fullEvaluator = fullEvaluator;
            this.function = function;
        }

        public Symbol VisitExpression(Expression expression) {
            return function[expression].Visit(fullEvaluator).Symbol;
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}