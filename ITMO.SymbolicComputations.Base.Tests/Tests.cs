using System.IO;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class Tests {
        public Tests(ITestOutputHelper output) {
            _out = output;
        }

        private readonly ITestOutputHelper _out;

        [Fact]
        public void XmlParsesWell() {
            var document = new XmlDocument();
            document.Load("Samples/First.xml");

            var json = document.AsExpressionInfo().AsJson();
            _out.WriteLine(json);

            Assert.Equal(File.ReadAllText("Samples/First.json"), json);
        }

        [Fact]
        public void MathematicaOutputIsOkay() {
            var document = new XmlDocument();
            document.Load("Samples/First.xml");

            var mathematica = document.AsExpressionInfo().AsMathematica();
            _out.WriteLine(mathematica);

            Assert.Equal("Times[Plus[Times[Plus[3, 5], x], 10], x]", mathematica);
        }
    }
}