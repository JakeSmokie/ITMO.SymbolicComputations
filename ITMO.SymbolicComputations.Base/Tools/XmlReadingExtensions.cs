using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class XmlReadingExtensions {
        public static ExpressionInfo AsExpressionInfo(this XmlDocument doc) {
            var root = doc.DocumentElement;

            if (root.Name != "Expression") {
                throw new ArgumentException("");
            }

            return new ExpressionInfo(ParseExpression(root.FirstChild));

            Symbol ParseExpression(XmlNode xmlElement) =>
                xmlElement.Name switch {
                    "Const" => ParseConstant(xmlElement),
                    "Symbol" => ParseSymbol(xmlElement),
                    "ApplySymbol" => ParseFunction(xmlElement),
                    _ => throw new NotImplementedException()
                };

            Symbol ParseConstant(XmlNode xmlElement) =>
                new Constant(decimal.Parse(xmlElement.Attributes["Value"].Value));

            Symbol ParseSymbol(XmlNode xmlElement) =>
                new StringSymbol(xmlElement.Attributes["Name"].Value);

            Symbol ParseFunction(XmlNode xmlElement) =>
                new Expression(
                    new StringSymbol(xmlElement.Name),
                    xmlElement.ChildNodes
                        .OfType<XmlNode>()
                        .Select(ParseExpression)
                        .ToImmutableList()
                );
        }
    }
}