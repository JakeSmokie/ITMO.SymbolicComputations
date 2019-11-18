using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Functions.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Functions.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.Functions.CastingFunctions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class CastingTests {
        public CastingTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void CastingWorks() {
            _evaluateAndAssert(AsConstant[3], 3);
            _evaluateAndAssert(AsConstant["Three"], Null);
            _evaluateAndAssert(AsConstant[List[1, 2]], Null);

            _evaluateAndAssert(AsStringSymbol["Three"], "Three");
            _evaluateAndAssert(AsStringSymbol[3], Null);
            _evaluateAndAssert(AsStringSymbol[List[1, 2]], Null);

            _evaluateAndAssert(AsExpressionArgs[Times, "Three"], Null);
            _evaluateAndAssert(AsExpressionArgs[Times, 3], Null);
            _evaluateAndAssert(
                AsExpressionArgs[Times, Times["x", "y"]],
                List["x", "y"]
            );
        }

        [Fact]
        public void TypeTestingWorks() {
            _evaluateAndAssert(IsConstant[3], True);
            _evaluateAndAssert(IsConstant["Three"], False);
            _evaluateAndAssert(IsConstant[List[1, 2]], False);

            _evaluateAndAssert(IsStringSymbol["Three"], True);
            _evaluateAndAssert(IsStringSymbol[3], False);
            _evaluateAndAssert(IsStringSymbol[List[1, 2]], False);

            _evaluateAndAssert(IsExpressionWithName[Times]["Three"], False);
            _evaluateAndAssert(IsExpressionWithName[Times][3], False);
            _evaluateAndAssert(IsExpressionWithName[Times][Times["x", "y"]], True);
        }
    }
}