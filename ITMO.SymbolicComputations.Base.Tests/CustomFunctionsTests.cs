using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class CustomFunctionsTests {
        public CustomFunctionsTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        
        [Fact]
        public void XPlus2() {
            Symbol x = "x";

            var func = Function[x, Plus[x, 2]];
            var (steps, result) = func[3].Visit(new FullEvaluator());
            
            _out.WriteLine(func.Visit(new MathematicaPrinter()));
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            
            Assert.Equal(5, result);
        }
        
        [Fact]
        public void XReturnsConstant() {
            Symbol x = "x";

            var func = Function[x, 3];
            var (steps, result) = func[0].Visit(new FullEvaluator());
            
            _out.WriteLine(func.Visit(new MathematicaPrinter()));
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            
            Assert.Equal(3, result);
        }
        
        [Fact]
        public void XPower2() {
            Symbol x = "x";

            var func = Function[x, Power[x, 2]];
            var (steps, result) = func[7].Visit(new FullEvaluator());
            
            _out.WriteLine(func.Visit(new MathematicaPrinter()));
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            
            Assert.Equal(49, result);
        }
        
        [Fact]
        public void TwoPowerX() {
            Symbol x = "x";

            var func = Function[x, Power[2, x]];
            var (steps, result) = func[7].Visit(new FullEvaluator());
            
            _out.WriteLine(func.Visit(new MathematicaPrinter()));
            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            
            Assert.Equal(128, result);
        }
    }
}