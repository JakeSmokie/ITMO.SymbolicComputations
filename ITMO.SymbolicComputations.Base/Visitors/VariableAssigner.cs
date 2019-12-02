using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Implementations;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors {
    public class VariableAssigner : AbstractFunctionImplementation {
        public VariableAssigner() : base(SetDelayed, Set) {
        }

        public ImmutableDictionary<Symbol, Symbol> Variables { get; private set; } = 
            ImmutableDictionary<Symbol, Symbol>.Empty;

        protected override Symbol Evaluate(Expression expression) {
            var variable = expression.Arguments[0];
            var body = expression.Arguments[1];

            Variables = Variables.SetItem(variable, body);
            return expression;
        }
    }
}