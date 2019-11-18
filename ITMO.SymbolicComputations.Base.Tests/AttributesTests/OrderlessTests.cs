using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class OrderlessTests {
        public OrderlessTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        private static readonly StringSymbol Orderless = new StringSymbol("Orderless", Attributes.Orderless);

        [Fact]
        public void ConstantOrderingWorks() {
            _evaluateAndAssert(
                Orderless["y", 30, "x", 10, "z", 60],
                Orderless["x", "y", "z", 10, 30, 60]
            );
        }

        [Fact]
        public void NestedOrderingWorks() {
            _evaluateAndAssert(
                Orderless["y", Orderless[1, "x", Orderless["y"]], "x", "z"],
                Orderless[Orderless[Orderless["y"], "x", 1], "x", "y", "z"]
            );
        }

        [Fact]
        public void StringSymbolsOrderingWorks() {
            _evaluateAndAssert(
                Orderless["y", "x", "z"],
                Orderless["x", "y", "z"]
            );
        }
    }
}