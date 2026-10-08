(() => {
  'use strict';

  const $ = id => document.getElementById(id);
  const editor = $('expression');
  const historyKey = 'symbolic.workbench.recent.v1';
  const state = { response: null, examples: [], functions: [], functionsError: false, category: 'Все', tab: 'result', step: 0, revision: 0, request: 0, controller: null, busy: false, recent: [], toastTimer: null, engine: null };
  const escape = value => String(value ?? '').replace(/[&<>"']/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[character]));
  const number = value => new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 2 }).format(Number(value) || 0);
  const symbols = ['↗', 'ƒ', '∑', '{ }', '↻', 'xⁿ', '∞', 'λ'];

  function toast(message) {
    clearTimeout(state.toastTimer);
    $('toast').textContent = message;
    $('toast').hidden = false;
    state.toastTimer = setTimeout(() => { $('toast').hidden = true; }, 3000);
  }

  function emptyView(title = 'Сначала вычисли выражение', detail = 'После вычисления здесь появится ответ движка.') {
    return `<div class="empty-state"><div class="empty-glyph" aria-hidden="true">ƒ<span>(x)</span></div><h3>${escape(title)}</h3><p>${escape(detail)}</p></div>`;
  }

  function invalidate() {
    state.revision++;
    state.request++;
    state.controller?.abort();
    state.controller = null;
    state.busy = false;
    state.response = null;
    state.step = 0;
    $('error-box').hidden = true;
    $('error-box').replaceChildren();
    editor.removeAttribute('aria-invalid');
    $('evaluate').disabled = false;
    $('evaluate-label').textContent = 'Вычислить';
    $('output-panel').setAttribute('aria-busy', 'false');
    $('run-status').textContent = editor.value.trim() ? 'Готов к вычислению' : 'Введи выражение';
    $('copy-result').disabled = true;
    $('copy-latex').disabled = true;
    $('export-result').disabled = true;
    $('steps-badge').textContent = '—';
    $('tree-badge').textContent = '—';
    $('result-meta').innerHTML = '<span class="muted">Ожидает вычисления</span>';
    $('view-result').innerHTML = emptyView('Новое выражение — новая история', 'Нажми «Вычислить» или Ctrl + Enter, чтобы получить результат.');
    $('view-steps').innerHTML = emptyView('Преобразования по шагам', 'Здесь можно будет пройти от исходной формулы к результату.');
    $('view-tree').innerHTML = emptyView('Структура выражения', 'Здесь появится дерево, которое построил движок.');
    updateEditorInfo();
    updateExampleSelection();
  }

  function updateEditorInfo() {
    const lines = Math.min(editor.value.split('\n').length, 1000);
    $('line-numbers').textContent = Array.from({ length: lines }, (_, index) => index + 1).join('\n');
    $('char-count').textContent = `${number(editor.value.length)} симв.`;
  }

  function setExpression(expression, focus = true) {
    editor.value = expression;
    invalidate();
    if (focus) {
      editor.focus({ preventScroll: true });
      editor.setSelectionRange(expression.length, expression.length);
      if (matchMedia('(max-width: 700px)').matches) editor.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  function insertText(text, caret) {
    const start = editor.selectionStart;
    const end = editor.selectionEnd;
    editor.setRangeText(text, start, end, 'end');
    invalidate();
    editor.focus({ preventScroll: true });
    const position = caret === undefined ? start + text.length : start + caret;
    editor.setSelectionRange(position, position);
  }

  function updateExampleSelection() {
    document.querySelectorAll('[data-example]').forEach(button => {
      const example = state.examples[Number(button.dataset.example)];
      const selected = example?.expression === editor.value;
      button.classList.toggle('active', selected);
      button.setAttribute('aria-pressed', String(selected));
    });
  }

  async function getJson(url, options = {}) {
    if (window.symbolicReady) await window.symbolicReady;
    if (options.signal?.aborted) throw new DOMException('Aborted', 'AbortError');
    const response = window.symbolicTransport
      ? await window.symbolicTransport(url, { cache: 'no-store', ...options })
      : await fetch(url, { cache: 'no-store', ...options });
    let data;
    try { data = await response.json(); } catch {
      const error = new Error(response.ok ? 'Движок вернул ответ в неизвестном формате.' : `Сервер ответил HTTP ${response.status}.`);
      error.code = 'INVALID_RESPONSE';
      throw error;
    }
    if (!response.ok) {
      const error = new Error(typeof data.error === 'string' ? data.error : `Не удалось выполнить запрос (HTTP ${response.status}).`);
      error.code = data.code || 'REQUEST_FAILED';
      if (Number.isInteger(data.position) && data.position >= 0) error.position = data.position;
      throw error;
    }
    return data;
  }

  async function loadExamples() {
    try {
      const examples = await getJson('/api/examples', { signal: AbortSignal.timeout(30000) });
      if (!Array.isArray(examples)) throw new Error('Invalid examples');
      state.examples = examples.filter(example => typeof example.expression === 'string');
      $('example-count').textContent = state.examples.length;
      $('examples').innerHTML = state.examples.map((example, index) => `<button type="button" class="example-button" data-example="${index}" aria-pressed="false" title="${escape(example.expression)}"><span class="example-symbol" aria-hidden="true">${symbols[index % symbols.length]}</span><span class="example-text"><strong>${escape(example.title || example.id || 'Пример')}</strong><span>${escape(example.description || example.expression)}</span></span></button>`).join('') || '<p class="sidebar-message muted">В движке пока нет примеров.</p>';
      updateExampleSelection();
    } catch {
      $('example-count').textContent = '—';
      $('examples').innerHTML = '<p class="sidebar-message muted">Примеры недоступны. <button type="button" class="text-button" id="retry-examples">Повторить</button></p>';
    }
  }

  async function loadFunctions() {
    state.functionsError = false;
    try {
      const functions = await getJson('/api/functions', { signal: AbortSignal.timeout(30000) });
      if (!Array.isArray(functions)) throw new Error('Invalid functions');
      state.functions = functions.filter(fn => typeof fn.name === 'string').map(fn => ({ ...fn, category: fn.category || 'Функции' }));
    } catch {
      state.functionsError = true;
    }
    renderReference();
  }

  async function checkHealth() {
    const status = $('health-status');
    try {
      const health = await getJson('/api/health', { signal: AbortSignal.timeout(30000) });
      state.engine = typeof health.engine === 'string' ? health.engine : 'Символьный движок';
      status.className = 'engine-pill healthy';
      status.innerHTML = `<span class="status-dot" aria-hidden="true"></span><span>Движок подключён<small class="engine-name">${escape(state.engine)}</small></span>`;
      status.title = state.engine;
    } catch {
      status.className = 'engine-pill unhealthy';
      status.innerHTML = '<span class="status-dot" aria-hidden="true"></span><span>Нет связи с движком</span>';
      status.title = window.symbolicTransport ? 'Браузерный движок не загрузился. Обнови страницу или попробуй вычисление ещё раз.' : 'Проверь, что локальный сервер запущен. Вычисление повторит подключение.';
    }
  }

  function renderReference() {
    const categories = ['Все', ...new Set(state.functions.map(fn => fn.category))];
    if (!categories.includes(state.category)) state.category = 'Все';
    $('function-categories').innerHTML = categories.map(category => `<button type="button" class="category-button" data-category="${escape(category)}" aria-pressed="${state.category === category}">${escape(category)}</button>`).join('');
    if (state.functionsError) {
      $('function-list').innerHTML = '<p class="reference-empty">Не удалось получить справочник. <button type="button" id="retry-functions" class="text-button">Попробовать ещё раз</button></p>';
      return;
    }
    const query = $('function-search').value.trim().toLocaleLowerCase('ru');
    const matches = state.functions.map((fn, index) => ({ ...fn, index })).filter(fn => (state.category === 'Все' || fn.category === state.category) && `${fn.name} ${fn.syntax} ${fn.description} ${fn.category}`.toLocaleLowerCase('ru').includes(query));
    if (!matches.length) {
      $('function-list').innerHTML = `<p class="reference-empty">${state.functions.length ? 'Ничего не нашлось. Попробуй другое имя или категорию.' : 'В движке пока нет опубликованного справочника.'}</p>`;
      return;
    }
    const groups = new Map();
    for (const fn of matches) {
      if (!groups.has(fn.category)) groups.set(fn.category, []);
      groups.get(fn.category).push(fn);
    }
    $('function-list').innerHTML = [...groups].map(([category, functions]) => `<section class="function-group"><h3>${escape(category)}</h3>${functions.map(fn => `<article class="function-card"><div class="function-card-heading"><h4>${escape(fn.name)}</h4><button type="button" class="text-button" data-function="${fn.index}" aria-label="Вставить ${escape(fn.name)} в редактор">Вставить ↗</button></div><code>${escape(fn.syntax || `${fn.name}()`)}</code><p>${escape(fn.description || '')}</p></article>`).join('')}</section>`).join('');
  }

  function openReference() {
    renderReference();
    $('reference-dialog').showModal();
    $('function-search').focus();
  }

  function readRecent() {
    try {
      const value = JSON.parse(localStorage.getItem(historyKey) || '[]');
      state.recent = Array.isArray(value) ? value.filter(item => typeof item.expression === 'string' && item.expression.length < 20000).slice(0, 7) : [];
    } catch { state.recent = []; }
    renderRecent();
  }

  function renderRecent() {
    $('history').innerHTML = state.recent.length ? state.recent.map((item, index) => `<button type="button" class="history-button" data-recent="${index}" title="${escape(item.expression)}"><span aria-hidden="true">↶</span><code>${escape(item.expression)}</code></button>`).join('') : '<p class="history-empty">Успешные вычисления появятся здесь.<br>История хранится в этом браузере.</p>';
    $('clear-history').hidden = !state.recent.length;
  }

  function remember(expression) {
    state.recent = [{ expression, date: new Date().toISOString() }, ...state.recent.filter(item => item.expression !== expression)].slice(0, 7);
    try { localStorage.setItem(historyKey, JSON.stringify(state.recent)); } catch { /* Private browsing can make storage unavailable. */ }
    renderRecent();
  }

  function activateTab(tab, focus = false) {
    if (!['result', 'steps', 'tree'].includes(tab)) return;
    state.tab = tab;
    document.querySelectorAll('[role="tab"][data-tab]').forEach(button => {
      const active = button.dataset.tab === tab;
      button.setAttribute('aria-selected', String(active));
      button.tabIndex = active ? 0 : -1;
      if (active && focus) button.focus();
    });
    ['result', 'steps', 'tree'].forEach(name => { $(`view-${name}`).hidden = name !== tab; });
  }

  function setPending() {
    state.busy = true;
    state.response = null;
    $('evaluate').disabled = true;
    $('evaluate-label').textContent = 'Вычисляем…';
    $('run-status').textContent = 'Разбираем и преобразуем выражение';
    $('output-panel').setAttribute('aria-busy', 'true');
    $('error-box').hidden = true;
    editor.removeAttribute('aria-invalid');
    $('copy-result').disabled = true;
    $('copy-latex').disabled = true;
    $('export-result').disabled = true;
    $('steps-badge').textContent = '…';
    $('tree-badge').textContent = '…';
    $('result-meta').innerHTML = '<span>Запрос к движку</span>';
    const pending = '<div class="pending-state"><span class="spinner" aria-hidden="true"></span><span>Следуем за выражением…</span><small>Редактор остаётся доступным</small></div>';
    ['result', 'steps', 'tree'].forEach(name => { $(`view-${name}`).innerHTML = pending; });
  }

  function showError(error) {
    const position = Number.isInteger(error.position) ? Math.min(error.position, editor.value.length) : null;
    const message = error.message || 'Не удалось вычислить выражение.';
    let location = '';
    if (position !== null) {
      const before = editor.value.slice(0, position);
      const line = before.split('\n').length;
      const column = position - (before.lastIndexOf('\n') + 1) + 1;
      const lineStart = before.lastIndexOf('\n') + 1;
      const lineEnd = editor.value.indexOf('\n', position);
      const text = editor.value.slice(lineStart, lineEnd < 0 ? editor.value.length : lineEnd);
      const crop = Math.max(0, column - 46);
      const fragment = text.slice(crop, crop + 100);
      location = `<div class="error-location">Строка ${line}, столбец ${column}</div><code>${escape(fragment)}\n${' '.repeat(Math.max(0, column - crop - 1))}^</code><button type="button" class="error-focus" data-error-position="${position}">Перейти к этому месту</button>`;
      editor.setAttribute('aria-invalid', 'true');
      editor.focus({ preventScroll: true });
      editor.setSelectionRange(position, Math.min(position + 1, editor.value.length));
    }
    $('error-box').innerHTML = `<strong>${escape(message)}</strong>${location}`;
    $('error-box').hidden = false;
    $('run-status').textContent = 'Вычисление не выполнено';
    $('view-result').innerHTML = emptyView('Выражение требует внимания', 'Подсказка об ошибке находится под редактором. Исправь выражение и попробуй снова.');
    $('view-steps').innerHTML = emptyView('Преобразования не получены', 'Движок не завершил это вычисление.');
    $('view-tree').innerHTML = emptyView('Дерево не получено', 'Исправь выражение и запусти вычисление.');
    $('steps-badge').textContent = '—';
    $('tree-badge').textContent = '—';
    $('result-meta').innerHTML = `<span>${escape(error.code || 'Ошибка вычисления')}</span>`;
  }

  async function evaluate() {
    if (!editor.value.trim()) {
      showError({ message: 'Напиши выражение или выбери пример слева.', code: 'EMPTY_INPUT', position: 0 });
      return;
    }
    state.controller?.abort();
    const controller = new AbortController();
    const request = ++state.request;
    const revision = state.revision;
    const expression = editor.value;
    state.controller = controller;
    let timedOut = false;
    const timeout = setTimeout(() => { timedOut = true; controller.abort(); }, 30000);
    setPending();
    const current = () => request === state.request && revision === state.revision;
    try {
      const response = await getJson('/api/evaluate', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ expression }), signal: controller.signal });
      if (!current()) return;
      if (typeof response.result !== 'string' || !Array.isArray(response.steps) || response.steps.some(step => typeof step !== 'string')) throw new Error('Ответ движка не содержит результата в ожидаемом формате.');
      state.response = response;
      state.step = 0;
      remember(expression);
      renderResult();
      $('run-status').textContent = 'Готово — можно исследовать';
      $('health-status').className = 'engine-pill healthy';
      $('health-status').innerHTML = `<span class="status-dot" aria-hidden="true"></span><span>Движок подключён${state.engine ? `<small class="engine-name">${escape(state.engine)}</small>` : ''}</span>`;
    } catch (error) {
      if (!current()) return;
      if (timedOut) showError({ message: 'Движок не ответил за 30 секунд. Попробуй более короткое выражение.', code: 'TIMEOUT' });
      else if (error.name !== 'AbortError') {
        if (error instanceof TypeError) showError({ message: window.symbolicTransport ? 'Не удалось обратиться к браузерному движку. Обнови страницу и попробуй ещё раз.' : 'Не удалось связаться с движком. Проверь, что локальный сервер запущен.', code: 'CONNECTION_ERROR' });
        else showError(error);
      }
    } finally {
      clearTimeout(timeout);
      if (current()) {
        state.busy = false;
        state.controller = null;
        $('evaluate').disabled = false;
        $('evaluate-label').textContent = 'Вычислить';
        $('output-panel').setAttribute('aria-busy', 'false');
      }
    }
  }


  function renderMath(element, latex, fallback) {
    element.replaceChildren();
    if (typeof latex !== 'string' || !window.katex || latex.length > 12000) {
      element.textContent = fallback;
      element.classList.add('math-fallback');
      return;
    }
    try {
      window.katex.render(latex, element, {
        displayMode: true, output: 'htmlAndMathml', throwOnError: true,
        trust: false, strict: 'ignore', maxExpand: 1000, maxSize: 20
      });
      element.classList.remove('math-fallback');
    } catch {
      element.textContent = fallback;
      element.classList.add('math-fallback');
    }
  }

  function renderResult() {
    const response = state.response;
    $('view-result').innerHTML = `<div class="result-label">РЕЗУЛЬТАТ</div><div id="result-math" class="math-output result-expression" tabindex="0"></div><div class="input-recap"><span>ИСХОДНАЯ ФОРМУЛА</span><div id="input-math" class="math-output" tabindex="0"></div></div><details class="source-details"><summary>Текст и LaTeX</summary><span>Ответ движка</span><pre>${escape(response.result)}</pre><span>LaTeX результата</span><pre id="latex-source">${escape(response.resultLatex || '')}</pre></details><button type="button" class="text-button result-note" data-tab="steps">Проследить вычисление по шагам →</button>`;
    renderMath($('result-math'), response.resultLatex, response.result);
    renderMath($('input-math'), response.inputLatex, response.input ?? editor.value);
    $('steps-badge').textContent = number(response.steps.length);
    $('tree-badge').textContent = response.ast ? number(response.nodeCount) : '—';
    $('result-meta').innerHTML = `<span>${number(response.elapsedMs)} мс</span><span>${number(response.totalStepCount ?? response.stepCount ?? response.steps.length)} шаг.</span>`;
    $('copy-result').disabled = false;
    $('copy-latex').disabled = typeof response.resultLatex !== 'string';
    $('export-result').disabled = false;
    renderSteps();
    renderTree();
  }

  function renderSteps() {
    const steps = state.response?.steps || [];
    if (!steps.length) {
      $('view-steps').innerHTML = emptyView('Без промежуточных преобразований', 'Движок вернул результат без списка шагов.');
      return;
    }
    const traceNote = state.response.traceTruncated ? `Показано ${number(steps.length)} сохранённых шагов${Number.isFinite(state.response.totalStepCount) ? ` из ${number(state.response.totalStepCount)}` : ''}. Движок ограничил размер истории.` : 'Порядок и содержание шагов сохранены из ответа движка.';
    $('view-steps').innerHTML = `<div class="steps-title"><span>ШАГИ ВЫЧИСЛЕНИЯ</span><strong id="step-label"></strong></div><div id="step-expression" class="math-output step-expression" tabindex="0"></div><details class="source-details"><summary>Текст шага</summary><pre id="step-source"></pre></details><div class="step-controls"><button type="button" class="icon-button" id="previous-step" aria-label="Предыдущий шаг">←</button><input type="range" id="step-range" min="0" max="${steps.length - 1}" value="0" aria-label="Шаг преобразования"><button type="button" class="icon-button" id="next-step" aria-label="Следующий шаг">→</button></div><div class="step-position">${escape(traceNote)}</div>${steps.length <= 30 ? `<div class="step-jump" aria-label="Перейти к шагу">${steps.map((_, index) => `<button type="button" class="step-dot" data-step="${index}" aria-label="Шаг ${index + 1}">${index + 1}</button>`).join('')}</div>` : ''}`;
    updateStep(0);
  }

  function updateStep(index) {
    const steps = state.response?.steps || [];
    if (!steps.length || !$('step-expression')) return;
    state.step = Math.min(Math.max(index, 0), steps.length - 1);
    renderMath($('step-expression'), state.response.stepsLatex?.[state.step], steps[state.step]);
    $('step-source').textContent = steps[state.step];
    $('step-label').textContent = `${state.step + 1} / ${steps.length}`;
    $('step-range').value = state.step;
    $('step-range').setAttribute('aria-valuetext', `Шаг ${state.step + 1} из ${steps.length}`);
    $('previous-step').disabled = state.step === 0;
    $('next-step').disabled = state.step === steps.length - 1;
    document.querySelectorAll('[data-step]').forEach(button => {
      if (Number(button.dataset.step) === state.step) button.setAttribute('aria-current', 'step');
      else button.removeAttribute('aria-current');
    });
  }

  function renderTree() {
    const ast = state.response?.ast;
    if (!ast || typeof ast !== 'object') {
      $('view-tree').innerHTML = emptyView('Дерево недоступно', 'Движок не вернул структуру для этого выражения.');
      return;
    }
    let visited = 0;
    let truncated = false;
    function node(value, depth = 0) {
      if (!value || typeof value !== 'object') return '';
      if (++visited > 1500 || depth > 60) { truncated = true; return ''; }
      const children = Array.isArray(value.children) ? value.children : [];
      const label = `<span class="tree-kind">${escape(value.kind || 'Node')}</span><span class="tree-value">${escape(value.value ?? '')}</span>`;
      if (!children.length) return `<div class="tree-leaf">${label}</div>`;
      return `<details${depth < 2 ? ' open' : ''}><summary>${label}</summary><div class="tree-children">${children.map(child => node(child, depth + 1)).join('')}</div></details>`;
    }
    const tree = node(ast);
    $('view-tree').innerHTML = `<div class="tree-toolbar"><span>СТРУКТУРА ИЗ ОТВЕТА ДВИЖКА</span><button type="button" class="text-button" id="toggle-tree">Развернуть всё</button></div><div class="tree">${tree}</div>${truncated ? '<p class="result-note">Большое дерево показано частично. Полная структура доступна в JSON.</p>' : ''}`;
  }

  async function copyResult(format = 'text') {
    const result = format === 'latex' ? state.response?.resultLatex : state.response?.result;
    if (typeof result !== 'string') return;
    try {
      if (navigator.clipboard?.writeText) await navigator.clipboard.writeText(result);
      else {
        const area = document.createElement('textarea');
        area.value = result;
        area.style.position = 'fixed';
        area.style.opacity = '0';
        document.body.append(area);
        area.select();
        const copied = document.execCommand('copy');
        area.remove();
        if (!copied) throw new Error('Clipboard unavailable');
      }
      toast(format === 'latex' ? 'LaTeX результата скопирован' : 'Результат скопирован');
    } catch { toast('Буфер обмена недоступен. Выдели результат и скопируй вручную.'); }
  }

  function exportResult() {
    if (!state.response) return;
    const blob = new Blob([JSON.stringify(state.response, null, 2)], { type: 'application/json;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `symbolic-${new Date().toISOString().replace(/[:.]/g, '-')}.json`;
    document.body.append(link);
    link.click();
    link.remove();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
    toast('Ответ движка сохранён в JSON');
  }

  editor.addEventListener('input', invalidate);
  editor.addEventListener('scroll', () => { $('line-numbers').scrollTop = editor.scrollTop; });
  editor.addEventListener('keydown', event => {
    if (event.key === 'Enter' && (event.ctrlKey || event.metaKey)) {
      event.preventDefault();
      evaluate();
    }
  });
  $('evaluate').addEventListener('click', evaluate);
  $('copy-result').addEventListener('click', () => copyResult());
  $('copy-latex').addEventListener('click', () => copyResult('latex'));
  $('export-result').addEventListener('click', exportResult);
  $('open-reference').addEventListener('click', openReference);
  $('open-reference-bottom').addEventListener('click', openReference);
  $('close-reference').addEventListener('click', () => $('reference-dialog').close());
  $('reference-dialog').addEventListener('click', event => {
    const bounds = $('reference-dialog').getBoundingClientRect();
    if (event.target === $('reference-dialog') && (event.clientX < bounds.left || event.clientX > bounds.right || event.clientY < bounds.top || event.clientY > bounds.bottom)) $('reference-dialog').close();
  });
  $('function-search').addEventListener('input', renderReference);
  $('clear-history').addEventListener('click', () => {
    state.recent = [];
    try { localStorage.removeItem(historyKey); } catch { /* In-memory history still clears. */ }
    renderRecent();
    toast('История очищена');
  });

  document.addEventListener('click', event => {
    const button = event.target.closest('button');
    if (!button || button.disabled) return;
    if (button.dataset.example !== undefined) { setExpression(state.examples[Number(button.dataset.example)].expression, false); activateTab('result'); evaluate(); }
    if (button.dataset.quick !== undefined) { setExpression(button.dataset.quick, false); activateTab('result'); evaluate(); }
    if (button.dataset.recent !== undefined) setExpression(state.recent[Number(button.dataset.recent)].expression);
    if (button.dataset.insert !== undefined) insertText(button.dataset.insert, button.dataset.caret === undefined ? undefined : Number(button.dataset.caret));
    if (button.dataset.tab) activateTab(button.dataset.tab);
    if (button.dataset.category !== undefined) { state.category = button.dataset.category; renderReference(); }
    if (button.dataset.function !== undefined) {
      const fn = state.functions[Number(button.dataset.function)];
      $('reference-dialog').close();
      insertText(fn.syntax || `${fn.name}()`);
      toast(`Синтаксис ${fn.name} вставлен в редактор`);
    }
    if (button.dataset.step !== undefined) updateStep(Number(button.dataset.step));
    if (button.dataset.errorPosition !== undefined) {
      const position = Number(button.dataset.errorPosition);
      editor.focus();
      editor.setSelectionRange(position, Math.min(position + 1, editor.value.length));
    }
    if (button.id === 'previous-step') updateStep(state.step - 1);
    if (button.id === 'next-step') updateStep(state.step + 1);
    if (button.id === 'retry-examples') loadExamples();
    if (button.id === 'retry-functions') loadFunctions();
    if (button.id === 'toggle-tree') {
      const details = [...$('view-tree').querySelectorAll('details')];
      const expand = details.some(detail => !detail.open);
      details.forEach(detail => { detail.open = expand; });
      button.textContent = expand ? 'Свернуть всё' : 'Развернуть всё';
    }
  });

  document.addEventListener('input', event => {
    if (event.target.id === 'step-range') updateStep(Number(event.target.value));
  });

  document.querySelector('.tabs').addEventListener('keydown', event => {
    const tabs = ['result', 'steps', 'tree'];
    const index = tabs.indexOf(state.tab);
    let next;
    if (event.key === 'ArrowRight') next = (index + 1) % tabs.length;
    if (event.key === 'ArrowLeft') next = (index + tabs.length - 1) % tabs.length;
    if (event.key === 'Home') next = 0;
    if (event.key === 'End') next = tabs.length - 1;
    if (next !== undefined) { event.preventDefault(); activateTab(tabs[next], true); }
  });

  $('view-steps').innerHTML = emptyView('Преобразования по шагам', 'После вычисления здесь можно пройти от исходной формулы к результату.');
  $('view-tree').innerHTML = emptyView('Структура выражения', 'После вычисления здесь появится дерево, которое построил движок.');
  updateEditorInfo();
  readRecent();
  loadExamples();
  loadFunctions();
  checkHealth().then(() => {
    // A real first result; never overwrite input edited while WASM was loading.
    if (state.revision === 0 && !state.busy && !state.response && state.engine) evaluate();
  });
})();
