using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class AttributesTests {
        public AttributesTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void ConstInsidePlusIsReducedToConst() {
            var expression = Plus[Plus[Plus[Plus[2]]]]
                .Visit(new FullEvaluator());
            
            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(2, expression);
        }
        
        [Fact]
        public void PlusInsidePlusIsReducedToOnePlus() {
            var expression = Plus[Plus[Plus[Plus[2, 3]]]]
                .Visit(new FullEvaluator());
            
            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Plus[2, 3], expression);
        }
        
        [Fact]
        public void HoldWorks() {
            var source = Hold[Plus[Plus[Plus[Plus[2]]]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }

        [Fact]
        public void HoldFormWorks() {
            var source = Plus[Plus[Plus[Plus[2]]]];
            var expression = HoldForm[source].Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }
        
        [Fact]
        public void NestedHoldWorks() {
            var insides = Hold[Plus[Plus[2]]];
            var source = Plus[Plus[insides]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(insides, expression);
        }

        [Fact]
        public void HoldSuppressingWithEvaluateFunctionWorks() {
            var source = Hold[Evaluate[Plus[Plus[Plus[Plus[2]]]]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(Hold[2], expression);
        }
        
        [Fact]
        public void HoldIsNotSuppressedWhenItIsComplete() {
            var source = HoldComplete[Evaluate[Plus[Plus[Plus[Plus[2]]]]]];
            var expression = source.Visit(new FullEvaluator());

            _out.WriteLine(expression.Visit(new MathematicaPrinter()));
            Assert.Equal(source, expression);
        }
    }
}