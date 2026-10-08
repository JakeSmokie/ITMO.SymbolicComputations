using System.Globalization;
using System.Text;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Workbench;

/// <summary>Presentation only: preserves the expression tree, never evaluates TeX.</summary>
public static class LatexFormatter {
    public static string Format(Symbol symbol) {
        ArgumentNullException.ThrowIfNull(symbol);
        var writer = new Writer();
        writer.Visit(symbol, 0, 0);
        return writer.ToString();
    }

    private sealed class Writer {
        private readonly StringBuilder output = new();
        private int nodes;
        public override string ToString() => output.ToString();

        private void Append(string text) {
            if (output.Length + text.Length > 65_536)
                throw new EvaluationLimitException("output", "LaTeX expression is too large.");
            output.Append(text);
        }

        private void Count(int depth) {
            if (++nodes > 8_192 || depth > 192)
                throw new EvaluationLimitException("output", "LaTeX expression has too many nodes.");
        }

        private static bool IsCall(Symbol value, string name, int arity) =>
            value is Expression e && e.Head is StringSymbol s &&
            s.Name == name && e.Arguments.Count == arity;

        private static int Precedence(Symbol value) {
            if (value is Constant c && c.Value < 0) return 30;
            if (value is not Expression e || e.Head is not StringSymbol s) return 60;
            return s.Name switch {
                "Plus" when e.Arguments.Count >= 2 => 10,
                "Times" when e.Arguments.Count >= 2 => IsNegative(value) ? 30 : 20,
                "Power" when e.Arguments.Count == 2 => 40,
                "Factorial" when e.Arguments.Count == 1 => 50,
                _ => 60
            };
        }

        private static bool IsNegative(Symbol value) =>
            value is Constant c && c.Value < 0 ||
            IsCall(value, "Times", 2) && ((Expression)value).Arguments[0] is Constant { Value: -1 };

        public void Visit(Symbol value, int parentPrecedence, int depth) {
            Count(depth);
            var parentheses = Precedence(value) < parentPrecedence;
            if (parentheses) Append(@"\left(");
            if (value is Constant c) Append(c.Value.ToString(CultureInfo.InvariantCulture));
            else if (value is StringSymbol s) Name(s.Name, false);
            else WriteExpression((Expression)value, depth);
            if (parentheses) Append(@"\right)");
        }

        private void Name(string name, bool function) {
            if (!function && name.Length == 1 && char.IsAsciiLetter(name[0])) {
                Append(name);
                return;
            }
            Append(function ? @"\operatorname{" : @"\text{");
            foreach (var character in name) {
                Append(character switch {
                    '\\' => @"\textbackslash{}",
                    '{' => @"\{", '}' => @"\}", '_' => @"\_", '$' => @"\$",
                    '%' => @"\%", '#' => @"\#", '&' => @"\&",
                    '^' => @"\textasciicircum{}", '~' => @"\textasciitilde{}",
                    _ => char.IsControl(character) ? " " : character.ToString()
                });
            }
            Append("}");
        }

        private void WriteExpression(Expression expression, int depth) {
            var args = expression.Arguments;
            var name = (expression.Head as StringSymbol)?.Name;
            void Child(int index, int precedence = 0) => Visit(args[index], precedence, depth + 1);
            void Joined(string separator, int precedence = 0) {
                for (var i = 0; i < args.Count; i++) {
                    if (i > 0) Append(separator);
                    Child(i, precedence);
                }
            }

            switch (name) {
                case "Plus" when args.Count >= 2:
                    Child(0);
                    for (var i = 1; i < args.Count; i++) {
                        if (IsNegative(args[i])) {
                            Append(" - ");
                            Count(depth + 1);
                            if (args[i] is Constant number)
                                Append(number.Value.ToString(CultureInfo.InvariantCulture)[1..]);
                            else Visit(((Expression)args[i]).Arguments[1], 31, depth + 2);
                        } else {
                            Append(" + ");
                            Child(i, 11);
                        }
                    }
                    break;
                case "Times" when args.Count >= 2:
                    if (IsNegative(expression)) { Append("-"); Child(1, 31); }
                    else Joined(@" \cdot ", 21);
                    break;
                case "Divide" when args.Count == 2:
                    Append(@"\frac{"); Child(0); Append("}{"); Child(1); Append("}");
                    break;
                case "Power" when args.Count == 2:
                    Append("{"); Child(0, 41); Append("}^{"); Child(1); Append("}");
                    break;
                case "List":
                    Append(@"\left["); Joined(@",\,"); Append(@"\right]");
                    break;
                case "Sin" when args.Count == 1:
                    Append(@"\sin\left("); Child(0); Append(@"\right)");
                    break;
                case "Factorial" when args.Count == 1:
                    Child(0, 51); Append("!");
                    break;
                default:
                    if (expression.Head is StringSymbol head) Name(head.Name, true);
                    else { Append(@"\left("); Visit(expression.Head, 0, depth + 1); Append(@"\right)"); }
                    Append(@"\left("); Joined(@",\,"); Append(@"\right)");
                    break;
            }
        }
    }
}
