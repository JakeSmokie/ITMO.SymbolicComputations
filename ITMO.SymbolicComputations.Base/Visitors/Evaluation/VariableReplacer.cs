using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class VariableReplacer : ISymbolVisitor<Symbol> {
        private readonly Symbol funcArgument;
        private readonly Symbol variable;

        public VariableReplacer(Symbol variable, Symbol funcArgument) {
            this.variable = variable;
            this.funcArgument = funcArgument;
        }

        public Symbol VisitExpression(Expression expression) {
            var head = expression.Head.Visit(this);
            var arguments = expression.Arguments.Select(x => x.Visit(this));

            if (Equals(head, Fun) && Equals(expression.Arguments[0], variable)) {
                return expression;
            }

            head = Equals(head, variable)
                ? funcArgument
                : head;

            var newArguments = arguments.Select((x, i) =>
                (!Equals(head, Set) || i != 0)
                && Equals(x, variable)
                    ? funcArgument
                    : x
            ).ToArray();

            return head[newArguments];
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}