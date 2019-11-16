using ITMO.SymbolicComputations.Base.Tests.Extensions;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.Boolean;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class IfTests {
        public IfTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void TrueIsTrue() {
            var expression = If[True, 2, 3];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(2, actual);
        }

        [Fact]
        public void FalseIsFalse() {
            var expression = If[False, 2, 3];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(3, actual);
        }

        [Fact]
        public void BottomIsBottom() {
            var expression = If[If, 2, 3, 4];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(4, actual);
        }

        [Fact]
        public void Holds() {
            var expression = If[True, If[True, 1]];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(If[True, 1], actual);
        }

        [Fact]
        public void HoldsAndEvaluates() {
            var expression = If[False, 3, Evaluate[If[True, 1]]];
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(1, actual);
        }
    }
}