using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class VariablesTests {
        public VariablesTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void Dependency() {
            Symbol x = "x";
            Symbol y = "y";
            
            var expression = Seq[
                SetDelayed[x, Plus[y, 2]],
                SetDelayed[y, 7],
                x,
                Plus[x, y]
            ];

            evaluateAndAssert(expression, Seq[SetDelayed[x, 9], SetDelayed[y, 7], 9, 16]);
        }

        [Fact]
        public void MapInsideFunction() {
            var expression = Part[List[
                SetDelayed[f, Fun[x, Map[GenerateList[x]][Fun[y, Plus[y, 1]]]]],
                SetDelayed[y, 7],
                f[y]
            ], 2];

            evaluateAndAssert(expression, List[1, 2, 3, 4, 5, 6, 7]);
        }
    }
}