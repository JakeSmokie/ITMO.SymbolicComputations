using System;
using System.Threading;
using ITMO.SymbolicComputations.Workbench;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.SymbolicComputations.Web.Controllers;

[ApiController]
[Route("api")]
public sealed class WorkbenchController : ControllerBase {
    public sealed record EvaluationRequest(string Expression);
    [HttpGet("health")]
    public object Health() => new { status = "ok", engine = "ITMO.SymbolicComputations · .NET 10 backend" };
    [HttpGet("examples")]
    public object Examples() => WorkbenchService.Examples;
    [HttpGet("functions")]
    public object Functions() => WorkbenchService.Functions;
    [HttpPost("fractal")]
    [RequestSizeLimit(32_768)]
    public IActionResult Fractal([FromBody] MandelbrotService.FractalRequest request, CancellationToken cancellationToken) {
        var response = MandelbrotService.Render(request, cancellationToken);
        return StatusCode(response.Status, response.Data);
    }
    [HttpPost("orbit")]
    [RequestSizeLimit(32_768)]
    public IActionResult Orbit([FromBody] MandelbrotService.OrbitRequest request, CancellationToken cancellationToken) {
        var response = MandelbrotService.Orbit(request, cancellationToken);
        return StatusCode(response.Status, response.Data);
    }
    [HttpPost("evaluate")]
    [RequestSizeLimit(32_768)]
    public IActionResult Evaluate([FromBody] EvaluationRequest request, CancellationToken cancellationToken) {
        var response = WorkbenchService.Evaluate(request?.Expression, cancellationToken);
        return StatusCode(response.Status, response.Data);
    }
}
