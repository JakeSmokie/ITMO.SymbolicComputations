# Third-party assets

The expression parser, evaluator and LaTeX formatter are C# code in this repository.
The browser uses **KaTeX 0.19.0** only to typeset the formatter's output.

- Upstream: https://github.com/KaTeX/KaTeX
- Distribution: https://github.com/KaTeX/KaTeX/releases/download/v0.19.0/katex.zip
- Archive SHA-256: 012B5E160D6D0EEEEDF68AFF641B22711C0D834B89C7C37D2E407325409BA17B
- Retrieved: 2026-10-08.
- Included unchanged: katex.min.js, katex.min.css, fonts.
- License: MIT; see ITMO.SymbolicComputations.Web/wwwroot/vendor/katex/LICENSE.

Assets are served with the app, with no runtime dependency on a third-party CDN.
Renderer options: trust=false, maxExpand=1000, maxSize=20; generated formulas over
12,000 characters use a readable plain-text fallback. The complete result remains
available in the text/LaTeX disclosure and JSON export. This is LaTeX output, not a
general-purpose TeX input interpreter.
