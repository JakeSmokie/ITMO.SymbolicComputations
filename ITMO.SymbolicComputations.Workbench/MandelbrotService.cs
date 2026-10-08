using System.Buffers.Binary;
using System.Diagnostics;
using System.Globalization;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Parsing;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Workbench;

/// <summary>
/// A double-precision image renderer and a separate, genuine symbolic-engine orbit.
/// A bounded pixel means only that escape was not observed in the requested iterations.
/// </summary>
public static class MandelbrotService {
    private static readonly TimeSpan ImageBudget = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan OrbitBudget = TimeSpan.FromSeconds(2);

    public sealed class FractalRequest {
        public double CenterRe { get; init; } = -0.65;
        public double CenterIm { get; init; }
        public double Span { get; init; } = 3.2;
        public int Width { get; init; } = 480;
        public int Height { get; init; } = 320;
        public int MaxIterations { get; init; } = 160;
    }

    public sealed class OrbitRequest {
        public double Real { get; init; }
        public double Imag { get; init; }
        public int Iterations { get; init; } = 12;
    }

    public sealed record FractalResult(int Width, int Height, double CenterRe, double CenterIm,
        double Span, int MaxIterations, string ValuesBase64, double ElapsedMs);

    public sealed record OrbitResult(decimal Real, decimal Imag, string Formula, string RealFormula,
        string ImagFormula, OrbitPoint[] Points, bool Escaped, int? EscapeIteration, string Engine,
        double ElapsedMs);

    public sealed record OrbitPoint(int N, decimal Re, decimal Im, double Modulus, string? Expression,
        string? RealExpression, string? ImagExpression, string? RealResult, string? ImagResult);

    public sealed record ErrorResult(string Error, string Code);

    public static WorkbenchService.Reply Render(FractalRequest? request,
        CancellationToken cancellationToken = default) {
        if (request == null) return Error(400, "Ожидались параметры изображения.", "invalid_request");
        if (!double.IsFinite(request.CenterRe) || !double.IsFinite(request.CenterIm) ||
            !double.IsFinite(request.Span)) {
            return Error(400, "Центр и масштаб должны быть конечными числами.", "invalid_viewport");
        }
        if (request.Span <= 1e-12) {
            return Error(400, "Горизонтальный диапазон должен быть больше 10⁻¹².", "invalid_viewport");
        }
        if (request.Width < 1 || request.Width > 640 || request.Height < 1 || request.Height > 480) {
            return Error(400, "Ширина должна быть от 1 до 640, высота — от 1 до 480.", "invalid_dimensions");
        }
        if (request.MaxIterations < 1 || request.MaxIterations > 512) {
            return Error(400, "Число итераций изображения должно быть от 1 до 512.", "invalid_iterations");
        }

        var pixelSpan = request.Span / request.Width;
        var halfHeight = pixelSpan * request.Height / 2;
        if (!double.IsFinite(halfHeight) ||
            !double.IsFinite(request.CenterRe - request.Span / 2) ||
            !double.IsFinite(request.CenterRe + request.Span / 2) ||
            !double.IsFinite(request.CenterIm - halfHeight) ||
            !double.IsFinite(request.CenterIm + halfHeight)) {
            return Error(400, "Границы изображения выходят за диапазон конечных чисел.", "invalid_viewport");
        }

        var timer = Stopwatch.StartNew();
        try {
            CheckBudget(timer, ImageBudget, cancellationToken);
            var pixels = new byte[request.Width * request.Height * sizeof(float)];
            var offset = 0;
            var work = 0;
            for (var row = 0; row < request.Height; row++) {
                CheckBudget(timer, ImageBudget, cancellationToken);
                // Square pixels, sampled at their centers. Positive imaginary values are above the center.
                var ci = request.CenterIm + (request.Height / 2.0 - row - 0.5) * pixelSpan;
                for (var column = 0; column < request.Width; column++) {
                    var cr = request.CenterRe + (column + 0.5 - request.Width / 2.0) * pixelSpan;
                    double x = 0, y = 0;
                    var value = -1f;
                    for (var n = 1; n <= request.MaxIterations; n++) {
                        if ((work++ & 1023) == 0) CheckBudget(timer, ImageBudget, cancellationToken);
                        var nextX = x * x - y * y + cr;
                        y = 2 * x * y + ci;
                        x = nextX;
                        if (Math.Abs(x) > 2 || Math.Abs(y) > 2 || x * x + y * y > 4) {
                            // Calculate log(|z|) without squaring huge finite coordinates.
                            var largest = Math.Max(Math.Abs(x), Math.Abs(y));
                            var ratio = Math.Min(Math.Abs(x), Math.Abs(y)) / largest;
                            var logModulus = Math.Log(largest) + 0.5 * Math.Log(1 + ratio * ratio);
                            var smooth = n + 1 - Math.Log(logModulus) / Math.Log(2);
                            // Very distant points can have a negative smooth count. Reserve -1 for bounded pixels.
                            value = (float) Math.Max(0, smooth);
                            break;
                        }
                    }
                    BinaryPrimitives.WriteInt32LittleEndian(pixels.AsSpan(offset, sizeof(float)),
                        BitConverter.SingleToInt32Bits(value));
                    offset += sizeof(float);
                }
            }
            CheckBudget(timer, ImageBudget, cancellationToken);
            var encoded = Convert.ToBase64String(pixels);
            CheckBudget(timer, ImageBudget, cancellationToken);
            return new(200, new FractalResult(request.Width, request.Height, request.CenterRe,
                request.CenterIm, request.Span, request.MaxIterations, encoded,
                Math.Round(timer.Elapsed.TotalMilliseconds, 2)));
        }
        catch (EvaluationLimitException) {
            return Error(422, "Рендер остановлен после достижения лимита 3 секунды. Уменьши изображение или число итераций.", "evaluation_limit");
        }
        catch (OperationCanceledException) {
            return Error(422, "Рендер отменён.", "cancelled");
        }
    }

