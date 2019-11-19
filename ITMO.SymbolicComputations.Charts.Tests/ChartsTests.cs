using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Charts.Tests {
    public class ChartsTests {
        public ChartsTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;
        

        [Fact]
        public void SinFunctionIsOkay() {
            Symbol x = "x";

            var func = Fun[x, Sin[x]];
            var xs = List[
                Enumerable.Range(0, 30)
                    .Select(x => x / 10m)
                    .Select(x => new Constant(x))
                    .OfType<Symbol>()
                    .ToArray()
            ];
            var expr = Map[xs][func];
            
            var (steps, actual) = expr.Visit(FullEvaluator.Default);
            steps.Print(_out);
        }
    }
}