# Browser host

This project compiles the same `ITMO.SymbolicComputations.Workbench` and Base engine to managed .NET WebAssembly. There is no JavaScript evaluator and no remote compute call.

```powershell
dotnet restore ITMO.SymbolicComputations.Browser/ITMO.SymbolicComputations.Browser.csproj
dotnet publish ITMO.SymbolicComputations.Browser/ITMO.SymbolicComputations.Browser.csproj -c Release --no-restore
```

Copy the **contents** of `bin/Release/net10.0/publish/wwwroot` into the site's `/lab/` directory. The host HTML must contain `<base href="/lab/">`. The included index is only a runtime smoke page and may be replaced by the workbench HTML.

For a localhost check, run `python ITMO.SymbolicComputations.Browser/serve-smoke.py`, then open `http://127.0.0.1:5219/lab/`. This helper serves static files only; the page's test calculation still executes in browser WebAssembly.

Before the existing interface's `app.js`, load:

```html
<script src="symbolic-transport.js"></script>
```

The bridge loads `blazor.webassembly.js` with `autostart=false`, starts .NET, and checks health through the compiled C# assembly. `await window.symbolicReady` resolves to that health reply. `await window.symbolicTransport(path, options)` returns `{ ok, status, json(), text() }` for the existing `/api/health`, `/api/examples`, `/api/functions`, and `/api/evaluate` routes. No network request is made for evaluation.

Names are case-sensitive. Evaluation uses the shared workbench's resource limits, including its three-second cooperative time budget. The managed runtime executes on the main browser thread; a difficult formula can delay the interface until the bounded evaluation returns. AbortSignal is checked before/after invocation, but it cannot interrupt synchronous C# work while the browser thread is busy. This host does not claim worker isolation.

AOT and native builds are disabled. Trimming is disabled so reflected builtin symbols and reflection-based JSON serialization are retained. No wasm-tools workload is intended for this managed publish path.
