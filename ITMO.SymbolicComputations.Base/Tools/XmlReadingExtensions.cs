using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class XmlReadingExtensions {
        public static ExpressionInfo AsExpressionInfo(this XmlDocument doc) {
            var root = doc.DocumentElement;
            return new ExpressionInfo(ParseSymbol(root.FirstChild));

            Symbol ParseSymbol(XmlNode xmlElement) =>
                xmlElement.Name switch {
                    "Const" => ParseConstant(xmlElement),
                    "Symbol" => ParseStringSymbol(xmlElement),
                    _ => ParseExpression(xmlElement)
                };

            Symbol ParseConstant(XmlNode xmlElement) =>
                new Constant(decimal.Parse(xmlElement.Attributes["Value"].Value));

            Symbol ParseStringSymbol(XmlNode xmlElement) =>
                new StringSymbol(xmlElement.Attributes["Name"].Value);

            Symbol ParseExpression(XmlNode xmlElement) =>
                new Expression(
                    new StringSymbol(xmlElement.Name),
                    xmlElement.ChildNodes
                        .OfType<XmlNode>()
                        .Select(ParseSymbol)
                        .ToImmutableList()
                );
        }
    }
}