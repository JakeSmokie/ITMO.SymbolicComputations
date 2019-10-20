using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Predefined;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class StepsTests {
        public StepsTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;
        private static readonly StringSymbol Orderless = new StringSymbol("Orderless", Attributes.Orderless);


        [Fact]
        public void NestedOrderingLoggingWorks() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var source = Orderless[y, Orderless[1, x, Orderless[y]], x, z];
            var steps = source.Visit(new FullEvaluator()).Steps.WithoutDuplicates();

            steps.ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));

            var expected = new [] {
                Orderless[y],
                Orderless[1, x, Orderless[y]],
                Orderless[Orderless[y], x, 1],
                Orderless[y, Orderless[Orderless[y], x, 1], x, z],
                Orderless[Orderless[Orderless[y], x, 1], x, y, z]
            };
            
            Assert.Equal(expected, steps);
        }
        
        [Fact]
        public void MoreComplexFlatWorks() {
            Symbol a = "a";
            Symbol b = "b";
            Symbol c = "c";
            Symbol d = "d";
            Symbol e = "e";
            Symbol f = "f";
            Symbol g = "g";
            Symbol h = "h";
            
            var source = Plus[Plus[a], Plus[b, Plus[c, d, Plus[e]], f], g, h];
            var steps = source.Visit(new FullEvaluator()).Steps.WithoutDuplicates();

            var expected = new [] {
                Plus[a],
                a,
                Plus[e],
                e,
                Plus[c, d, e],
                Plus[b, Plus[c, d, e], f],
                Plus[b, c, d, e, f],
                Plus[a, Plus[b, c, d, e, f], g, h],
                Plus[a, b, c, d, e, f, g, h]
            };

            steps.ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(expected, steps);
        }
        
        [Fact]
        public void HoldFormWorks() {
            var source = Plus[2];
            var steps = HoldForm[source].Visit(new FullEvaluator()).Steps.WithoutDuplicates();

            steps.ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));

            var expected = new [] {
                HoldForm[Plus[2]],
                Plus[2]
            };

            Assert.Equal(expected, steps);
        }
    }
}