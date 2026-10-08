using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Parsing {
    /// <summary>
    /// Parses explicit arithmetic and calls without executing the supplied formula.
    /// Exponentiation is right associative and binds more tightly than unary signs.
    /// </summary>
    public static class SymbolicParser {
        public static Symbol Parse(string formula) => Parse(formula, new FormulaParserLimits());

        public static Symbol Parse(string formula, FormulaParserLimits limits) {
            if (limits == null) {
                throw new ArgumentNullException(nameof(limits));
            }

            if (limits.MaxInputLength < 1 || limits.MaxNodes < 1 ||
                limits.MaxDepth < 1 || limits.MaxDepth > 256) {
                throw new ArgumentOutOfRangeException(nameof(limits),
                    "Лимиты длины и числа узлов должны быть положительными, глубина — от 1 до 256.");
            }

            if (formula == null) {
                throw new FormulaParseException("Введите формулу", 0);
            }

            if (formula.Length > limits.MaxInputLength) {
                throw new FormulaParseException(
                    $"Формула слишком длинная: максимум {limits.MaxInputLength} символов",
                    limits.MaxInputLength);
            }

            return new Reader(formula, limits.MaxDepth, limits.MaxNodes).Read();
        }

        private sealed class Reader {
            private readonly string text;
            private readonly int maxDepth;
            private readonly int maxNodes;
            private int cursor;
            private int recursionDepth;
            private int nodes;

            public Reader(string text, int maxDepth, int maxNodes) {
                this.text = text;
                this.maxDepth = maxDepth;
                this.maxNodes = maxNodes;
            }

            public Symbol Read() {
                SkipWhitespace();
                if (cursor == text.Length) {
                    throw Error("Введите формулу");
                }

                var result = ReadExpression(0);
                SkipWhitespace();
                if (cursor != text.Length) {
                    if (IsIdentifierStart(text[cursor]) || char.IsDigit(text[cursor]) || text[cursor] == '(') {
                        throw Error("Ожидался оператор. Для умножения используйте '*'");
                    }

                    throw Error($"Лишний символ '{text[cursor]}' после выражения");
                }

                return result.Value;
            }

            private Node ReadExpression(int minimumPriority) {
                SkipWhitespace();
                if (++recursionDepth > maxDepth) {
                    throw Error($"Превышена допустимая глубина формулы ({maxDepth})");
                }

                try {
                    var left = ReadPrefix();
                    while (true) {
                        SkipWhitespace();
                        if (cursor == text.Length) {
                            return left;
                        }

                        var token = text[cursor];
                        if ((token == '(' || token == '[') && minimumPriority <= 50) {
                            if (left.Value is Constant) {
                                throw Error("Число нельзя вызвать как функцию. Для умножения используйте '*'");
                            }

                            var position = cursor++;
                            left = Expression(left, ReadArguments(token == '(' ? ')' : ']'), position);
                            continue;
                        }

                        var priority = Priority(token);
                        if (priority < minimumPriority || priority < 0) {
                            return left;
                        }

                        var operatorPosition = cursor++;
                        var right = ReadExpression(token == '^' ? priority : priority + 1);
                        switch (token) {
                            case '+': left = Operation("Plus", operatorPosition, left, right); break;
                            case '-': left = Operation("Plus", operatorPosition, left, Negate(right, operatorPosition)); break;
                            case '*': left = Operation("Times", operatorPosition, left, right); break;
                            case '/': left = Operation("Divide", operatorPosition, left, right); break;
                            case '^': left = Operation("Power", operatorPosition, left, right); break;
                        }
                    }
                }
                finally {
                    recursionDepth--;
                }
            }

            private Node ReadPrefix() {
                SkipWhitespace();
                if (cursor == text.Length) {
                    throw Error("Ожидалось выражение");
                }

                var position = cursor;
                var token = text[cursor];
                if (token == '+' || token == '-') {
                    cursor++;
                    var operand = ReadExpression(30);
                    return token == '+' ? operand : Negate(operand, position);
                }

                if (token == '(') {
                    cursor++;
                    var expression = ReadExpression(0);
                    Expect(')');
                    return expression;
                }

                if (token == '{') {
                    cursor++;
                    var head = Leaf(BuiltInSymbols.Resolve("List"), position);
                    return Expression(head, ReadArguments('}'), position);
                }

                if (IsAsciiDigit(token) || token == '.') {
                    return ReadNumber();
                }

                if (IsIdentifierStart(token)) {
                    cursor++;
                    while (cursor < text.Length && IsIdentifierPart(text[cursor])) {
                        cursor++;
                    }

                    return Leaf(BuiltInSymbols.Resolve(text.Substring(position, cursor - position)), position);
                }

                throw Error($"Ожидалось число, имя или скобка; получен символ '{token}'");
            }

            private Node ReadNumber() {
                var start = cursor;
                var digits = 0;
                while (cursor < text.Length && IsAsciiDigit(text[cursor])) {
                    cursor++;
                    digits++;
                }

                if (cursor < text.Length && text[cursor] == '.') {
                    cursor++;
                    while (cursor < text.Length && IsAsciiDigit(text[cursor])) {
                        cursor++;
                        digits++;
                    }
                }

                if (digits == 0) {
                    throw new FormulaParseException("После десятичной точки ожидалась цифра", start);
                }

                var mantissaEnd = cursor;
                if (cursor < text.Length && (text[cursor] == 'e' || text[cursor] == 'E')) {
                    cursor++;
                    if (cursor < text.Length && (text[cursor] == '+' || text[cursor] == '-')) {
                        cursor++;
                    }

                    var exponentStart = cursor;
                    while (cursor < text.Length && IsAsciiDigit(text[cursor])) {
                        cursor++;
                    }

                    if (cursor == exponentStart) {
                        throw Error("В показателе степени ожидалась цифра");
                    }
                }

                if (!decimal.TryParse(text.Substring(start, cursor - start),
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                        CultureInfo.InvariantCulture, out var value)) {
                    throw new FormulaParseException("Число выходит за диапазон decimal", start);
                }

                if (value == 0m) {
                    for (var i = start; i < mantissaEnd; i++) {
                        if (text[i] >= '1' && text[i] <= '9') {
                            throw new FormulaParseException("Число слишком мало для точности decimal", start);
                        }
                    }
                }

                return Leaf(new Constant(value), start);
            }

            private List<Node> ReadArguments(char closing) {
                var arguments = new List<Node>();
                SkipWhitespace();
                if (cursor < text.Length && text[cursor] == closing) {
                    cursor++;
                    return arguments;
                }

                while (true) {
                    arguments.Add(ReadExpression(0));
                    SkipWhitespace();
                    if (cursor < text.Length && text[cursor] == closing) {
                        cursor++;
                        return arguments;
                    }

                    if (cursor == text.Length || text[cursor] != ',') {
                        throw Error($"Ожидалась запятая или '{closing}'");
                    }

                    cursor++;
                    SkipWhitespace();
                    if (cursor == text.Length || text[cursor] == closing) {
                        throw Error("После запятой ожидалось выражение");
                    }
                }
            }

            private Node Negate(Node operand, int position) =>
                Operation("Times", position, Leaf(new Constant(-1m), position), operand);

            private Node Operation(string name, int position, params Node[] arguments) =>
                Expression(Leaf(BuiltInSymbols.Resolve(name), position), arguments, position);

            private Node Expression(Node head, IReadOnlyCollection<Node> arguments, int position) {
                var depth = 1 + Math.Max(head.Depth, arguments.Count == 0 ? 0 : arguments.Max(node => node.Depth));
                if (depth > maxDepth) {
                    throw new FormulaParseException($"Превышена допустимая глубина дерева ({maxDepth})", position);
                }

                CountNode(position);
                return new Node(new Expression(head.Value, arguments.Select(node => node.Value).ToImmutableList()), depth);
            }

            private Node Leaf(Symbol value, int position) {
                CountNode(position);
                return new Node(value, 1);
            }

            private void CountNode(int position) {
                if (++nodes > maxNodes) {
                    throw new FormulaParseException($"В формуле слишком много узлов (максимум {maxNodes})", position);
                }
            }

            private void Expect(char expected) {
                SkipWhitespace();
                if (cursor == text.Length || text[cursor] != expected) {
                    throw Error($"Ожидалась закрывающая скобка '{expected}'");
                }

                cursor++;
            }

            private void SkipWhitespace() {
                while (cursor < text.Length && char.IsWhiteSpace(text[cursor])) {
                    cursor++;
                }
            }

            private FormulaParseException Error(string message) => new FormulaParseException(message, cursor);

            private static int Priority(char token) =>
                token == '+' || token == '-' ? 10 : token == '*' || token == '/' ? 20 : token == '^' ? 40 : -1;

            private static bool IsAsciiDigit(char value) => value >= '0' && value <= '9';
            private static bool IsIdentifierStart(char value) => char.IsLetter(value) || value == '_';
            private static bool IsIdentifierPart(char value) => char.IsLetterOrDigit(value) || value == '_' || value == '\'';

            private readonly struct Node {
                public Node(Symbol value, int depth) {
                    Value = value;
                    Depth = depth;
                }

                public Symbol Value { get; }
                public int Depth { get; }
            }
        }
    }
}
