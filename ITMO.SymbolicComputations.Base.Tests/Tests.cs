using System;
using System.IO;
using System.Xml;
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

            var expressionInfo = XmlExpressionReader.ReadXml(document);
        }
    }
}