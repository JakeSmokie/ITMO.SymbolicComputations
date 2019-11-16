using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class VariableReplacer : ISymbolVisitor<Symbol> {
        private readonly Symbol _funcArgument;
        private readonly StringSymbol _variable;

        public VariableReplacer(StringSymbol variable, Symbol funcArgument) {
            _variable = variable;
            _funcArgument = funcArgument;
        }

        public Symbol VisitFunction(Expression expression) {
            var head = expression.Head.Visit(this);
            var arguments = expression.Arguments.Select(x => x.Visit(this));

            head = Equals(head, _variable)
                ? _funcArgument
                : head;

            var newArguments = arguments.Select(x =>
                Equals(x, _variable)
                    ? _funcArgument
                    : x
            ).ToArray();

            return head[newArguments];
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}