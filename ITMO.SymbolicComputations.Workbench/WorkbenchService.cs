using System.Diagnostics;
using System.Globalization;
using System.Text;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Parsing;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Workbench;

public static class WorkbenchService {
    public sealed record Reply(int Status, object Data);
    public sealed record AstNode(string Kind, string Value, AstNode[] Children);
    public sealed record Example(string Id, string Title, string Description, string Expression);
    public sealed record Function(string Name, string Category, string Syntax, string Description);
    public static EvaluationOptions Limits() => new() {
        MaxDepth = 192, MaxNodeVisits = 200_000, MaxSteps = 20_000,
        MaxListLength = 512, MaxDuration = TimeSpan.FromSeconds(3)
    };
    public static readonly Example[] Examples = {
        new("arithmetic", "Арифметика", "Скобки, приоритет операций и степень", "(2 + 3)^2 / 5"),
        new("symbolic", "Символьное выражение", "Числа и неизвестная x остаются в дереве формулы", "Plus(x, 2, 3)"),
        new("factorial", "Рекурсивная функция", "Факториал задан средствами самого движка", "Factorial(5)"),
        new("map", "Квадраты списка", "Map применяет пользовательскую функцию к элементам", "Map({1, 2, 3})(Fun(x, x^2))"),
        new("function", "Своя функция", "Определение функции и вызов в одной последовательности", "Seq(SetDelayed(square, Fun(x, x^2)), square(7))"),
        new("hold", "Удержать вычисление", "Hold сохраняет выражение для исследования дерева", "Hold(2 + 3 * x)"),
        new("distinct", "Сравнить выражения", "Удаление структурно одинаковых элементов списка", "Distinct({f(x), f(x), f(y)})"),
        new("sine", "Синус", "Численное значение; не произвольная точность", "Sin(1)")
    };
    public static readonly Function[] Functions = {
        new("Plus", "Арифметика", "Plus(1, 2, x)", "Сумма. Запись a + b эквивалентна Plus(a, b)."),
        new("Times", "Арифметика", "Times(2, 3, x)", "Произведение. Используй *: неявное умножение не поддерживается."),
        new("Divide", "Арифметика", "Divide(8, 2)", "Деление десятичных чисел. Деление на ноль — ошибка."),
        new("Power", "Арифметика", "Power(2, 10)", "Степень, также a^b. Вычисляется через double; не точная рациональная арифметика."),
        new("Sin", "Арифметика", "Sin(1)", "Синус в радианах, с ограниченной численной точностью."),
        new("Factorial", "Арифметика", "Factorial(5)", "Рекурсивная функция стандартной библиотеки для неотрицательных целых."),
        new("List", "Списки", "{1, 2, 3}", "Список, также List(1, 2, 3)."),
        new("Map", "Списки", "Map({1, 2, 3})(Fun(x, x^2))", "Применяет функцию к каждому элементу. Два последовательных вызова."),
        new("Length", "Списки", "Length({1, 2, 3})", "Количество элементов списка."),
        new("Part", "Списки", "Part({10, 20, 30}, 1)", "Элемент по индексу. Первый индекс — 0."),
        new("Distinct", "Списки", "Distinct({f(x), f(x), f(y)})", "Убирает структурно одинаковые элементы."),
        new("GenerateList", "Списки", "GenerateList(5)", "Последовательность 0, 1, …, n − 1. В лаборатории не более 512 элементов."),
        new("Fun", "Функции", "Fun(x, x^2)(4)", "Анонимная функция: аргумент, затем тело. Регистр имён имеет значение."),
        new("Set", "Функции", "Seq(Set(x, 5), x + 2)", "Присваивание в пределах одного запроса. Между запросами состояние не сохраняется."),
        new("SetDelayed", "Функции", "Seq(SetDelayed(f, Fun(x, x + 1)), f(4))", "Задаёт отложенное правило или свою функцию."),
        new("Seq", "Функции", "Seq(Set(x, 5), x + 2)", "Вычисляет последовательность выражений."),
        new("Hold", "Управление", "Hold(2 + 3)", "Удерживает аргументы от обычного вычисления."),
        new("If", "Управление", "If(Eq(2, 2), 10, 20)", "Выбирает ветвь по условию."),
        new("Eq", "Управление", "Eq(2, 2)", "Проверяет равенство выражений.")
    };
    public static Reply Evaluate(string expression, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(expression)) return Error(400, "Введи выражение для вычисления.", "empty_input", 0);
        try {
            var timer = Stopwatch.StartNew();
            var input = SymbolicParser.Parse(expression);
            var (steps, result) = new SymbolicContext().Run(input, Limits(), cancellationToken);
            var trace = new List<string>();
            var traceLatex = new List<string>();
            var traceCharacters = 0;
            var unique = steps.Add(result).WithoutDuplicates();
            foreach (var step in unique) {
                if (trace.Count == 256) break;
                var printed = Print(step);
                var latex = LatexFormatter.Format(step);
                if (traceCharacters + printed.Length + latex.Length > 200_000) break;
                trace.Add(printed); traceLatex.Add(latex); traceCharacters += printed.Length + latex.Length;
            }
            var count = 0;
            var ast = Tree(input, ref count);
            return new Reply(200, new {
                input = Print(input), result = Print(result),
                inputLatex = LatexFormatter.Format(input), resultLatex = LatexFormatter.Format(result), stepsLatex = traceLatex,
                steps = trace, ast, elapsedMs = Math.Round(timer.Elapsed.TotalMilliseconds, 2),
                stepCount = trace.Count, totalStepCount = unique.Count, nodeCount = count,
                traceTruncated = trace.Count < unique.Count
            });
        }
        catch (FormulaParseException e) { return Error(400, e.Message, "parse_error", e.Position); }
        catch (EvaluationLimitException e) { return Error(422, "Вычисление остановлено: достигнут лимит " + e.Limit + ". Уменьши выражение или размер списка.", "evaluation_limit"); }
        catch (OperationCanceledException) { return Error(408, "Вычисление отменено.", "cancelled"); }
        catch (DivideByZeroException) { return Error(422, "Деление на ноль не определено.", "division_by_zero"); }
        catch (OverflowException) { return Error(422, "Число выходит за диапазон decimal или результат не является конечным действительным числом.", "numeric_range"); }
        catch (ArgumentException e) { return Error(422, e.Message, "invalid_argument"); }
        catch (ArithmeticException e) { return Error(422, e.Message, "numeric_domain"); }
        catch (IndexOutOfRangeException) { return Error(422, "Индекс или число аргументов выходит за допустимые границы.", "invalid_argument"); }
    }
    private static Reply Error(int status, string error, string code, int? position = null) => new(status, new { error, code, position });
    // Enforce response bounds while printing, before allocating a potentially enormous string.
    private static string Print(Symbol symbol) {
        var output = new StringBuilder();
        var nodes = 0;
        void Append(string text) {
            if (output.Length + text.Length > 65_536)
                throw new EvaluationLimitException("output", "Printed expression is too large.");
            output.Append(text);
        }
        void Visit(Symbol value, int depth) {
            if (++nodes > 8_192 || depth > 192)
                throw new EvaluationLimitException("output", "Printed expression has too many nodes.");
            if (value is Constant c) { Append(c.Value.ToString(CultureInfo.InvariantCulture)); return; }
            if (value is StringSymbol s) { Append(s.Name); return; }
            var e = (Expression)value;
            var sequence = Equals(e.Head, Base.StandardLibrary.Functions.Seq);
            Visit(e.Head, depth + 1);
            Append(sequence ? "[\n" : "[");
            for (var i = 0; i < e.Arguments.Count; i++) {
                if (i > 0) Append(sequence ? ", \n" : ", ");
                if (sequence) Append("    ");
                Visit(e.Arguments[i], depth + 1);
            }
            Append(sequence ? "\n]" : "]");
        }
        Visit(symbol, 0);
        return output.ToString();
    }
    private static AstNode Tree(Symbol symbol, ref int count) {
        count++;
        if (symbol is Constant constant) return new("constant", constant.Value.ToString(CultureInfo.InvariantCulture), Array.Empty<AstNode>());
        if (symbol is StringSymbol name) return new("symbol", name.Name, Array.Empty<AstNode>());
        var expression = (Expression)symbol;
        var children = new List<AstNode>();
        if (expression.Head is not StringSymbol) children.Add(Tree(expression.Head, ref count));
        foreach (var argument in expression.Arguments) children.Add(Tree(argument, ref count));
        return new("expression", expression.Head is StringSymbol head ? head.Name : "Применение функции", children.ToArray());
    }
}
