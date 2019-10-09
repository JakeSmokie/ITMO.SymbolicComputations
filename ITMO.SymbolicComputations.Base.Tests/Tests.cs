using System;
using System.IO;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class Tests {
        private readonly ITestOutputHelper _testOutputHelper;

        public Tests(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void LoadsWell() {
            var document = new XmlDocument();
            document.Load("Samples/First.xml");

            var expressionInfo = document.AsExpressionInfo();
            
            var a = (BinaryOperation) expressionInfo.Expression;
            var b = (BinaryOperation) a.First;
            var c = (Symbol) a.Second;
            var d = (BinaryOperation) b.First;
            var e = (Constant) b.Second;
            var f = (BinaryOperation) d.First;
            var g = (Symbol) d.Second;
            var h = (Constant) f.First;
            var j = (Constant) f.Second;

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
    }
}