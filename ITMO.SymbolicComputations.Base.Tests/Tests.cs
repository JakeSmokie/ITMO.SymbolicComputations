using System;
using System.IO;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class Tests {
        private readonly ITestOutputHelper _out;

        public Tests(ITestOutputHelper output) {
            _out = output;
        }

        [Fact]
        public void LoadsWell() {
            var document = new XmlDocument();
            document.Load("Samples/First.xml");

            var expressionInfo = document.AsExpressionInfo();
            _out.WriteLine(expressionInfo.AsJson());

            var a = (Function) expressionInfo.BaseSymbol;
            var b = (Function) a.Arguments[0];
            var c = (Symbol) a.Arguments[1];
            var d = (Function) b.Arguments[0];
            var e = (Constant) b.Arguments[1];
            var f = (Function) d.Arguments[0];
            var g = (Symbol) d.Arguments[1];
            var h = (Constant) f.Arguments[0];
            var j = (Constant) f.Arguments[1];

            Assert.Equal("Mul", a.Name);
            Assert.Equal("Sum", b.Name);
            Assert.Equal("x", c.Name);
            Assert.Equal("Mul", d.Name);
            Assert.Equal(10, e.Value);
            Assert.Equal("Sum", f.Name);
            Assert.Equal("x", g.Name);
            Assert.Equal(3, h.Value);
            Assert.Equal(5, j.Value);
        }

        [Fact]
        public void SimplifiesConstSum() {
            var document = new XmlDocument();
            document.Load("Samples/Second.xml");

            var symbol = document.AsExpressionInfo().Simplify();
            _out.WriteLine(symbol.AsJson());

            Assert.Equal(8, ((Constant) symbol.BaseSymbol).Value);
        }
        [Fact]
        public void SumsMergeAndEntriesSort() {
            var document = new XmlDocument();
            document.Load("Samples/MergeSums.xml");

            var symbol = document.AsExpressionInfo().Simplify();
            _out.WriteLine(symbol.AsJson());

            Assert.Equal(File.ReadAllText("Samples/MergeSums.json"), symbol.AsJson());
        }

    }
}