using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.CastingFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class CastingTests {
        public CastingTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void CastingWorks() {
            evaluateAndAssert(AsConstant[3], 3);
            evaluateAndAssert(AsConstant["Three"], Null);
            evaluateAndAssert(AsConstant[List[1, 2]], Null);

            evaluateAndAssert(AsStringSymbol["Three"], "Three");
            evaluateAndAssert(AsStringSymbol[3], Null);
            evaluateAndAssert(AsStringSymbol[List[1, 2]], Null);

            evaluateAndAssert(AsExpressionArgs[Times, "Three"], Null);
            evaluateAndAssert(AsExpressionArgs[Times, 3], Null);
            evaluateAndAssert(
                AsExpressionArgs[Times, Times["x", "y"]],
                List["x", "y"]
            );
        }

        [Fact]
        public void TypeTestingWorks() {
            evaluateAndAssert(IsConstant[3], True);
            evaluateAndAssert(IsConstant["Three"], False);
            evaluateAndAssert(IsConstant[List[1, 2]], False);

            evaluateAndAssert(IsStringSymbol["Three"], True);
            evaluateAndAssert(IsStringSymbol[3], False);
            evaluateAndAssert(IsStringSymbol[List[1, 2]], False);

            evaluateAndAssert(IsExpressionWithName[Times]["Three"], False);
            evaluateAndAssert(IsExpressionWithName[Times][3], False);
            evaluateAndAssert(IsExpressionWithName[Times][Times["x", "y"]], True);
        }
    }
}