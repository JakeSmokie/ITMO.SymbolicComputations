using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Parsing;
using Xunit;

namespace ITMO.SymbolicComputations.Workbench.Tests;

public sealed class LatexFormatterTests {
    [Theory]
    [InlineData("(2 + 3)^2 / 5", @"\frac{{\left(2 + 3\right)}^{2}}{5}")]
    [InlineData("(x + y) * z", @"\left(x + y\right) \cdot z")]
    [InlineData("(x^y)^z", @"{\left({x}^{y}\right)}^{z}")]
    [InlineData("x^(y^z)", @"{x}^{{y}^{z}}")]
    [InlineData("(-x)^2", @"{\left(-x\right)}^{2}")]
    [InlineData("-x^2", @"-{x}^{2}")]
    [InlineData("x - (y + z)", @"x - \left(y + z\right)")]
    [InlineData("x / (y / z)", @"\frac{x}{\frac{y}{z}}")]
    [InlineData("{1, x^2}", @"\left[1,\,{x}^{2}\right]")]
    [InlineData("{}", @"\left[\right]")]
    [InlineData("Sin(x)", @"\sin\left(x\right)")]
    [InlineData("Factorial(5)", "5!")]
    [InlineData("Factorial(Factorial(5))", @"\left(5!\right)!")]
    [InlineData("-(-x)", @"-\left(-x\right)")]
    [InlineData("foo_bar(x_1)", @"\operatorname{foo\_bar}\left(\text{x\_1}\right)")]
    [InlineData("Sin(x, y)", @"\operatorname{Sin}\left(x,\,y\right)")]
    [InlineData("Plus()", @"\operatorname{Plus}\left(\right)")]
    public void PreservesGroupingAndDoesNotInventUnsupportedOperations(string formula, string expected) =>
        Assert.Equal(expected, LatexFormatter.Format(SymbolicParser.Parse(formula)));

    [Fact]
    public void DecimalOutputIsIndependentOfLocale() {
        var previous = CultureInfo.CurrentCulture;
        try {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Assert.Equal("1.25", LatexFormatter.Format((Symbol)1.25m));
        } finally { CultureInfo.CurrentCulture = previous; }
    }

    [Fact]
    public void SymbolNamesCannotInjectTexCommands() {
        var result = LatexFormatter.Format(new StringSymbol(@"\href{x}{y}_%$&#^~"));
        Assert.DoesNotContain(@"\href", result);
        Assert.Contains(@"\textbackslash{}href\{x\}\{y\}\_\%\$\&\#", result);
    }

    [Fact]
    public void ExcessiveOutputAndDepthAreRejected() {
        Assert.Throws<EvaluationLimitException>(() =>
            LatexFormatter.Format(new StringSymbol(new string('_', 40_000))));
        Symbol nested = new StringSymbol("x");
        for (var i = 0; i < 200; i++)
            nested = new Expression(new StringSymbol("f"), ImmutableList.Create(nested));
        Assert.Throws<EvaluationLimitException>(() => LatexFormatter.Format(nested));
    }

    [Fact]
    public void LatexMatchesTheActualEngineResultAndTrace() {
        var reply = WorkbenchService.Evaluate("(2 + 3)^2 / 5");
        Assert.Equal(200, reply.Status);
        var json = JsonSerializer.SerializeToElement(reply.Data);
        Assert.Equal("5", json.GetProperty("resultLatex").GetString());
        Assert.Equal(@"\frac{{\left(2 + 3\right)}^{2}}{5}", json.GetProperty("inputLatex").GetString());
        Assert.Equal(json.GetProperty("steps").GetArrayLength(), json.GetProperty("stepsLatex").GetArrayLength());
        Assert.Equal("5", json.GetProperty("stepsLatex").EnumerateArray().Last().GetString());
    }
}
