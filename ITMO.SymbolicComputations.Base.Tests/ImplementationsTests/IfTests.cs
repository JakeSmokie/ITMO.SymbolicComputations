using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Tools;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class IfTests {
        public IfTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void TrueIsTrue() {
            Test.EvaluateAndAssert(
                If[True, 2, 3],
                2,
                _out
            );
        }

        [Fact]
        public void FalseIsFalse() {
            Test.EvaluateAndAssert(
                If[False, 2, 3],
                3,
                _out
            );
        }

        [Fact]
        public void BottomIsBottom() {
            Test.EvaluateAndAssert(
                If[If, 2, 3, 4],
                4,
                _out
            );
        }

        [Fact]
        public void Holds() {
            Test.EvaluateAndAssert(
                If[True, If[True, 1]],
                If[True, 1],
                _out
            );
        }

        [Fact]
        public void HoldsAndEvaluates() {
            Test.EvaluateAndAssert(
                If[False, 3, Evaluate[If[True, 1]]],
                1,
                _out
            );
        }
    }
}