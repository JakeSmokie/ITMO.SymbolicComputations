using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class FlatTests {
        public FlatTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void FlatWorks() {
            var flat = new StringSymbol("FlatTest", Attributes.Flat);

            evaluateAndAssert(
                flat[flat[1], flat[2, flat[3, 4, flat[5]], 6], 7, 8],
                flat[1, 2, 3, 4, 5, 6, 7, 8]
            );
        }

        [Fact]
        public void TimesFlatWorks() {
            evaluateAndAssert(
                Times[1, 3, 4, Times[5, Times[x, 4], 10]],
                Times[x, 1, 3, 4, 4, 5, 10]
            );
        }
    }
}