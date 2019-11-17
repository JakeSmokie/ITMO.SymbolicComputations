using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

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
                HoldFirst[Evaluate[BinaryPlus[2, 4]], BinaryPlus[3, 5]],
                HoldFirst[6, 8],
                _out
            );
        }

        [Fact]
        public void HoldFirstWorks() {
            Test.EvaluateAndAssert(
                HoldFirst[BinaryPlus[2, 4], BinaryPlus[3, 5]],
                HoldFirst[BinaryPlus[2, 4], 8],
                _out
            );
        }

        [Fact]
        public void HoldFormSuppressingWithEvaluateFunctionWorks() {
            Test.EvaluateAndAssert(
                HoldForm[Evaluate[BinaryPlus[2, 1]]],
                3,
                _out
            );
        }

        [Fact]
        public void HoldFormWorks() {
            var source = BinaryPlus[2, 4];

            Test.EvaluateAndAssert(
                HoldForm[source],
                source,
                _out
            );
        }

        [Fact]
        public void HoldIsNotSuppressedWhenItIsComplete() {
            var source = HoldComplete[Evaluate[BinaryPlus[2, 6]]];

            Test.EvaluateAndAssert(
                source,
                source,
                _out
            );
        }

        [Fact]
        public void HoldRestSuppressionWorks() {
            Test.EvaluateAndAssert(
                HoldRest[BinaryPlus[2, 5], BinaryPlus[3, 7], Evaluate[BinaryPlus[4, 8]]],
                HoldRest[7, BinaryPlus[3, 7], 12],
                _out
            );
        }

        [Fact]
        public void HoldRestWorks() {
            Test.EvaluateAndAssert(
                HoldRest[BinaryPlus[2, 1], BinaryPlus[3], BinaryPlus[4]],
                HoldRest[3, BinaryPlus[3], BinaryPlus[4]],
                _out
            );
        }

        [Fact]
        public void HoldSuppressingWithEvaluateFunctionWorks() {
            Test.EvaluateAndAssert(
                Hold[Evaluate[BinaryPlus[2, 3]]],
                Hold[5],
                _out
            );
        }

        [Fact]
        public void HoldWorks() {
            var source = Hold[BinaryPlus[2]];
            Test.EvaluateAndAssert(
                source,
                source,
                _out
            );
        }

        [Fact]
        public void NestedHoldWorks() {
            Symbol dull = "";

            var source = dull[Hold[BinaryPlus[2, 4]], 1];
            Test.EvaluateAndAssert(
                source,
                source,
                _out
            );
        }
    }
}