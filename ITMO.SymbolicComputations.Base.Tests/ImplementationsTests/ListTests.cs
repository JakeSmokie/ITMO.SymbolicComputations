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
        public void AppendListWorks() {
            Test.EvaluateAndAssert(
                Concat[List[1, 2, 3, 4]][List[5, 6]],
                List[1, 2, 3, 4, 5, 6],
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
        public void ApplyListWorks() {
            Test.EvaluateAndAssert(
                ApplyList[Plus, List[x, y, 12]],
                Plus[x, y, 12],
                _out
            );
        }

        [Fact]
        public void ContainsWorks() {
            Test.EvaluateAndAssert(
                Contains[List[1, 2, 3, 4]][3],
                True,
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

            Test.EvaluateAndAssert(
                CountItem[List["x", "x"]]["x"],
                2,
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

            Test.EvaluateAndAssert(
                Distinct[List["x", "a", "x", 1]],
                List["x", "a", 1],
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
        public void FoldWorks() {
            Test.EvaluateAndAssert(
                Fold[List[1, 2, 3, 4]][0][
                    Fun[acc, Fun[x, Plus[acc, x]]]
                ],
                10,
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
        public void MapWorks() {
            Test.EvaluateAndAssert(
                Map[List[1, 2, 3, 4]][Fun[x, Seq[Plus[x, 3]]]],
                List[4, 5, 6, 7],
                _out
            );
        }

        [Fact]
        public void PartWorks() {
            Test.EvaluateAndAssert(
                Part[List[1, 2, 3, 4], 2],
                3,
                _out
            );
        }

        [Fact]
        public void RangeWorks() {
            Test.EvaluateAndAssert(
                Range[0, 10, 40],
                List[
                    0.00m,
                    0.25m,
                    0.50m,
                    0.75m,
                    1.00m,
                    1.25m,
                    1.50m,
                    1.75m,
                    2.00m,
                    2.25m,
                    2.50m,
                    2.75m,
                    3.00m,
                    3.25m,
                    3.50m,
                    3.75m,
                    4.00m,
                    4.25m,
                    4.50m,
                    4.75m,
                    5.00m,
                    5.25m,
                    5.50m,
                    5.75m,
                    6.00m,
                    6.25m,
                    6.50m,
                    6.75m,
                    7.00m,
                    7.25m,
                    7.50m,
                    7.75m,
                    8.00m,
                    8.25m,
                    8.50m,
                    8.75m,
                    9.00m,
                    9.25m,
                    9.50m,
                    9.75m
                ],
                _out
            );
        }
    }
}