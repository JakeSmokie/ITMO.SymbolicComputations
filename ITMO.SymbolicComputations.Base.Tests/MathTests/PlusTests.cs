using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests {
    public sealed class PlusTests {
        public PlusTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void BinaryPlusWorks() {
            Test.EvaluateAndAssert(
                Plus[1, 2],
                3,
                _out
            );
        }

        [Fact]
        public void ListPlusWorks() {
            Test.EvaluateAndAssert(
                ListPlus[List[1, 3, 5, 7, -7]],
                9,
                _out
            );
        }
    }
}