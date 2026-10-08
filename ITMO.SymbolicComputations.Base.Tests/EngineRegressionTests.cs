using System;
using System.Collections.Generic;
using ITMO.SymbolicComputations.Base.Models;
using Xunit;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class EngineRegressionTests {
        [Fact]
        public void DivisionByZeroDoesNotInventAFiniteAnswer() {
            var error = Assert.Throws<DivideByZeroException>(() => new SymbolicContext().Run(Divide[1, 0]));
            Assert.Contains("zero", error.Message);
        }

        [Fact]
        public void IndependentlyConstructedEqualExpressionsHaveEqualHashes() {
            Symbol x = "x";
            var first = Plus[x, Times[2, x]];
            var second = Plus[new StringSymbol("x"), Times[2, new StringSymbol("x")]];

            Assert.Equal(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
            Assert.Single(new HashSet<Symbol> {first, second});
        }

        [Fact]
        public void DistinctRemovesStructurallyEqualExpressions() {
            Symbol f = "f";
            var (_, actual) = new SymbolicContext().Run(Distinct[List[f[1, 2], f[1, 2]]]);
            Assert.Equal(List[f[1, 2]], actual);
        }

        [Fact]
        public void ZeroToNegativePowerIsAnError() =>
            Assert.Throws<DivideByZeroException>(() => new SymbolicContext().Run(Power[0, -1]));

        [Fact]
        public void UnsupportedComplexPowerIsAnError() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new SymbolicContext().Run(Power[-1, 0.5m]));

        [Theory]
        [InlineData(-1)]
        [InlineData(1.5)]
        public void GeneratedListsRequireWholeNonNegativeCounts(double count) {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SymbolicContext().Run(GenerateList[(decimal) count]));
            Assert.Throws<ArgumentOutOfRangeException>(() => new SymbolicContext().Run(StandardLibrary.ListFunctions.Range[0, 10, (decimal) count]));
        }

        [Fact]
        public void EmptyRangeDoesNotDivideByZero() {
            var (_, actual) = new SymbolicContext().Run(StandardLibrary.ListFunctions.Range[0, 10, 0]);
            Assert.Equal(List[Array.Empty<Symbol>()], actual);
        }

        [Fact]
        public void FactorialRejectsNegativeAndFractionalValuesIncludingEvaluatedArguments() {
            var context = new SymbolicContext();
            Assert.Throws<ArgumentOutOfRangeException>(() => context.Run(Factorial[-1]));
            Assert.Throws<ArgumentOutOfRangeException>(() => context.Run(Factorial[1.5m]));
            Assert.Throws<ArgumentOutOfRangeException>(() => context.Run(Factorial[Divide[3, 2]]));
            Assert.Throws<OverflowException>(() => context.Run(Factorial[28]));
            Assert.Equal((Symbol) 1, context.Run(Factorial[0]).Item2);
            Assert.Equal((Symbol) 120, context.Run(Factorial[5]).Item2);
        }

        [Fact]
        public void ApplyListConsumesFastMapOutput() {
            Symbol x = "x";
            var input = StandardLibrary.Functions.ApplyList[Plus,
                FastMap[List[1, 2, 3], StandardLibrary.Functions.Fun[x, Times[x, 2]]]];
            Assert.Equal((Symbol) 12, new SymbolicContext().Run(input).Item2);
        }

        [Fact]
        public void EmptyIfHasAHandledArgumentError() =>
            Assert.Throws<ArgumentException>(() => new SymbolicContext().Run(
                StandardLibrary.BooleanFunctions.If[Array.Empty<Symbol>()]));

        [Fact]
        public void FunctionCallsRequireExactlyTheirDeclaredArity() {
            Symbol x = "x";
            Symbol y = "y";
            var context = new SymbolicContext();
            var unary = StandardLibrary.Functions.Fun[x, x];
            var binary = StandardLibrary.Functions.Fun[List[x, y], Plus[x, y]];
            Assert.Throws<ArgumentException>(() => context.Run(unary[Array.Empty<Symbol>()]));
            Assert.Throws<ArgumentException>(() => context.Run(unary[1, 2]));
            Assert.Throws<ArgumentException>(() => context.Run(binary[1]));
            Assert.Throws<ArgumentException>(() => context.Run(binary[1, 2, 3]));
            Assert.Equal((Symbol) 3, context.Run(binary[1, 2]).Item2);
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(-1)]
        [InlineData(2)]
        public void PartRejectsFractionalNegativeAndOutOfRangeIndexes(double index) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new SymbolicContext().Run(
                Part[List[1, 2], (decimal) index]));
    }
}
