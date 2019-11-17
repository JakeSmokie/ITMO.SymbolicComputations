using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Tests.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class OneIdentityTests {
        public OneIdentityTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void OneIdentityWorks() {
            var one = new StringSymbol("OneIdentity", Attributes.OneIdentity);
            Test.EvaluateAndAssert(
                one[one[one["x"]]],
                "x",
                _out
            );
        }
    }
}