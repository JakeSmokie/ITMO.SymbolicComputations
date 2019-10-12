using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class HoldTests {
        private static readonly StringSymbol HoldFirst = new StringSymbol("", Attributes.HoldFirst);
        private static readonly StringSymbol HoldRest = new StringSymbol("", Attributes.HoldRest);

        public HoldTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void HoldFirstWorks() {
            var source = HoldFirst[Plus[2], Plus[3]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(HoldFirst[Plus[2], 3], expression);
        }

        [Fact]
        public void HoldFirstSuppressionWorks() {
            var holdFirst = new StringSymbol("", Attributes.HoldFirst);

            var source = holdFirst[Evaluate[Plus[2]], Plus[3]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(holdFirst[2, 3], expression);
        }


        [Fact]
        public void HoldFormSuppressingWithEvaluateFunctionWorks() {
            var source = HoldForm[Evaluate[Plus[2]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(2, expression);
        }

        [Fact]
        public void HoldFormWorks() {
            var source = Plus[2];
            var expression = HoldForm[source].Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }

        [Fact]
        public void HoldIsNotSuppressedWhenItIsComplete() {
            var source = HoldComplete[Evaluate[Plus[2]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }

        [Fact]
        public void HoldSuppressingWithEvaluateFunctionWorks() {
            var source = Hold[Evaluate[Plus[2]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Hold[2], expression);
        }

        [Fact]
        public void HoldWorks() {
            var source = Hold[Plus[2]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }

        [Fact]
        public void NestedHoldWorks() {
            var insides = Hold[Plus[2]];
            var source = Plus[insides];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(insides, expression);
        }
    }
}