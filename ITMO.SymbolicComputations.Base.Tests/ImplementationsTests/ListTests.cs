using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Tools;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

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
                Fold[List[1, 2, 3, 4], 0, Fun[acc, Fun[x, BinaryPlus[acc, x]]]],
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
                Map[List[1, 2, 3, 4]][Fun[x, BinaryPlus[x, 3]]],
                List[4, 5, 6, 7],
                _out
            );
        }

        [Fact]
        public void AppendListWorks() {
            Test.EvaluateAndAssert(
                AppendList[List[1, 2, 3, 4]][List[5, 6]],
                List[1, 2, 3, 4, 5, 6],
                _out
            );
        }

        [Fact]
        public void AppendEmptyListAndEmptyListWorks() {
            Test.EvaluateAndAssert(
                AppendList[EmptyList][EmptyList],
                EmptyList,
                _out
            );
        }
        
        [Fact]
        public void AppendEmptyListAndNonEmptyListWorks() {
            Test.EvaluateAndAssert(
                AppendList[EmptyList][List[1, 2, 3]],
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
    }
}