using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class HoldTests {
        public HoldTests(ITestOutputHelper output) =>
            _out = output;

        private static readonly StringSymbol HoldFirst = new StringSymbol("", Attributes.HoldFirst);
        private static readonly StringSymbol HoldRest = new StringSymbol("", Attributes.HoldRest);

        private readonly ITestOutputHelper _out;

        [Fact]
        public void HoldFirstSuppressionWorks() {
            Test.EvaluateAndAssert(
                HoldFirst[Evaluate[Plus[2, 4]], Plus[3, 5]],
                HoldFirst[6, 8],
                _out
            );
        }

        [Fact]
        public void HoldFirstWorks() {
            Test.EvaluateAndAssert(
                HoldFirst[Plus[2, 4], Plus[3, 5]],
                HoldFirst[Plus[2, 4], 8],
                _out
            );
        }
        
        [Fact]
        public void HoldIsNotSuppressedWhenItIsComplete() {
            var source = HoldComplete[Evaluate[Plus[2, 6]]];

            Test.EvaluateAndAssert(
                source,
                source,
                _out
            );
        }

        [Fact]
        public void HoldRestSuppressionWorks() {
            Test.EvaluateAndAssert(
                HoldRest[Plus[2, 5], Plus[3, 7], Evaluate[Plus[4, 8]]],
                HoldRest[7, Plus[3, 7], 12],
                _out
            );
        }

        [Fact]
        public void HoldRestWorks() {
            Test.EvaluateAndAssert(
                HoldRest[Plus[2, 1], Plus[3], Plus[4]],
                HoldRest[3, Plus[3], Plus[4]],
                _out
            );
        }

        [Fact]
        public void HoldSuppressingWithEvaluateFunctionWorks() {
            Test.EvaluateAndAssert(
                Hold[Evaluate[Plus[2, 3]]],
                Hold[5],
                _out
            );
        }

        [Fact]
        public void HoldWorks() {
            var source = Hold[Plus[2]];
            Test.EvaluateAndAssert(
                source,
                source,
                _out
            );
        }

        [Fact]
        public void NestedHoldWorks() {
            Symbol dull = "";

            var source = dull[Hold[Plus[2, 4]], 1];
            Test.EvaluateAndAssert(
                source,
                source,
                _out
            );
        }
    }
}