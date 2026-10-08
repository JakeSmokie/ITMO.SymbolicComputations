"""Serve published static WASM assets under a chosen path. No compute API."""

import argparse
import functools
import http.server
from pathlib import Path


class LabHandler(http.server.SimpleHTTPRequestHandler):
    extensions_map = {**http.server.SimpleHTTPRequestHandler.extensions_map, ".wasm": "application/wasm"}
    base_path = "/lab/"

    def do_GET(self):
        if self.path == self.base_path.rstrip("/"):
            self.send_response(301)
            self.send_header("Location", self.base_path)
            self.end_headers()
            return
        if not self.path.startswith(self.base_path):
            self.send_error(404)
            return
        super().do_GET()

    def translate_path(self, path):
        relative_path = "/" + path[len(self.base_path):] if path.startswith(self.base_path) else path
        return super().translate_path(relative_path)


if __name__ == "__main__":
    arguments = argparse.ArgumentParser()
    arguments.add_argument("--port", type=int, default=5219)
    arguments.add_argument("--base-path", default="/lab/")
    arguments.add_argument("--directory", type=Path, default=Path(__file__).parent / "bin/Release/net10.0/publish/wwwroot")
    options = arguments.parse_args()
    if not options.base_path.startswith("/") or not options.base_path.endswith("/"):
        arguments.error("--base-path must start and end with '/'")
    LabHandler.base_path = options.base_path
    root = options.directory.resolve(strict=True)
    handler = functools.partial(LabHandler, directory=str(root))
    print(f"Static browser smoke page: http://127.0.0.1:{options.port}{options.base_path}", flush=True)
    http.server.ThreadingHTTPServer(("127.0.0.1", options.port), handler).serve_forever()
