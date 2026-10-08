using System.Text.Json;
using ITMO.SymbolicComputations.Workbench;
using Microsoft.JSInterop;

namespace ITMO.SymbolicComputations.Browser;

/// <summary>The same workbench contract as ASP.NET, dispatched entirely inside browser .NET.</summary>
public static class BrowserApi {
    private const int MaximumRequestLength = 32_768;
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        MaxDepth = 256
    };

    private sealed record EvaluationRequest(string? Expression);

    [JSInvokable("SymbolicRequest")]
    public static string Request(string path, string method, string? body) {
        var response = Dispatch(path, method, body);
        return JsonSerializer.Serialize(response, JsonOptions);
    }

    private static WorkbenchService.Reply Dispatch(string path, string method, string? body) {
        var normalizedPath = path.Split('?', '#')[0].TrimEnd('/');
        if (normalizedPath.StartsWith("/lab/", StringComparison.Ordinal)) {
            normalizedPath = normalizedPath[4..];
        }

        var expectedMethod = normalizedPath switch {
            "/api/health" or "/api/examples" or "/api/functions" => "GET",
            "/api/evaluate" or "/api/fractal" or "/api/orbit" => "POST",
            _ => null
        };

        if (expectedMethod == null) {
            return Error(404, "Неизвестный путь лаборатории.", "not_found");
        }

        if (!string.Equals(method, expectedMethod, StringComparison.OrdinalIgnoreCase)) {
            return Error(405, $"Для этого запроса используется {expectedMethod}.", "method_not_allowed");
        }

        if (normalizedPath == "/api/health") {
            return new(200, new {
                status = "ok",
                engine = "ITMO.SymbolicComputations · C# WebAssembly (.NET 10)",
                runtime = "browser-wasm",
                execution = "main-thread",
                maxEvaluationMs = (int) WorkbenchService.Limits().MaxDuration.TotalMilliseconds,
                note = "Вычисление выполняется в браузере тем же C# движком. Сложный запрос может временно задержать интерфейс."
            });
        }

        if (normalizedPath == "/api/examples") return new(200, WorkbenchService.Examples);
        if (normalizedPath == "/api/functions") return new(200, WorkbenchService.Functions);

        if (body == null || body.Length > MaximumRequestLength) {
            return Error(body == null ? 400 : 413,
                body == null ? "Отсутствует тело запроса." : "Тело запроса слишком большое.",
                body == null ? "invalid_json" : "request_too_large");
        }

        try {
            if (normalizedPath == "/api/fractal") {
                return MandelbrotService.Render(JsonSerializer.Deserialize<MandelbrotService.FractalRequest>(body, JsonOptions));
            }
            if (normalizedPath == "/api/orbit") {
                return MandelbrotService.Orbit(JsonSerializer.Deserialize<MandelbrotService.OrbitRequest>(body, JsonOptions));
            }
            var request = JsonSerializer.Deserialize<EvaluationRequest>(body, JsonOptions);
            return WorkbenchService.Evaluate(request?.Expression ?? string.Empty, CancellationToken.None);
        }
        catch (JsonException) {
            return Error(400, "Ожидался JSON с корректными полями запроса и конечными числами.", "invalid_json");
        }
    }

    private static WorkbenchService.Reply Error(int status, string error, string code) =>
        new(status, new { error, code });
}
