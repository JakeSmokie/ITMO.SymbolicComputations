using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.ListFunctions {
    public class GroupImplementation : AbstractListFunctionImplementation{
        public GroupImplementation() : base(Group) {
        }

        protected override Symbol EvaluateList(Expression expression, ImmutableList<Symbol> items) {
            var groups = items
                .GroupWithCounting()
                .Select(x => (Symbol) List[x.Item, x.Count])
                .ToArray();

            return List[groups];
        }
    }
}