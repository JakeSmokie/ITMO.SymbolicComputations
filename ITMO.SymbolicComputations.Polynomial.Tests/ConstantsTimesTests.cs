﻿using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.TimesConstantsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class ConstantsTimesTests {
        public ConstantsTimesTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void ConstantsMultipliedGeneral() {
            Symbol x = "x";
            Symbol y = "y";
            
            _evaluateAndAssert(
                TimesConstants[
                    List[3, x, y, 10, -1]
                ],
                List[x, y, -30]
            );
        }
        
        [Fact]
        public void ConstantsMultipliedWithZero() {
            Symbol x = "x";
            Symbol y = "y";
            
            _evaluateAndAssert(
                TimesConstants[
                    List[3, x, y, 0, -1]
                ],
                0
            );
        }
        
        [Fact]
        public void ConstantsMultipliedWithOne() {
            Symbol x = "x";
            Symbol y = "y";
            
            _evaluateAndAssert(
                TimesConstants[
                    List[1, x, y, 0.2m, 5]
                ],
                List[x, y]
            );
        }
    }
}