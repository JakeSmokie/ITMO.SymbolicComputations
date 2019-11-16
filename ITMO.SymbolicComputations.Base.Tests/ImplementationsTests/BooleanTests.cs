using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Extensions;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.Boolean;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class BooleanTests {
        public BooleanTests(ITestOutputHelper output) =>
            _out = output;

        private void EvaluateAndAssert(Expression expression, Symbol expectedResult) {
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(expectedResult, actual);
        }

        private readonly ITestOutputHelper _out;

        [Fact]
        public void CompareEqWorks() {
            EvaluateAndAssert(Compare[1, 1], 0);
        }

        [Fact]
        public void CompareLessWorks() {
            EvaluateAndAssert(Compare[1, 2], -1);
        }

        [Fact]
        public void CompareMoreWorks() {
            EvaluateAndAssert(Compare[3, 2], 1);
        }

        [Fact]
        public void FalseWorks() {
            EvaluateAndAssert(Eq[3, 5], False);
        }

        [Fact]
        public void LessWorks() {
            EvaluateAndAssert(Less[1][2], True);
        }

        [Fact]
        public void MoreWorks() {
            EvaluateAndAssert(More[2][1], True);
        }

        [Fact]
        public void ListEqualityWorks() {
            EvaluateAndAssert(Eq[List[1, 2, List[4]], List[1, 2, List[4]]], True);
        }

        [Fact]
        public void NotLessWorks() {
            EvaluateAndAssert(Less[2][1], False);
        }

        [Fact]
        public void NotWorks() {
            EvaluateAndAssert(Not[Eq[1, 1]], False);
        }

        [Fact]
        public void TrueWorks() {
            EvaluateAndAssert(Eq[3, 3], True);
        }
    }
}