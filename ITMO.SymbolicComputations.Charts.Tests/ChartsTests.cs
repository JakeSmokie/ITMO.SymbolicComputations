using System.Linq;
using ITMO.SymbolicComputations.Base;
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

            var xs = Range[0][7][100]; 
            var func = Fun[x, List[x, Sin[x]]];
            
            var expr = Map[xs][func];
            
            var (steps, actual) = new SymbolicContext().Run(expr);
            var points = actual.Visit(new ListOfListToDecimalTuples()).ToList();
            
            _out.WriteLine(actual + "\n\n");
            points.ForEach(x => _out.WriteLine($"{x.Item1}, {x.Item2}"));
        }
        
        [Fact]
        public void TaylorSinIsOkay() {
            Symbol x = "x";

            var xs = Range[0][7][100]; 
            var func = Fun[x, List[x, TaylorSin[x]]];
            
            var expr = Map[xs][func];
            
            var (steps, actual) = new SymbolicContext(Seq[Set[TaylorSin, TaylorSinImplementation]]).Run(expr);
            var points = actual.Visit(new ListOfListToDecimalTuples()).ToList();
            
            _out.WriteLine(actual + "\n\n");
            points.ForEach(x => _out.WriteLine($"{x.Item1}, {x.Item2}"));
        }
    }
}