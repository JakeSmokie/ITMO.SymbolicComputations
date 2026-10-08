(() => {
  'use strict';

  const $ = id => document.getElementById(id);
  const canvas = $('fractal-canvas');
  const context = canvas.getContext('2d', { alpha: false });
  const presets = {
    whole: { centerRe: -0.65, centerIm: 0, span: 3.2 },
    seahorse: { centerRe: -0.743643887, centerIm: 0.131825904, span: 0.003 },
    elephant: { centerRe: 0.275, centerIm: 0, span: 0.1 }
  };
  const state = {
    target: { ...presets.whole }, displayed: null, bitmap: null,
    renderController: null, renderSequence: 0, orbitController: null, orbitSequence: 0,
    selected: { real: -0.75, imag: 0.1 }, orbit: null, showAll: false,
    clickTimer: null, resizeTimer: null, engine: null
  };
  const escape = value => String(value ?? '').replace(/[&<>"']/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[character]));
  const fixed = (value, precision = 8) => {
    if (!Number.isFinite(value)) return 'не определено';
    if (Object.is(value, -0) || value === 0) return '0';
    return Number(value.toPrecision(precision)).toString().replace('e+', 'e');
  };
  const complex = (real, imag, precision = 10) => `${fixed(real, precision)} ${imag < 0 ? '−' : '+'} ${fixed(Math.abs(imag), precision)}i`;
  const nextFrame = () => new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));

  async function api(path, body, signal) {
    if (window.symbolicReady) await window.symbolicReady;
    if (signal?.aborted) throw new DOMException('Запрос отменён.', 'AbortError');
    const options = body === undefined
      ? { method: 'GET', signal, cache: 'no-store' }
      : { method: 'POST', signal, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) };
    const response = window.symbolicTransport
      ? await window.symbolicTransport(path, options)
      : await fetch(path, options);
    if (signal?.aborted) throw new DOMException('Запрос отменён.', 'AbortError');
    let data;
    try { data = await response.json(); }
    catch { throw new Error(`Движок вернул ответ неизвестного формата (HTTP ${response.status}).`); }
    if (!response.ok) throw new Error(typeof data.error === 'string' ? data.error : `Ошибка движка: HTTP ${response.status}.`);
    return data;
  }

  async function health() {
    try {
      const value = await api('/api/health');
      state.engine = typeof value.engine === 'string' ? value.engine : 'SymbolicComputations';
      $('runtime-status').className = 'runtime-status ready';
      $('runtime-status').innerHTML = `<i aria-hidden="true"></i><span>Движок готов<small>${escape(state.engine)}</small></span>`;
    } catch (error) {
      $('runtime-status').className = 'runtime-status failed';
      $('runtime-status').innerHTML = '<i aria-hidden="true"></i><span>Не удалось загрузить движок</span>';
      $('runtime-status').title = error.message;
    }
  }

  function dimensions() {
    const width = Math.max(240, Math.min(640, Math.round($('plot-wrap').clientWidth || 640)));
    return { width, height: Math.min(480, Math.max(240, Math.round(width * 0.6625))) };
  }

  function bounds(view) {
    const ySpan = view.span * view.height / view.width;
    return { left: view.centerRe - view.span / 2, right: view.centerRe + view.span / 2, top: view.centerIm + ySpan / 2, bottom: view.centerIm - ySpan / 2, ySpan };
  }

  function pixelForPoint(real, imag, view = state.displayed) {
    const area = bounds(view);
    return { x: (real - area.left) / view.span * view.width, y: (area.top - imag) / area.ySpan * view.height };
  }

  function pointForEvent(event) {
    if (!state.displayed) return null;
    const rect = canvas.getBoundingClientRect();
    const area = bounds(state.displayed);
    return {
      real: area.left + (event.clientX - rect.left) / rect.width * state.displayed.span,
      imag: area.top - (event.clientY - rect.top) / rect.height * area.ySpan
    };
  }

  const paletteStops = [[22, 27, 53], [79, 55, 129], [158, 106, 199], [99, 217, 220], [188, 247, 125], [22, 27, 53]];
  function color(value) {
    if (value < 0) return [5, 7, 13];
    const position = ((value * 0.037) % 1 + 1) % 1 * (paletteStops.length - 1);
    const index = Math.floor(position);
    const fraction = position - index;
    return paletteStops[index].map((channel, channelIndex) => Math.round(channel + (paletteStops[index + 1][channelIndex] - channel) * fraction));
  }

  function decodeImage(reply) {
    if (!Number.isInteger(reply.width) || !Number.isInteger(reply.height) || reply.width < 1 || reply.height < 1 || reply.width > 640 || reply.height > 480 || !Number.isFinite(reply.centerRe) || !Number.isFinite(reply.centerIm) || !Number.isFinite(reply.span) || reply.span <= 0 || typeof reply.valuesBase64 !== 'string') throw new Error('Движок вернул некорректную геометрию изображения.');
    const raw = atob(reply.valuesBase64);
    if (raw.length !== reply.width * reply.height * 4) throw new Error('Размер численных данных не совпадает с размером изображения.');
    const bytes = Uint8Array.from(raw, character => character.charCodeAt(0));
    const values = new DataView(bytes.buffer);
    const image = new ImageData(reply.width, reply.height);
    for (let index = 0; index < reply.width * reply.height; index++) {
      const value = values.getFloat32(index * 4, true);
      if (!Number.isFinite(value)) throw new Error('Изображение содержит нечисловое значение.');
      const rgb = color(value);
      image.data[index * 4] = rgb[0];
      image.data[index * 4 + 1] = rgb[1];
      image.data[index * 4 + 2] = rgb[2];
      image.data[index * 4 + 3] = 255;
    }
    return image;
  }

  function gridStep(span) {
    const rough = span / 5;
    const power = 10 ** Math.floor(Math.log10(rough));
    const fraction = rough / power;
    return (fraction <= 1 ? 1 : fraction <= 2 ? 2 : fraction <= 5 ? 5 : 10) * power;
  }

  function paintPlot() {
    if (!state.displayed || !state.bitmap) return;
    const view = state.displayed;
    if (canvas.width !== view.width || canvas.height !== view.height) {
      canvas.width = view.width;
      canvas.height = view.height;
    }
    canvas.style.aspectRatio = `${view.width}/${view.height}`;
    context.putImageData(state.bitmap, 0, 0);
    const area = bounds(view);
    const step = gridStep(view.span);
    context.font = '12px "Cascadia Code", Consolas, monospace';
    context.lineWidth = 1;
    context.strokeStyle = 'rgba(216, 211, 239, .14)';
    context.fillStyle = 'rgba(228, 215, 239, .8)';
    context.shadowColor = '#0b0817';
    context.shadowBlur = 3;
    for (let value = Math.ceil(area.left / step) * step, count = 0; value < area.right && count < 20; value += step, count++) {
      const x = pixelForPoint(value, view.centerIm).x;
      context.beginPath(); context.moveTo(x, 0); context.lineTo(x, view.height); context.stroke();
      if (x > 24 && x < view.width - 32) context.fillText(fixed(value, 5), x + 4, 17);
    }
    for (let value = Math.ceil(area.bottom / step) * step, count = 0; value < area.top && count < 20; value += step, count++) {
      const y = pixelForPoint(view.centerRe, value).y;
      context.beginPath(); context.moveTo(0, y); context.lineTo(view.width, y); context.stroke();
      if (y > 28 && y < view.height - 30) context.fillText(fixed(value, 5) + 'i', 7, y - 5);
    }
    context.shadowBlur = 0;
    const selected = pixelForPoint(state.selected.real, state.selected.imag);
    if (selected.x >= 0 && selected.x <= view.width && selected.y >= 0 && selected.y <= view.height) {
      context.strokeStyle = '#bcf77d'; context.lineWidth = 1.5;
      context.shadowBlur = 7; context.shadowColor = '#050910';
      context.beginPath(); context.arc(selected.x, selected.y, 5, 0, Math.PI * 2); context.stroke();
      context.beginPath(); context.moveTo(selected.x - 11, selected.y); context.lineTo(selected.x - 7, selected.y); context.moveTo(selected.x + 7, selected.y); context.lineTo(selected.x + 11, selected.y); context.moveTo(selected.x, selected.y - 11); context.lineTo(selected.x, selected.y - 7); context.moveTo(selected.x, selected.y + 7); context.lineTo(selected.x, selected.y + 11); context.stroke();
      context.shadowBlur = 0;
    }
  }

  function commitViewport(reply, image) {
    // Coordinates and labels belong to the returned image, never to a request still in flight.
    state.displayed = { centerRe: reply.centerRe, centerIm: reply.centerIm, span: reply.span, width: reply.width, height: reply.height, maxIterations: reply.maxIterations };
    state.bitmap = image;
    paintPlot();
    const area = bounds(state.displayed);
    $('real-range').textContent = `Re: ${fixed(area.left, 10)} … ${fixed(area.right, 10)}`;
    $('imaginary-range').textContent = `Im: ${fixed(area.bottom, 10)} … ${fixed(area.top, 10)}`;
    $('zoom-level').textContent = `${fixed(presets.whole.span / reply.span, 5)}×`;
    $('render-meta').textContent = `${reply.width} × ${reply.height} · ${fixed(Number(reply.elapsedMs), 4)} мс · ${reply.maxIterations} итер.`;
    canvas.setAttribute('aria-label', `Множество Мандельброта: Re от ${fixed(area.left)} до ${fixed(area.right)}, Im от ${fixed(area.bottom)} до ${fixed(area.top)}, ${reply.maxIterations} итераций. Нажми на точку или используй форму координат.`);
    document.querySelectorAll('[data-preset]').forEach(button => {
      const preset = presets[button.dataset.preset];
      button.setAttribute('aria-pressed', String(reply.centerRe === preset.centerRe && reply.centerIm === preset.centerIm && reply.span === preset.span));
    });
  }

  async function renderFractal(view = state.target) {
    state.renderController?.abort();
    const controller = new AbortController();
    const sequence = ++state.renderSequence;
    state.renderController = controller;
    state.target = { centerRe: view.centerRe, centerIm: view.centerIm, span: Math.max(1e-12, Math.min(20, view.span)) };
    const request = { ...state.target, ...dimensions(), maxIterations: Number($('max-iterations').value) };
    $('plot-loading').hidden = false;
    $('plot-error').hidden = true;
    $('plot-wrap').setAttribute('aria-busy', 'true');
    let timedOut = false;
    const timeout = setTimeout(() => { timedOut = true; controller.abort(); }, 45000);
    try {
      await nextFrame();
      if (controller.signal.aborted) return;
      const reply = await api('/api/fractal', request, controller.signal);
      if (sequence !== state.renderSequence || controller.signal.aborted) return;
      const image = decodeImage(reply);
      if (sequence !== state.renderSequence) return;
      commitViewport(reply, image);
    } catch (error) {
      if (sequence !== state.renderSequence) return;
      if (error.name !== 'AbortError' || timedOut) {
        const message = timedOut ? 'Движок не ответил на запрос изображения. Попробуй уменьшить число итераций.' : error.message;
        $('plot-error').innerHTML = `<strong>Не удалось построить область</strong><span>${escape(message)}</span>${state.displayed ? '<span>Предыдущее изображение и его координаты сохранены.</span>' : ''}<button type="button" id="retry-render" class="quiet-button">Повторить</button>${state.displayed ? '<button type="button" id="keep-render" class="quiet-button">Вернуться к предыдущему виду</button>' : ''}`;
        $('plot-error').hidden = false;
      }
    } finally {
      clearTimeout(timeout);
      if (sequence === state.renderSequence) {
        $('plot-loading').hidden = true;
        $('plot-wrap').setAttribute('aria-busy', 'false');
        state.renderController = null;
      }
    }
  }

  function setPoint(real, imag, scroll = false) {
    state.selected = { real, imag };
    $('point-real').value = fixed(real, 13);
    $('point-imaginary').value = fixed(imag, 13);
    paintPlot();
    loadOrbit(real, imag);
    if (scroll && matchMedia('(max-width: 950px)').matches) $('point-details').scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  function parseCoordinate(value) {
    const normalized = value.trim().replace(/−/g, '-').replace(',', '.');
    if (!normalized || !/^[+-]?(?:\d+\.?\d*|\.\d+)(?:e[+-]?\d+)?$/i.test(normalized)) return NaN;
    return Number(normalized);
  }

  function paintOrbit(points = []) {
    const target = $('orbit-canvas');
    const ratio = Math.min(2, window.devicePixelRatio || 1);
    const width = Math.max(240, target.clientWidth || 280);
    const height = Math.round(width * 370 / 480);
    target.width = Math.round(width * ratio);
    target.height = Math.round(height * ratio);
    target.style.aspectRatio = '480/370';
    const ctx = target.getContext('2d');
    ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
    ctx.fillStyle = '#100e1b'; ctx.fillRect(0, 0, width, height);
    const finitePoints = points.filter(point => Number.isFinite(point.re) && Number.isFinite(point.im));
    const extent = Math.max(2.3, ...finitePoints.flatMap(point => [Math.abs(point.re) * 1.12, Math.abs(point.im) * 1.12]));
    const left = 26, top = 33, plotWidth = width - 46, plotHeight = height - 53;
    const scale = Math.min(plotWidth, plotHeight) / (2 * extent);
    const origin = { x: left + plotWidth / 2, y: top + plotHeight / 2 };
    const xy = point => ({ x: origin.x + point.re * scale, y: origin.y - point.im * scale });
    ctx.lineWidth = 1; ctx.strokeStyle = '#3c2a4b';
    ctx.beginPath(); ctx.moveTo(left, origin.y); ctx.lineTo(width - 14, origin.y); ctx.moveTo(origin.x, top); ctx.lineTo(origin.x, height - 15); ctx.stroke();
    ctx.fillStyle = '#9980aa'; ctx.font = '12px "Segoe UI",sans-serif';
    ctx.fillText('Re', width - 25, origin.y - 5); ctx.fillText('Im', origin.x + 5, top + 7);
    ctx.fillText('0', origin.x + 5, origin.y + 15);
    ctx.strokeStyle = '#594268'; ctx.setLineDash([3, 4]);
    ctx.beginPath(); ctx.arc(origin.x, origin.y, 2 * scale, 0, Math.PI * 2); ctx.stroke(); ctx.setLineDash([]);
    ctx.fillStyle = '#81688f'; ctx.fillText('|z| = 2', left, height - 8);
    if (!finitePoints.length) return;
    ctx.lineWidth = 1.6;
    finitePoints.forEach((point, index) => {
      const current = xy(point);
      if (index > 0) {
        const previous = xy(finitePoints[index - 1]);
        const line = ctx.createLinearGradient(previous.x, previous.y, current.x + .01, current.y + .01);
        line.addColorStop(0, '#b19be2'); line.addColorStop(1, '#6bd8d5'); ctx.strokeStyle = line;
        ctx.beginPath(); ctx.moveTo(previous.x, previous.y); ctx.lineTo(current.x, current.y); ctx.stroke();
      }
      ctx.fillStyle = index === finitePoints.length - 1 ? '#bcf77d' : index === 0 ? '#b69be3' : '#68d5d6';
      ctx.beginPath(); ctx.arc(current.x, current.y, index === finitePoints.length - 1 ? 4 : 2.5, 0, Math.PI * 2); ctx.fill();
    });
    const last = finitePoints[finitePoints.length - 1];
    const endpoint = xy(last);
    ctx.fillStyle = '#b9de9e'; ctx.font = '12px "Segoe UI",sans-serif';
    ctx.fillText(`n = ${last.n}`, Math.min(width - 48, endpoint.x + 8), Math.max(top + 13, endpoint.y - 8));
    target.setAttribute('aria-label', `Орбита ${finitePoints.length} точек. Последняя точка: Re ${fixed(last.re)}, Im ${fixed(last.im)}. Пунктирный круг обозначает |z| = 2.`);
  }

  function renderTable() {
    const points = state.orbit?.points || [];
    const visible = state.showAll ? points : points.slice(0, 12);
    $('iteration-rows').innerHTML = visible.map(point => {
      const isEscape = state.orbit.escaped && Number(point.n) === Number(state.orbit.escapeIteration);
      const expression = typeof point.expression === 'string' && point.expression.length
        ? `<details><summary>Показать выражение</summary><pre>${escape(point.expression)}</pre></details>`
        : `<span class="no-expression">${Number(point.n) === 0 ? 'Начальное значение z₀' : 'Движок не вернул выражение'}</span>`;
      return `<tr${isEscape ? ' class="escape-row"' : ''}><td>${escape(point.n)}${isEscape ? ' ↗' : ''}</td><td>${escape(fixed(point.re, 12))}</td><td>${escape(fixed(point.im, 12))}</td><td>${escape(fixed(point.modulus, 12))}</td><td>${expression}</td></tr>`;
    }).join('') || '<tr><td colspan="5" class="table-empty">Движок не вернул точки орбиты.</td></tr>';
    $('show-all-points').hidden = points.length <= 12;
    $('show-all-points').textContent = state.showAll ? 'Показать первые 12 строк' : `Показать все ${points.length} строк`;
  }

  async function loadOrbit(real, imag) {
    state.orbitController?.abort();
    const controller = new AbortController();
    const sequence = ++state.orbitSequence;
    state.orbitController = controller;
    state.orbit = null;
    state.showAll = false;
    $('point-error').hidden = true;
    $('evaluate-point').disabled = true;
    $('evaluate-point').textContent = 'Вычисляем орбиту…';
    $('orbit-status').className = 'orbit-status';
    $('orbit-status').textContent = 'Символьный движок считает 12 итераций…';
    $('selected-coordinate').textContent = `c = ${complex(real, imag, 13)}`;
    $('iteration-rows').innerHTML = '<tr><td colspan="5" class="table-empty">Вычисляем выбранную орбиту…</td></tr>';
    $('show-all-points').hidden = true;
    paintOrbit();
    let timedOut = false;
    const timeout = setTimeout(() => { timedOut = true; controller.abort(); }, 45000);
    try {
      await nextFrame();
      if (controller.signal.aborted) return;
      const reply = await api('/api/orbit', { real, imag, iterations: 12 }, controller.signal);
      if (sequence !== state.orbitSequence || controller.signal.aborted) return;
      if (!Array.isArray(reply.points) || !Number.isFinite(reply.real) || !Number.isFinite(reply.imag)) throw new Error('Движок вернул орбиту неизвестного формата.');
      state.orbit = reply;
      const lastN = reply.points.length ? reply.points[reply.points.length - 1].n : 0;
      $('orbit-status').className = `orbit-status${reply.escaped ? ' escaped' : ''}`;
      $('orbit-status').innerHTML = reply.escaped
        ? `<strong>Орбита вышла за |z| = 2.</strong><br>Первый выход: итерация ${escape(reply.escapeIteration)}.`
        : `<strong>За ${escape(lastN)} итераций орбита не вышла за |z| = 2.</strong><br>Это наблюдение при текущем лимите, а не доказательство принадлежности.`;
      $('selected-coordinate').textContent = `c = ${complex(reply.real, reply.imag, 13)}`;
      $('orbit-engine').textContent = 'Каждый шаг вычислен библиотекой ITMO.SymbolicComputations: формула → дерево выражения → результат.';
      paintOrbit(reply.points);
      renderTable();
    } catch (error) {
      if (sequence !== state.orbitSequence) return;
      if (error.name !== 'AbortError' || timedOut) {
        $('point-error').textContent = timedOut ? 'Движок не ответил на запрос орбиты. Попробуй повторить вычисление.' : error.message;
        $('point-error').hidden = false;
        $('orbit-status').textContent = 'Орбита не получена';
        $('iteration-rows').innerHTML = '<tr><td colspan="5" class="table-empty">Не удалось получить орбиту. Повтори запрос в форме координат.</td></tr>';
      }
    } finally {
      clearTimeout(timeout);
      if (sequence === state.orbitSequence) {
        state.orbitController = null;
        $('evaluate-point').disabled = false;
        $('evaluate-point').textContent = 'Проследить орбиту';
      }
    }
  }

  function invalidatePoint() {
    state.orbitSequence++;
    state.orbitController?.abort();
    state.orbitController = null;
    state.orbit = null;
    $('point-error').hidden = true;
    $('evaluate-point').disabled = false;
    $('evaluate-point').textContent = 'Проследить орбиту';
    $('orbit-status').textContent = 'Координаты изменены. Запусти вычисление новой точки.';
    $('orbit-status').className = 'orbit-status';
    $('selected-coordinate').textContent = 'Новая точка ещё не вычислена';
    $('iteration-rows').innerHTML = '<tr><td colspan="5" class="table-empty">Вычисли новую точку, чтобы увидеть её орбиту.</td></tr>';
    $('show-all-points').hidden = true;
    paintOrbit();
  }

  function zoom(factor, point) {
    const base = state.target;
    renderFractal({ centerRe: point?.real ?? base.centerRe, centerIm: point?.imag ?? base.centerIm, span: base.span * factor });
  }

  $('zoom-in').addEventListener('click', () => zoom(.5));
  $('zoom-out').addEventListener('click', () => zoom(2));
  $('reset-view').addEventListener('click', () => renderFractal(presets.whole));
  $('max-iterations').addEventListener('change', () => renderFractal());
  $('show-all-points').addEventListener('click', () => { state.showAll = !state.showAll; renderTable(); });
  $('point-real').addEventListener('input', invalidatePoint);
  $('point-imaginary').addEventListener('input', invalidatePoint);
  $('point-form').addEventListener('submit', event => {
    event.preventDefault();
    const real = parseCoordinate($('point-real').value);
    const imag = parseCoordinate($('point-imaginary').value);
    if (!Number.isFinite(real) || !Number.isFinite(imag)) {
      $('point-error').textContent = 'Введи конечные действительное и мнимое числа, например −0.75 и 0.1.';
      $('point-error').hidden = false;
      (!Number.isFinite(real) ? $('point-real') : $('point-imaginary')).focus();
      return;
    }
    setPoint(real, imag);
  });

  document.addEventListener('click', event => {
    const button = event.target.closest('button');
    if (!button || button.disabled) return;
    if (button.dataset.preset) renderFractal(presets[button.dataset.preset]);
    if (button.dataset.point !== undefined) setPoint(Number(button.dataset.point), 0);
    if (button.id === 'retry-render') renderFractal();
    if (button.id === 'keep-render' && state.displayed) {
      state.target = { centerRe: state.displayed.centerRe, centerIm: state.displayed.centerIm, span: state.displayed.span };
      $('max-iterations').value = state.displayed.maxIterations;
      $('plot-error').hidden = true;
    }
  });

  canvas.addEventListener('pointermove', event => {
    const point = pointForEvent(event);
    if (point) $('cursor-coordinate').textContent = `c = ${complex(point.real, point.imag, 8)}`;
  });
  canvas.addEventListener('pointerleave', () => { $('cursor-coordinate').textContent = `Выбрано c = ${complex(state.selected.real, state.selected.imag, 8)}`; });
  canvas.addEventListener('click', event => {
    if (!state.displayed || state.renderController) return;
    clearTimeout(state.clickTimer);
    const point = pointForEvent(event);
    state.clickTimer = setTimeout(() => { if (point) setPoint(point.real, point.imag, true); }, 250);
  });
  canvas.addEventListener('dblclick', event => {
    event.preventDefault();
    clearTimeout(state.clickTimer);
    if (!state.displayed || state.renderController) return;
    const point = pointForEvent(event);
    if (point) zoom(.5, point);
  });
  canvas.addEventListener('keydown', event => {
    if (event.key === '+' || event.key === '=') { event.preventDefault(); zoom(.5); }
    if (event.key === '-' || event.key === '_') { event.preventDefault(); zoom(2); }
    if (['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown'].includes(event.key)) {
      event.preventDefault();
      const view = { ...state.target };
      if (event.key === 'ArrowLeft') view.centerRe -= view.span * .2;
      if (event.key === 'ArrowRight') view.centerRe += view.span * .2;
      if (event.key === 'ArrowUp') view.centerIm += view.span * .2;
      if (event.key === 'ArrowDown') view.centerIm -= view.span * .2;
      renderFractal(view);
    }
    if (event.key === 'Enter' && state.displayed) { event.preventDefault(); setPoint(state.displayed.centerRe, state.displayed.centerIm, true); }
  });
  window.addEventListener('resize', () => {
    clearTimeout(state.resizeTimer);
    state.resizeTimer = setTimeout(() => {
      paintOrbit(state.orbit?.points || []);
      if (state.displayed && Math.abs(dimensions().width - state.displayed.width) > 60) renderFractal();
    }, 250);
  });

  paintOrbit();
  health();
  renderFractal(presets.whole);
  loadOrbit(state.selected.real, state.selected.imag);
})();
