using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using ITMO.SymbolicComputations.Workbench;
using Xunit;

namespace ITMO.SymbolicComputations.Workbench.Tests;

public sealed class MandelbrotTests {
    private static readonly JsonSerializerOptions JsonOptions = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
    [Theory]
    [InlineData(0d)]
    [InlineData(-1d)]
    public void KnownBoundedRealOrbitsIncludeZ0AndTwelveIterations(double real) {
        var data = Orbit(real, 0);
        Assert.False(data.GetProperty("escaped").GetBoolean());
        Assert.Equal(JsonValueKind.Null, data.GetProperty("escapeIteration").ValueKind);
        var points = data.GetProperty("points").EnumerateArray().ToArray();
        Assert.Equal(13, points.Length);
        for (var n = 0; n < points.Length; n++) {
            Assert.Equal(n, points[n].GetProperty("n").GetInt32());
            Assert.Equal(real == 0 || n % 2 == 0 ? 0m : -1m, points[n].GetProperty("re").GetDecimal());
            Assert.Equal(0m, points[n].GetProperty("im").GetDecimal());
        }
    }

    [Fact]
    public void OneEscapesAtIterationThreeNotAtTheRadiusTwoBoundary() {
        var data = Orbit(1, 0);
        Assert.True(data.GetProperty("escaped").GetBoolean());
        Assert.Equal(3, data.GetProperty("escapeIteration").GetInt32());
        var points = data.GetProperty("points").EnumerateArray().ToArray();
        Assert.Equal(new[] {0m, 1m, 2m, 5m}, points.Select(p => p.GetProperty("re").GetDecimal()));
        Assert.All(points, p => Assert.Equal(0m, p.GetProperty("im").GetDecimal()));
        Assert.Equal(5d, points[^1].GetProperty("modulus").GetDouble(), 12);
        Assert.All(points.Skip(1), p => Assert.False(string.IsNullOrWhiteSpace(p.GetProperty("expression").GetString())));
    }

    [Fact]
    public void ImaginaryUnitOrbitUsesTheFullComplexSquare() {
        var data = Orbit(0, 1);
        var points = data.GetProperty("points").EnumerateArray().ToArray();
        Assert.False(data.GetProperty("escaped").GetBoolean());
        var expected = new[] {(0m, 0m), (0m, 1m), (-1m, 1m), (0m, -1m), (-1m, 1m)};
        for (var n = 0; n < expected.Length; n++) {
            Assert.Equal(expected[n].Item1, points[n].GetProperty("re").GetDecimal());
            Assert.Equal(expected[n].Item2, points[n].GetProperty("im").GetDecimal());
        }
    }

    [Fact]
    public void FractionalOrbitPreservesDecimalArithmeticAndDisplayedResults() {
        const decimal cr = 0.123456789012345m, ci = 0.234567890123456m;
        var data = Success(MandelbrotService.Orbit(new MandelbrotService.OrbitRequest {
            Real = (double) cr, Imag = (double) ci, Iterations = 3
        }));
        var points = data.GetProperty("points").EnumerateArray().ToArray();
        Assert.Equal(4, points.Length);
        decimal x = 0, y = 0;
        for (var n = 1; n <= 3; n++) {
            var nextX = x * x - y * y + cr;
            var nextY = 2m * x * y + ci;
            x = nextX;
            y = nextY;
            var actualRe = points[n].GetProperty("re").GetDecimal();
            var actualIm = points[n].GetProperty("im").GetDecimal();
            // Decimal reassociation may change final rounding, but a double implementation
            // loses far more than this tolerance with these non-binary-exact coordinates.
            Assert.InRange(Math.Abs(actualRe - x), 0m, 0.0000000000000000000000001m);
            Assert.InRange(Math.Abs(actualIm - y), 0m, 0.0000000000000000000000001m);
            Assert.Equal(actualRe.ToString(CultureInfo.InvariantCulture), points[n].GetProperty("realResult").GetString());
            Assert.Equal(actualIm.ToString(CultureInfo.InvariantCulture), points[n].GetProperty("imagResult").GetString());
        }
    }

