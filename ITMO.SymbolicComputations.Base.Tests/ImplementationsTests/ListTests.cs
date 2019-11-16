using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Extensions;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Boolean;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class ListTests {
        public ListTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void PartWorks() {
            var expression = Part[List[1, 2, 3, 4], 2];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(3, actual);
        }

        [Fact]
        public void FoldWorks() {
            Symbol acc = "acc";
            Symbol x = "x";
            
            var expression = Fold[List[1, 2, 3, 4], 0, Fun[acc, Fun[x, BinaryPlus[acc, x]]]];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(10, actual);
        }

        [Fact]
        public void AppendWorks() {
            var expression = Append[List[1, 2, 3, 4], 5, 6, 0];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(List[1, 2, 3, 4, 5, 6, 0], actual);
        }

        [Fact]
        public void MapWorks() {
            Symbol x = "x";
            
            var expression = Map[List[1, 2, 3, 4]][Fun[x, BinaryPlus[x, 3]]];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(List[4, 5, 6, 7], actual);
        }

        [Fact]
        public void FilterWorks() {
            Symbol x = "x";
            
            var expression = Filter[List[1, 2, 3, 4]][Fun[x, Less[x][3]]];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(List[1, 2], actual);
        }
    }
}