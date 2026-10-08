using ITMO.SymbolicComputations.Base.Models;
using Xunit;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class AssignmentTests {
        [Fact]
        public void SelfReferentialAssignmentReportsIterationLimitInsteadOfAnAnswer() {
            Symbol x = "x";
            var error = Assert.Throws<EvaluationLimitException>(() =>
                new SymbolicContext(maxIterations: 2).Run(Set[x, Plus[x, 2]]));
            Assert.Equal("iterations", error.Limit);
            Assert.Equal(2, error.Maximum);
        }
    }
}
