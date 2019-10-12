using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests.AttributesTests {
    public sealed class HoldTests {
        public HoldTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;
        
        [Fact]
        public void HoldWorks() {
            var source = Hold[Plus[2]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }

        [Fact]
        public void HoldFormWorks() {
            var source = Plus[2];
            var expression = HoldForm[source].Visit(new FullEvaluator());

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

        [Fact]
        public void HoldSuppressingWithEvaluateFunctionWorks() {
            var source = Hold[Evaluate[Plus[2]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Hold[2], expression);
        }

        [Fact]
        public void HoldFormSuppressingWithEvaluateFunctionWorks() {
            var source = HoldForm[Evaluate[Plus[2]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(2, expression);
        }

        [Fact]
        public void HoldIsNotSuppressedWhenItIsComplete() {
            var source = HoldComplete[Evaluate[Plus[2]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }
    }
}