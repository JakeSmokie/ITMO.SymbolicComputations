using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class XmlReadingExtensions {
        public static ExpressionInfo AsExpressionInfo(this XmlDocument doc) {
            var root = doc.DocumentElement;

            if (root.Name != nameof(ExpressionInfo)) {
                throw new ArgumentException("");
            }

            if (root.ChildNodes.Count > 1) {
                throw new ArgumentException("");
            }

            return new ExpressionInfo(ParseExpression(root.FirstChild));

            ISymbol ParseExpression(XmlNode xmlElement) =>
                xmlElement.Name switch {
                    "Const" => ParseConstant(xmlElement),
                    "Symbol" => ParseSymbol(xmlElement),
                    _ => ParseFunction(xmlElement)
                };

            ISymbol ParseConstant(XmlNode xmlElement) =>
                new Constant(decimal.Parse(xmlElement.Attributes["Value"].Value));

            ISymbol ParseSymbol(XmlNode xmlElement) =>
                new StringSymbol(xmlElement.Attributes["Name"].Value);

            ISymbol ParseFunction(XmlNode xmlElement) =>
                new Function(
                    new StringSymbol(xmlElement.Name),
                    xmlElement.ChildNodes
                        .OfType<XmlNode>()
                        .Select(ParseExpression)
                        .ToImmutableList()
                );
        }
    }
}