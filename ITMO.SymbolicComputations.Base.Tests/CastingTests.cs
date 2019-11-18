using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Functions.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Functions.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;

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
        
        [Fact]
        public void TypeTestingWorks() {
            Test.EvaluateAndAssert(IsConstant[3], True, _out);
            Test.EvaluateAndAssert(IsConstant["Three"], False, _out);
            Test.EvaluateAndAssert(IsConstant[List[1, 2]], False, _out);
            
            Test.EvaluateAndAssert(IsStringSymbol["Three"], True, _out);
            Test.EvaluateAndAssert(IsStringSymbol[3], False, _out);
            Test.EvaluateAndAssert(IsStringSymbol[List[1, 2]], False, _out);
        }
    }
}