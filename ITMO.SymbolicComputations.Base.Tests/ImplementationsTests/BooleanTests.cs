using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class BooleanTests {
        public BooleanTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void CompareWorks() {
            evaluateAndAssert(Compare[1, 1], 0);
            evaluateAndAssert(Compare[1, 2], -1);
            evaluateAndAssert(Compare[3, 2], 1);
        }

        [Fact]
        public void EqWorks() {
            evaluateAndAssert(Eq[3, 3], True);
            evaluateAndAssert(Eq[3, 5], False);

            evaluateAndAssert(Eq[List[1, 2, List[4]], List[1, 2, List[4]]], True);
            evaluateAndAssert(Eq[List[1, 2, List[4]], List[1, 2, List[6]]], False);
        }

        [Fact]
        public void InequalityWorks() {
            evaluateAndAssert(Less[1][2], True);
            evaluateAndAssert(More[2][1], True);

            evaluateAndAssert(Less[2][1], False);
            evaluateAndAssert(More[1][2], False);
        }

        [Fact]
        public void NotWorks() {
            evaluateAndAssert(Not[Eq[1, 1]], False);
        }

        [Fact]
        public void AndWorks() {
            evaluateAndAssert(And[True][True], True);
            evaluateAndAssert(And[True][False], False);
            evaluateAndAssert(And[False][True], False);
            evaluateAndAssert(And[False][False], False);
        }

        [Fact]
        public void OrWorks() {
            evaluateAndAssert(Or[True][True], True);
            evaluateAndAssert(Or[True][False], True);
            evaluateAndAssert(Or[False][True], True);
            evaluateAndAssert(Or[False][False], False);
        }

//        [Fact]
//        public void WhileWorks() {
//            _evaluateAndAssert(
//                While.Value[10][Fun[x, Eq[x, 0]]][Fun[x,
//                    BinaryPlus[x, -1]
//                ]],
//                0
//            );
//        }
    }
}