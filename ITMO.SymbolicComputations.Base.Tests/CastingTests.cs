using ITMO.SymbolicComputations.Base.Tests.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class CastingTests {
        public CastingTests(ITestOutputHelper output) =>
            _out = output;
        
        private readonly ITestOutputHelper _out;

        [Fact]
        public void CastingWorks() {
            Test.EvaluateAndAssert(AsConstant[3], 3, _out);
            Test.EvaluateAndAssert(AsConstant["Three"], Null, _out);
            Test.EvaluateAndAssert(AsConstant[List[1, 2]], Null, _out);
            
            Test.EvaluateAndAssert(AsStringSymbol["Three"], "Three", _out);
            Test.EvaluateAndAssert(AsStringSymbol[3], Null, _out);
            Test.EvaluateAndAssert(AsStringSymbol[List[1, 2]], Null, _out);
        }
    }
}