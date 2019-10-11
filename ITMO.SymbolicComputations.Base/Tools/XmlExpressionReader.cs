using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class XmlExpressionReader {
        public static ExpressionInfo AsExpressionInfo(this XmlDocument doc) {
            var root = doc.DocumentElement;

            if (root.Name != nameof(ExpressionInfo)) {
                throw new ArgumentException("");
            }

            if (root.ChildNodes.Count > 1) {
                throw new ArgumentException("");
            }

            return new ExpressionInfo(ParseExpression(root.FirstChild));

            BaseSymbol ParseExpression(XmlNode xmlElement) =>
                xmlElement.Name switch {
                    "Const" => ParseConstant(xmlElement),
                    "Symbol" => ParseSymbol(xmlElement),
                    _ => ParseFunction(xmlElement)
                };

            BaseSymbol ParseConstant(XmlNode xmlElement) =>
                new Constant(decimal.Parse(xmlElement.Attributes["Value"].Value));

            BaseSymbol ParseSymbol(XmlNode xmlElement) =>
                new Symbol(xmlElement.Attributes["Name"].Value);

            BaseSymbol ParseFunction(XmlNode xmlElement) =>
                new Function(
                    new Symbol(xmlElement.Name),
                    xmlElement.ChildNodes
                        .OfType<XmlNode>()
                        .Select(ParseExpression)
                        .ToImmutableList()
                );
        }
    }
}