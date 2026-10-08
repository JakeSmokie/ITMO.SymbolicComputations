(() => {
  "use strict";

  if (window.symbolicTransport) return;

  const scriptUrl = document.currentScript && document.currentScript.src;
  const assetBase = new URL("./", scriptUrl || new URL("/lab/symbolic-transport.js", location.href));
  const assemblyName = "ITMO.SymbolicComputations.Browser";
  let evaluationQueue = Promise.resolve();
  const managedStarted = new Promise((resolve, reject) => {
    window.symbolicManagedStarted = name => {
      if (name === assemblyName) resolve();
      else reject(new Error("Запущена неожиданная сборка C# WebAssembly: " + name));
    };
  });

  function status(state, detail) {
    window.dispatchEvent(new CustomEvent("symbolic-runtime", { detail: { state, ...detail } }));
  }

  async function loadBlazor() {
    if (window.Blazor && typeof window.Blazor.start === "function") return;
    await new Promise((resolve, reject) => {
      const script = document.createElement("script");
      script.src = new URL("_framework/blazor.webassembly.js", assetBase).href;
      script.setAttribute("autostart", "false");
      script.onload = resolve;
      script.onerror = () => reject(new Error("Не удалось загрузить среду C# WebAssembly."));
      document.head.appendChild(script);
    });
  }

  async function call(path, method, body) {
    const serialized = await window.DotNet.invokeMethodAsync(assemblyName, "SymbolicRequest", path, method, body);
    return JSON.parse(serialized);
  }

  // This promise includes a round trip through the actual C# assembly, not merely script loading.
  window.symbolicReady = (async () => {
    status("loading", {});
    await loadBlazor();
    await window.Blazor.start();
    await managedStarted;
    const health = await call("/api/health", "GET", null);
    if (health.status !== 200) throw new Error("C# WebAssembly не прошёл проверку готовности.");
    status("ready", health.data);
    return health.data;
  })();

  // Retain a rejected promise for callers while avoiding an unhandled-rejection notification.
  window.symbolicReady.catch(error => status("error", { message: error.message }));

  window.symbolicTransport = async (path, options = {}) => {
    const signal = options.signal;
    const abortIfNeeded = () => {
      if (signal && signal.aborted) throw new DOMException("Запрос отменён.", "AbortError");
    };

    abortIfNeeded();
    const url = new URL(path, assetBase);
    if (url.origin !== location.origin) throw new TypeError("Доступны только локальные пути лаборатории.");
    const method = (options.method || "GET").toUpperCase();
    const body = options.body == null ? null : options.body;
    if (body !== null && typeof body !== "string") {
      throw new TypeError("Передайте тело запроса как JSON.stringify({ expression }).");
    }

    await window.symbolicReady;
    abortIfNeeded();

    const run = async () => {
      abortIfNeeded();
      // Let the interface display its busy state before synchronous managed evaluation begins.
      await new Promise(resolve => setTimeout(resolve, 0));
      abortIfNeeded();
      const reply = await call(url.pathname, method, body);
      abortIfNeeded();
      return {
        ok: reply.status >= 200 && reply.status < 300,
        status: reply.status,
        json: async () => reply.data,
        text: async () => JSON.stringify(reply.data)
      };
    };

    // The engine is bounded by WorkbenchService and runs on the main thread, not in a worker.
    const pending = evaluationQueue.then(run, run);
    evaluationQueue = pending.then(() => undefined, () => undefined);
    return pending;
  };
})();