    public static WorkbenchService.Reply Orbit(OrbitRequest? request,
        CancellationToken cancellationToken = default) {
        if (request == null) return Error(400, "Ожидались координаты точки.", "invalid_request");
        if (!double.IsFinite(request.Real) || !double.IsFinite(request.Imag)) {
            return Error(400, "Координаты точки должны быть конечными числами.", "invalid_point");
        }
        if (request.Iterations < 1 || request.Iterations > 16) {
            return Error(400, "Число шагов орбиты должно быть от 1 до 16.", "invalid_iterations");
        }

        var timer = Stopwatch.StartNew();
        try {
            CheckBudget(timer, OrbitBudget, cancellationToken);
            var cr = AsDecimal(request.Real);
            var ci = AsDecimal(request.Imag);
            decimal x = 0, y = 0;
            var points = new List<OrbitPoint> {
                new(0, 0, 0, 0, "z[0] = 0", null, null, "0", "0")
            };
            int? escapedAt = null;
            var context = new SymbolicContext();
            for (var n = 1; n <= request.Iterations; n++) {
                CheckBudget(timer, OrbitBudget, cancellationToken);
                // The engine's Power currently passes through Math.Pow(double). Multiplication keeps this orbit decimal.
                var realExpression = $"({Number(x)})*({Number(x)})-({Number(y)})*({Number(y)})+({Number(cr)})";
                var imagExpression = $"2*({Number(x)})*({Number(y)})+({Number(ci)})";
                var input = SymbolicParser.Parse($"List({realExpression}, {imagExpression})");
                var (_, result) = context.Run(input, new EvaluationOptions {
                    MaxDepth = 192,
                    MaxNodeVisits = 50_000,
                    MaxSteps = 4_000,
                    MaxListLength = 128,
                    MaxDuration = Remaining(timer, OrbitBudget)
                }, cancellationToken);
                CheckBudget(timer, OrbitBudget, cancellationToken);
                if (result is not Expression { Arguments.Count: 2 } list ||
                    !Equals(list.Head, ListFunctions.List) ||
                    list.Arguments[0] is not Constant real || list.Arguments[1] is not Constant imag) {
                    return Error(422, "Движок не вернул численную пару координат.", "unexpected_engine_result");
                }
                x = real.Value;
                y = imag.Value;
                var realPrinted = real.Visit(MathematicaPrinter.Default);
                var imagPrinted = imag.Visit(MathematicaPrinter.Default);
                var modulus = Math.Sqrt((double) x * (double) x + (double) y * (double) y);
                points.Add(new(n, x, y, modulus,
                    $"Re(z[{n}]) = {realExpression} = {realPrinted}; Im(z[{n}]) = {imagExpression} = {imagPrinted}",
                    realExpression, imagExpression, realPrinted, imagPrinted));
                // Compare in decimal when safe, preserving the strict radius boundary for the decimal orbit.
                if (Math.Abs(x) > 2 || Math.Abs(y) > 2 || x * x + y * y > 4) {
                    escapedAt = n;
                    break;
                }
            }
            CheckBudget(timer, OrbitBudget, cancellationToken);
            return new(200, new OrbitResult(cr, ci, "z[n+1] = z[n]^2 + c",
                "x[n+1] = x[n]^2 - y[n]^2 + Re(c)",
                "y[n+1] = 2*x[n]*y[n] + Im(c)", points.ToArray(), escapedAt.HasValue,
                escapedAt, "ITMO.SymbolicComputations · SymbolicParser → AST → SymbolicContext / FullEvaluator; decimal multiplication",
                Math.Round(timer.Elapsed.TotalMilliseconds, 2)));
        }
        catch (EvaluationLimitException) {
            return Error(422, "Вычисление орбиты остановлено по лимиту ресурсов или времени (2 секунды).", "evaluation_limit");
        }
        catch (OperationCanceledException) {
            return Error(422, "Вычисление орбиты отменено.", "cancelled");
        }
        catch (OverflowException) {
            return Error(422, "Координата или промежуточное значение выходит за диапазон decimal.", "numeric_range");
        }
        catch (FormulaParseException) {
            return Error(422, "Движок не смог разобрать численное выражение орбиты.", "engine_parse_error");
        }
    }

    private static decimal AsDecimal(double value) {
        // Preserve the round-trip decimal spelling of the supplied double instead of the cast's 15-digit rounding.
        var result = decimal.Parse(value.ToString("R", CultureInfo.InvariantCulture),
            NumberStyles.Float, CultureInfo.InvariantCulture);
        if (value != 0 && result == 0) throw new OverflowException("Coordinate underflows decimal.");
        return result;
    }

    private static string Number(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    private static TimeSpan Remaining(Stopwatch timer, TimeSpan maximum) {
        var remaining = maximum - timer.Elapsed;
        if (remaining <= TimeSpan.Zero) throw new EvaluationLimitException("duration", "Time budget exceeded.");
        return remaining;
    }

    private static void CheckBudget(Stopwatch timer, TimeSpan maximum, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        _ = Remaining(timer, maximum);
    }

    private static WorkbenchService.Reply Error(int status, string error, string code) =>
        new(status, new ErrorResult(error, code));
}
