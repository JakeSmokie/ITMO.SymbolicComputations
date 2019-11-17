using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class BooleanTests {
        public BooleanTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.EvaluateAndAssert(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void CompareWorks() {
            _evaluateAndAssert(Compare[1, 1], 0);
            _evaluateAndAssert(Compare[1, 2], -1);
            _evaluateAndAssert(Compare[3, 2], 1);
        }

        [Fact]
        public void EqWorks() {
            _evaluateAndAssert(Eq[3, 3], True);
            _evaluateAndAssert(Eq[3, 5], False);
            
            _evaluateAndAssert(Eq[List[1, 2, List[4]], List[1, 2, List[4]]], True);
            _evaluateAndAssert(Eq[List[1, 2, List[4]], List[1, 2, List[6]]], False);
        }

        [Fact]
        public void InequalityWorks() {
            _evaluateAndAssert(Less[1][2], True);
            _evaluateAndAssert(More[2][1], True);
            
            _evaluateAndAssert(Less[2][1], False);
            _evaluateAndAssert(More[1][2], False);
        }

        [Fact]
        public void NotWorks() {
            _evaluateAndAssert(Not[Eq[1, 1]], False);
        }

        [Fact]
        public void AndWorks() {
            _evaluateAndAssert(And[True][True], True);
            _evaluateAndAssert(And[True][False], False);
            _evaluateAndAssert(And[False][True], False);
            _evaluateAndAssert(And[False][False], False);
        }

        [Fact]
        public void OrWorks() {
            _evaluateAndAssert(Or[True][True], True);
            _evaluateAndAssert(Or[True][False], True);
            _evaluateAndAssert(Or[False][True], True);
            _evaluateAndAssert(Or[False][False], False);
        }
    }
}