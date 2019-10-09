using System;
using System.Xml;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class XmlExpressionReader {
        public static ExpressionInfo ReadXml(XmlDocument doc) {
            var root = doc.DocumentElement;

            if (root.Name != nameof(ExpressionInfo)) {
                throw new ArgumentException("");
            }

            if (root.ChildNodes.Count > 1) {
                throw new ArgumentException("");
            }

            return new ExpressionInfo(ParseExpression(root.FirstChild));

            Expression ParseExpression(XmlNode xmlElement) {
                return xmlElement.Name switch {
                    "BinaryOperation" => ParseBinaryOperation(xmlElement),
                    "UnaryOperation" => ParseUnaryOperation(xmlElement),
                    "Const" => ParseConstant(xmlElement),
                    "Symbol" => ParseSymbol(xmlElement),
                    _ => throw new ArgumentException($"Wrong tag: {xmlElement.Name}")
                };
            }

            Expression ParseBinaryOperation(XmlNode xmlElement) {
                return new BinaryOperation(
                    ParseExpression(xmlElement.ChildNodes[0]),
                    ParseExpression(xmlElement.ChildNodes[1]),
                    xmlElement.Attributes["Name"].Value
                );
            }

            Expression ParseUnaryOperation(XmlNode xmlElement) {
                return new UnaryOperation(
                    ParseExpression(xmlElement.FirstChild),
                    xmlElement.Attributes["Name"].Value
                );
            }
            
            Expression ParseConstant(XmlNode xmlElement) {
                return new Constant(decimal.Parse(xmlElement.Attributes["Value"].Value));
            }
            
            Expression ParseSymbol(XmlNode xmlElement) {
                return new Symbol(xmlElement.Attributes["Name"].Value);
            }
        }
    }
}