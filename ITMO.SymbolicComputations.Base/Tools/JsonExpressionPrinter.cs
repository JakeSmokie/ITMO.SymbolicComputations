using ITMO.SymbolicComputations.Base.Models;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class JsonExpressionPrinter {
        public static string AsJson(this ExpressionInfo expressionInfo) {
            return JsonConvert.SerializeObject(expressionInfo, Formatting.Indented);
        }
    }
}