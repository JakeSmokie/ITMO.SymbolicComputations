using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.ImplementationsTests {
    public class ListTests {
        public ListTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void PartWorks() {
            Test.EvaluateAndAssert(
                Part[List[1, 2, 3, 4], 2],
                3,
                _out
            );
        }

        [Fact]
        public void FoldWorks() {
            Test.EvaluateAndAssert(
                Fold[List[1, 2, 3, 4], 0, Fun[acc, Fun[x, Plus[acc, x]]]],
                10,
                _out
            );
        }

        [Fact]
        public void AppendWorks() {
            Test.EvaluateAndAssert(
                Append[List[1, 2, 3, 4], 5, 6, 0],
                List[1, 2, 3, 4, 5, 6, 0],
                _out
            );
        }

        [Fact]
        public void MapWorks() {
            Test.EvaluateAndAssert(
                Map[List[1, 2, 3, 4]][Fun[x, Plus[x, 3]]],
                List[4, 5, 6, 7],
                _out
            );
        }

        [Fact]
        public void AppendListWorks() {
            Test.EvaluateAndAssert(
                Concat[List[1, 2, 3, 4]][List[5, 6]],
                List[1, 2, 3, 4, 5, 6],
                _out
            );
        }

        [Fact]
        public void AppendEmptyListAndEmptyListWorks() {
            Test.EvaluateAndAssert(
                Concat[EmptyList][EmptyList],
                EmptyList,
                _out
            );
        }

        [Fact]
        public void AppendEmptyListAndNonEmptyListWorks() {
            Test.EvaluateAndAssert(
                Concat[EmptyList][List[1, 2, 3]],
                List[1, 2, 3],
                _out
            );
        }

        [Fact]
        public void FilterWorks() {
            Test.EvaluateAndAssert(
                Filter[List[1, 2, 3, 4]][Fun[x, Less[x][3]]],
                List[1, 2],
                _out
            );
        }

        [Fact]
        public void LengthWorksOnEmptyList() {
            Test.EvaluateAndAssert(
                Length[EmptyList],
                0,
                _out
            );
        }

        [Fact]
        public void LengthWorksOnNonEmptyList() {
            Test.EvaluateAndAssert(
                Length[List[1, 2, 3, 4]],
                4,
                _out
            );
        }

        [Fact]
        public void CountItemWorks() {
            Test.EvaluateAndAssert(
                CountItem[List[1, 2, 3, 4, 4, 5, 4]][4],
                3,
                _out
            );
        }

        [Fact]
        public void ContainsWorks() {
            Test.EvaluateAndAssert(
                Contains[List[1, 2, 3, 4]][4],
                True,
                _out
            );
        }

        [Fact]
        public void DistinctWorks() {
            Test.EvaluateAndAssert(
                Distinct[List[1, 2, 3, 4, 2, 3, 4, 4, 4, 1]],
                List[1, 2, 3, 4],
                _out
            );
        }

        [Fact]
        public void GroupWorks() {
            Test.EvaluateAndAssert(
                Group[List[1, 2, 3, 4, 2, 3, 4, 4, 4, 1]],
                List[List[1, 2], List[2, 2], List[3, 2], List[4, 4]],
                _out
            );
        }

        [Fact]
        public void ApplyListWorks() {
            Test.EvaluateAndAssert(
                ApplyList[Plus, List[x, y, 12]],
                Plus[x, y, 12],
                _out
            );
        }
    }
}