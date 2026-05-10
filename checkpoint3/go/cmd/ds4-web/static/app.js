// DS4 web SPA — vanilla JS, no framework, no build step (DESIGN_go.md §O7b).
'use strict';

const $ = (sel) => document.querySelector(sel);

const els = {
  domain:   $('#domain'),
  query:    $('#query'),
  solve:    $('#solve'),
  result:   $('#result'),
  status:   $('#status-text'),
  examples: $('#examples-menu'),
};

async function loadExamples() {
  try {
    const list = await fetch('/api/examples').then((r) => r.json()) || [];
    list.forEach((e) => {
      const opt = document.createElement('option');
      opt.value = e.id;
      opt.textContent = e.title;
      els.examples.appendChild(opt);
    });
  } catch (err) {
    console.warn('examples list unavailable:', err);
  }
}

els.examples.addEventListener('change', async () => {
  const id = els.examples.value;
  if (!id) return;
  try {
    const ex = await fetch(`/api/examples/${encodeURIComponent(id)}`).then((r) => r.json());
    els.domain.value = ex.domain || '';
    els.query.value = (ex.queries && ex.queries[0]?.text) || '';
  } catch (err) {
    setStatus(`błąd: ${err}`);
  }
});

els.solve.addEventListener('click', async () => {
  setStatus('liczę…');
  try {
    const res = await fetch('/api/solve', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ domain: els.domain.value, query: els.query.value }),
    }).then((r) => r.json());
    render(res);
  } catch (err) {
    setStatus(`błąd: ${err}`);
  }
});

function render(res) {
  els.result.innerHTML = '';
  const ans = document.createElement('p');
  ans.className = `answer-${res.answer}`;
  ans.textContent = `Odpowiedź: ${res.answer.toUpperCase()}`;
  els.result.appendChild(ans);

  if (res.error) {
    const e = document.createElement('pre');
    e.textContent = `${res.error.kind}: ${res.error.message}` +
      (res.error.location ? ` (${res.error.location.line}:${res.error.location.column})` : '');
    els.result.appendChild(e);
  }

  (res.trace || []).forEach((branch, i) => {
    const det = document.createElement('details');
    det.open = i === 0;
    const sum = document.createElement('summary');
    sum.textContent = `σ₀ #${i + 1}: { ${(branch.initial_state.true || []).join(', ')} }`;
    det.appendChild(sum);
    det.appendChild(renderSteps(branch.steps || []));
    els.result.appendChild(det);
  });

  const s = res.summary || {};
  setStatus(`|Σ|=${s.sigma_count ?? '?'} · |Σ₀|=${s.sigma0_count ?? '?'} · ${s.elapsed_ms ?? '?'}ms`);
}

function renderSteps(steps) {
  const ul = document.createElement('ul');
  ul.className = 'trace';
  for (const st of steps) {
    const li = document.createElement('li');
    const label = st.action ? `${st.action} → { ${(st.state.true || []).join(', ')} }`
                            : `{ ${(st.state.true || []).join(', ')} }`;
    li.textContent = label + (st.blocked_reason ? `  ✗ ${st.blocked_reason}` : '');
    if (st.children && st.children.length) li.appendChild(renderSteps(st.children));
    ul.appendChild(li);
  }
  return ul;
}

function setStatus(msg) { els.status.textContent = msg; }

loadExamples();
