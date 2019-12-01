using System;
using System.Diagnostics;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations {
    public class ApplyListImplementation : AbstractFunctionImplementation{
        public ApplyListImplementation() : base(ApplyList) {
        }

        protected override Symbol Evaluate(Expression expression) {
            var func = expression.Arguments[0];
            var list = expression.Arguments[1].Visit(AsExpressionVisitor.Instance);

            if (!Equals(list?.Head, List)) {
                return expression;
            }

            return func[list?.Arguments?.ToArray()];
        }
    }
}