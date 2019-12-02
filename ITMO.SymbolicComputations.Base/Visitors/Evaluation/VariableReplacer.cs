using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Attributes;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Attributes;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class VariableReplacer : ISymbolVisitor<Symbol> {
        private static readonly HasAttributeChecker HoldAllChecker = new HasAttributeChecker(HoldAll);
        private static readonly HasAttributeChecker HoldRestChecker = new HasAttributeChecker(HoldRest);
        private static readonly HasAttributeChecker HoldFirstChecker = new HasAttributeChecker(HoldFirst);

        private readonly Symbol funcArgument;
        private readonly Symbol variable;
        private readonly bool eager;

        public VariableReplacer(Symbol variable, Symbol funcArgument, bool eager = false) {
            this.variable = variable;
            this.funcArgument = funcArgument;
            this.eager = eager;
        }

        public Symbol VisitExpression(Expression expression) {
            var head = expression.Head.Visit(this);
            
            if (Equals(head, Fun) && Equals(expression.Arguments[0], variable)) {
                return expression;
            }

            if (!eager) {
                if (head.Visit(HoldAllChecker)) {
                    return head[expression.Arguments.ToArray()];
                }

                if (head.Visit(HoldFirstChecker)) {
                    return head[
                        expression.Arguments
                            .Select((x, i) => i != 0 ? x.Visit(this) : x)
                            .ToArray()
                    ];
                }

                if (head.Visit(HoldRestChecker)) {
                    return head[
                        expression.Arguments
                            .Select((x, i) => i == 0 ? x.Visit(this) : x)
                            .ToArray()
                    ];
                }
            }
            
            var arguments = expression.Arguments
                .Select(x => x.Visit(this))
                .ToArray();

            return head[arguments];
        }

        public Symbol VisitSymbol(StringSymbol symbol) => Equals(symbol, variable) ? funcArgument : symbol;
        public Symbol VisitConstant(Constant constant) => constant;
    }
}