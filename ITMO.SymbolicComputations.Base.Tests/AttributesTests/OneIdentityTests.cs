﻿using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class OneIdentityTests {
        public OneIdentityTests(ITestOutputHelper output) =>
            evaluateAndAssert = Test.CreateAsserter(output);

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void OneIdentityWorks() {
            var one = new StringSymbol("OneIdentity", Attributes.OneIdentity);
            evaluateAndAssert(
                one[one[one["x"]]],
                "x"
            );
        }

        [Fact]
        public void TimesOneIdentityWorks() {
            evaluateAndAssert(
                Times[Times[Times[3]], 4],
                12
            );
        }
    }
}