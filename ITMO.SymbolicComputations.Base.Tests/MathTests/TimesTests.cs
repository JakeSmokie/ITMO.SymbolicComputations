using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests {
    public sealed class TimesTests {
        public TimesTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;
        
        [Fact]
        public void BinaryTimesWorks() {
            var source = Times[1m, 3m];
            var (steps, symbol) = source.Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(3, symbol);
        }

        [Fact]
        public void ListTimesWorks() {
            var source = ListTimes[List[-1, 6, 3]];
            var (steps, symbol) = source.Visit(FullEvaluator.Default);

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(-18, symbol);
        }
    }
}