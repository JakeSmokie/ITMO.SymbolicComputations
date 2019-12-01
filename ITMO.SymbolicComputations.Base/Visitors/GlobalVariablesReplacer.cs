using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public class GlobalVariablesReplacer : ISymbolVisitor<Symbol> {
        private readonly VariableAssigner variableAssigner;

        public GlobalVariablesReplacer(VariableAssigner variableAssigner) {
            this.variableAssigner = variableAssigner;
        }

        public Symbol VisitExpression(Expression expression) {
            return variableAssigner.Variables.Aggregate(
                (Symbol) expression,
                (acc, x) => acc.Visit(new VariableReplacer(x.Key, x.Value))
            );
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;

        public Symbol VisitConstant(Constant constant) => constant;
    }
}