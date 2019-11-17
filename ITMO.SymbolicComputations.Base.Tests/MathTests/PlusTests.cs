﻿using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Tools;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests.MathTests {
    public sealed class PlusTests {
        public PlusTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void BinaryPlusWorks() {
            var source = BinaryPlus[1, 2];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.Print(_out);
            Assert.Equal(3, symbol);
        }

        [Fact]
        public void ListPlusWorks() {
            var source = ListPlus[List[1, 3, 5, 7, -7]];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.Print(_out);
            Assert.Equal(9, symbol);
        }
        
        [Fact]
        public void PlusForSameSymbolCreatesTimes() {
            Symbol x = "x";
            Symbol y = "y";

            var source = BinaryPlus[y, x, x];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(BinaryPlus[BinaryTimes[x, 2], y], symbol);
        }

        [Fact]
        public void PlusForSameSymbolCreatesTimes2() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var source = BinaryPlus[x, y, z, z, x, x, y];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(BinaryPlus[BinaryTimes[x, 3], BinaryTimes[y, 2], BinaryTimes[z, 2]], symbol);
        }

        [Fact]
        public void XPlusZeroEqualsX() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            var source = BinaryPlus[x, y, z, 6, -6];
            var (steps, symbol) = source.Visit(new FullEvaluator());

            steps.WithoutDuplicates().ForEach(e => _out.WriteLine(e.Visit(new MathematicaPrinter())));
            Assert.Equal(BinaryPlus[x, y, z], symbol);
        }
    }
}