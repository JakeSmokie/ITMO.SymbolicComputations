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
            Symbol prev;
            Symbol @new = expression;
            
            do {
                prev = @new;
                return variableAssigner.Variables.Aggregate(
                    @new,
                    (acc, x) => acc.Visit(new VariableReplacer(x.Key, x.Value))
                );
            } while (!Equals(@new, prev));

            return @new;
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;

        public Symbol VisitConstant(Constant constant) => constant;
    }
}