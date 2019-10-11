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

            BaseSymbol ParseExpression(XmlNode xmlElement) {
                return xmlElement.Name switch {
                    "Function" => ParseFunction(xmlElement),
                    "BinaryOperation" => ParseBinaryOperation(xmlElement),
                    "UnaryOperation" => ParseUnaryOperation(xmlElement),
                    "Const" => ParseConstant(xmlElement),
                    "Symbol" => ParseSymbol(xmlElement),
                    _ => throw new ArgumentException($"Wrong tag: {xmlElement.Name}")
                };
            }

            BaseSymbol ParseFunction(XmlNode xmlElement) {
                return new Function(
                    xmlElement.Attributes["Name"].Value,
                    xmlElement.ChildNodes
                        .OfType<XmlNode>()
                        .Select(ParseExpression)
                        .ToImmutableList()
                );
            }

            BaseSymbol ParseBinaryOperation(XmlNode xmlElement) {
                return new BinaryOperation(
                    ParseExpression(xmlElement.ChildNodes[0]),
                    ParseExpression(xmlElement.ChildNodes[1]),
                    xmlElement.Attributes["Name"].Value
                );
            }

            BaseSymbol ParseUnaryOperation(XmlNode xmlElement) {
                return new UnaryOperation(
                    ParseExpression(xmlElement.FirstChild),
                    xmlElement.Attributes["Name"].Value
                );
            }

            BaseSymbol ParseConstant(XmlNode xmlElement) {
                return new Constant(decimal.Parse(xmlElement.Attributes["Value"].Value));
            }

            BaseSymbol ParseSymbol(XmlNode xmlElement) {
                return new Symbol(xmlElement.Attributes["Name"].Value);
            }
        }
    }
}