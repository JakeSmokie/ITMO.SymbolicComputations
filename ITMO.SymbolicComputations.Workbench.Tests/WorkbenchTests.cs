using System.Text.Json;
using ITMO.SymbolicComputations.Workbench;
using Xunit;

namespace ITMO.SymbolicComputations.Workbench.Tests;
public sealed class WorkbenchTests {
    [Theory]
    [InlineData("(2 + 3)^2 / 5", "5")]
    [InlineData("Factorial(5)", "120")]
    [InlineData("Map({1, 2, 3})(Fun(x, x^2))", "List[1, 4, 9]")]
    [InlineData("Distinct({f(x), f(x), f(y)})", "List[f[x], f[y]]")]
    public void PublishedExamplesExecute(string input, string expected) {
        var reply = WorkbenchService.Evaluate(input);
        Assert.Equal(200, reply.Status);
        var json = JsonSerializer.SerializeToElement(reply.Data);
        Assert.Equal(expected, json.GetProperty("result").GetString());
        Assert.True(json.GetProperty("steps").GetArrayLength() > 0);
    }
    [Fact]
    public void EveryVisibleExampleCompletesWithinWorkbenchLimits() {
        foreach (var example in WorkbenchService.Examples) {
            var reply = WorkbenchService.Evaluate(example.Expression);
            Assert.True(reply.Status == 200, example.Id + ": " + JsonSerializer.Serialize(reply.Data));
        }
    }
    [Theory]
    [InlineData("1 / 0", 422, "division_by_zero")]
    [InlineData("2 + )", 400, "parse_error")]
    [InlineData("GenerateList(1000000000)", 422, "evaluation_limit")]
    [InlineData("Fun(x, x(x))(Fun(x, x(x)))", 422, "evaluation_limit")]
    [InlineData("", 400, "empty_input")]
    public void InvalidOrUnboundedInputIsAnError(string input, int status, string code) {
        var reply = WorkbenchService.Evaluate(input);
        Assert.Equal(status, reply.Status);
        Assert.Equal(code, JsonSerializer.SerializeToElement(reply.Data).GetProperty("code").GetString());
        Assert.Equal(200, WorkbenchService.Evaluate("1 + 2").Status);
    }
    [Fact]
    public void ExpandedLongIdentifiersAreBoundedBeforeResponseAllocation() {
        var formula = "Fun(x, {x,x,x,x,x,x,x,x,x})(" + new string('z', 8000) + ")";
        var reply = WorkbenchService.Evaluate(formula);
        Assert.Equal(422, reply.Status);
        Assert.Contains("output", JsonSerializer.SerializeToElement(reply.Data).GetProperty("error").GetString());
    }
    [Fact]
    public void RequestsDoNotShareVariables() {
        WorkbenchService.Evaluate("Seq(Set(x, 5), x + 2)");
        var reply = WorkbenchService.Evaluate("x");
        Assert.Equal("x", JsonSerializer.SerializeToElement(reply.Data).GetProperty("result").GetString());
    }
}