    [Fact]
    public void TinyAsymmetricImageMatchesAnIndependentComplexNumberReference() {
        const int width = 5, height = 3, iterations = 40;
        const double centerRe = -0.4, centerIm = 0.2, span = 4;
        var data = Render(centerRe, centerIm, span, width, height, iterations);
        Assert.Equal(width, data.GetProperty("width").GetInt32());
        Assert.Equal(height, data.GetProperty("height").GetInt32());
        Assert.Equal(iterations, data.GetProperty("maxIterations").GetInt32());
        var actual = DecodeValues(data, width * height);
        var expectedEscaped = 0;
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                // Pixel centers, equal real/imaginary scale, and positive imaginary direction upward.
                var c = new Complex(centerRe + (x + 0.5 - width / 2d) * span / width,
                    centerIm + (height / 2d - y - 0.5) * span / width);
                var expected = ReferenceEscape(c, iterations);
                Assert.True(float.IsFinite(actual[y * width + x]));
                if (expected < 0) {
                    Assert.Equal(-1f, actual[y * width + x]);
                }
                else {
                    expectedEscaped++;
                    Assert.InRange(Math.Abs(actual[y * width + x] - expected), 0, 0.00001);
                }
            }
        }
        Assert.InRange(expectedEscaped, 1, width * height - 1);
        Assert.Equal(expectedEscaped, actual.Count(value => value >= 0));
    }

    [Fact]
    public void ImageRespectsComplexConjugateSymmetry() {
        const int width = 11, height = 8;
        var values = DecodeValues(Render(-0.5, 0, 3.5, width, height, 64), width * height);
        for (var y = 0; y < height / 2; y++) {
            for (var x = 0; x < width; x++) {
                Assert.Equal(values[y * width + x], values[(height - 1 - y) * width + x]);
            }
        }
    }

    [Theory]
    [InlineData(0d, -1d)]
    [InlineData(-1d, -1d)]
    [InlineData(1d, 3d)]
    public void SinglePixelSamplesItsCenterAndUsesTheNonEscapeSentinel(double center, double escape) {
        var values = DecodeValues(Render(center, 0, 1, 1, 1, 12), 1);
        if (escape < 0) Assert.Equal(-1f, values[0]);
        else Assert.InRange(Math.Abs(values[0] - ReferenceEscape(new Complex(center, 0), 12)), 0, 0.00001);
    }

    [Fact]
    public void NonEscapeSentinelDescribesTheIterationBudgetNotMembership() {
        // c = 1 escapes on iteration three, so the two-iteration image must still mark it unescaped.
        Assert.Equal(-1f, DecodeValues(Render(1, 0, 1, 1, 1, 2), 1)[0]);
        Assert.True(DecodeValues(Render(1, 0, 1, 1, 1, 3), 1)[0] >= 0);
    }

    [Theory]
    [InlineData(0, 10, 32)]
    [InlineData(10, 0, 32)]
    [InlineData(-1, 10, 32)]
    [InlineData(641, 1, 32)]
    [InlineData(1, 481, 32)]
    [InlineData(int.MaxValue, int.MaxValue, 32)]
    [InlineData(10, 10, 0)]
    [InlineData(10, 10, -1)]
    [InlineData(10, 10, int.MaxValue)]
    public void InvalidImageDimensionsAndIterationLimitsAreRejected(int width, int height, int iterations) {
        AssertRejected(MandelbrotService.Render(new MandelbrotService.FractalRequest {
            CenterRe = 0, CenterIm = 0, Span = 4, Width = width, Height = height, MaxIterations = iterations
        }));
    }

    [Theory]
    [InlineData(double.NaN, 0d, 4d)]
    [InlineData(double.PositiveInfinity, 0d, 4d)]
    [InlineData(0d, double.NegativeInfinity, 4d)]
    [InlineData(0d, 0d, double.NaN)]
    [InlineData(0d, 0d, double.PositiveInfinity)]
    [InlineData(0d, 0d, 0d)]
    [InlineData(0d, 0d, -1d)]
    public void NonFiniteCoordinatesAndInvalidSpansAreRejected(double real, double imaginary, double span) {
        AssertRejected(MandelbrotService.Render(new MandelbrotService.FractalRequest {
            CenterRe = real, CenterIm = imaginary, Span = span, Width = 8, Height = 8, MaxIterations = 32
        }));
    }

    [Theory]
    [InlineData(double.NaN, 0d)]
    [InlineData(double.PositiveInfinity, 0d)]
    [InlineData(0d, double.NegativeInfinity)]
    public void OrbitRejectsNonFiniteCoordinates(double real, double imaginary) {
        AssertRejected(MandelbrotService.Orbit(new MandelbrotService.OrbitRequest {Real = real, Imag = imaginary}));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(17)]
    public void OrbitTraceHasAnExplicitIterationLimit(int iterations) =>
        AssertRejected(MandelbrotService.Orbit(new MandelbrotService.OrbitRequest {Iterations = iterations}));

    [Fact]
    public void MissingRequestsAreRejected() {
        AssertRejected(MandelbrotService.Render(null));
        AssertRejected(MandelbrotService.Orbit(null));
    }

    [Fact]
    public void CancellationIsReportedForBothNumericalImageAndDslOrbit() {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var image = MandelbrotService.Render(new MandelbrotService.FractalRequest {Width = 8, Height = 8}, cancellation.Token);
        var orbit = MandelbrotService.Orbit(new MandelbrotService.OrbitRequest(), cancellation.Token);
        Assert.Equal(422, image.Status);
        Assert.Equal(422, orbit.Status);
        Assert.Equal("cancelled", JsonSerializer.SerializeToElement(image.Data, JsonOptions).GetProperty("code").GetString());
        Assert.Equal("cancelled", JsonSerializer.SerializeToElement(orbit.Data, JsonOptions).GetProperty("code").GetString());
    }

    private static JsonElement Render(double real, double imaginary, double span, int width, int height, int iterations) =>
        Success(MandelbrotService.Render(new MandelbrotService.FractalRequest {
            CenterRe = real, CenterIm = imaginary, Span = span, Width = width, Height = height, MaxIterations = iterations
        }));

    private static JsonElement Orbit(double real, double imaginary) =>
        Success(MandelbrotService.Orbit(new MandelbrotService.OrbitRequest {Real = real, Imag = imaginary}));

    private static JsonElement Success(WorkbenchService.Reply reply) {
        Assert.True(reply.Status == 200, JsonSerializer.Serialize(reply.Data));
        return JsonSerializer.SerializeToElement(reply.Data, JsonOptions);
    }

    private static void AssertRejected(WorkbenchService.Reply reply) {
        Assert.InRange(reply.Status, 400, 499);
        var data = JsonSerializer.SerializeToElement(reply.Data, JsonOptions);
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("error").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("code").GetString()));
    }

    private static float[] DecodeValues(JsonElement data, int length) {
        var bytes = Convert.FromBase64String(data.GetProperty("valuesBase64").GetString()!);
        Assert.Equal(length * sizeof(float), bytes.Length);
        var values = new float[length];
        for (var i = 0; i < length; i++)
            values[i] = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(i * 4, 4)));
        return values;
    }

    private static double ReferenceEscape(Complex c, int iterations) {
        var z = Complex.Zero;
        for (var n = 1; n <= iterations; n++) {
            z = z * z + c;
            if (z.Magnitude > 2)
                return n + 1 - Math.Log(Math.Log(z.Magnitude)) / Math.Log(2);
        }
        return -1;
    }
}
