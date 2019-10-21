using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class GroupByExtensions {
        public static ImmutableList<(T Item, int Count)> GroupWithCounting<T>(this IEnumerable<T> enumerable) {
            return enumerable.Aggregate(
                ImmutableList<(T Item, int Count)>.Empty,
                (list, t) => {
                    var i = list.FindIndex(x => Equals(x.Item1, t));

                    return i != -1
                        ? list.SetItem(i, (list[i].Item, list[i].Count + 1))
                        : list.Add((t, 1));
                }
            );
        }
    }
}