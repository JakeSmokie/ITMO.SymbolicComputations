using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Alphabet;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.BooleanFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class RecursionTests {
        public RecursionTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void RecursiveFacWorks() {
            Symbol fac = "fac";

            evaluateAndAssert(Part[List[
                SetDelayed[fac, Fun[x,
                    If[More[x][1], Times[x, fac[Plus[x, -1]]], 1]
                ]],
                fac[5]
            ], 1], 120);
        }

        [Fact]
        public void WhileFacWorks() {
            evaluateAndAssert(Seq[
                Part[
                    While[List[5, 1]][Fun[list,
                        More[Part[list, 0]][1]
                    ]][Fun[list,
                        List[
                            Plus[Part[list, 0], -1],
                            Times[Part[list, 0], Part[list, 1]]
                        ]
                    ]],
                    1
                ]
            ], 120);
        }

        [Fact]
        public void WhileWorks() {
            evaluateAndAssert(Seq[
                While[2][Fun[x,
                    Less[x][500]
                ]][Fun[x,
                    Times[x, 2]
                ]]
            ], 512);
        }
    }
}