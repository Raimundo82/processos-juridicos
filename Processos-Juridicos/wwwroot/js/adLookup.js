// adLookup.js
import { clearValidity, shouldResolve, searchUsers, resolveId, markInvalid } from './lookup-utils.js';

export const escapeHtml = (s) => {
    const str = s == null ? '' : String(s);
    return str.replace(/[&<>"']/g, c => (
        { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]
    ));
};

export function initAdLookup({ applySelection, setupIds, debounceMs = 1000 }) {
    let currentTarget = null;
    const modalEl = document.getElementById('adPickerModal');
    const adModal = modalEl ? new bootstrap.Modal(modalEl) : null;
    const searchBox = document.getElementById('adSearchBox');
    const searchBtn = document.getElementById('adSearchBtn');
    const resultsBody = document.getElementById('adResultsBody');
    const statusEl = document.getElementById('adSearchStatus');
    const clearBtn = document.getElementById('adClearBtn');
    const setStatus = (msg) => { if (statusEl) statusEl.textContent = msg || ''; };

    const renderResults = (items) => {
        if (!resultsBody) return;
        resultsBody.innerHTML = '';
        if (!items?.length) {
            resultsBody.innerHTML = '<tr><td colspan="5" class="text-muted text-center">Sem resultados</td></tr>';
            return;
        }
        for (const u of items) {
            const tr = document.createElement('tr');
            tr.style.cursor = 'pointer'; 
            tr.innerHTML = `
                <td>${escapeHtml(u.displayName || '')}</td>
                <td>${escapeHtml(u.samAccountName || '')}</td>
                <td>${escapeHtml(u.email || '')}</td>
                <td>${escapeHtml(u.department || u.company || '')}</td>
                <td><a class="btn btn-sm text-success"><i class="bi bi-check-lg"></i></a></td>
            `;
            tr.addEventListener('dblclick', () => applySelection(u, currentTarget, adModal));
            tr.querySelector('a').addEventListener('click', () => applySelection(u, currentTarget, adModal));
            resultsBody.appendChild(tr);
        }
    };

    const doSearch = async () => {
        if (!searchBox) return;
        const q = searchBox.value.trim();
        if (!q) { renderResults([]); setStatus(''); return; }
        setStatus('A pesquisar…');
        try {
            const data = await searchUsers(q);
            renderResults(data);
            setStatus(`Encontrados ${data.length} resultado(s)`);
        } catch {
            renderResults([]);
            setStatus('⚠️ Erro ao pesquisar');
        }
    };

    if (searchBox) {
        let searchTimer = null;
        searchBox.addEventListener('input', () => {
            clearTimeout(searchTimer);
            searchTimer = setTimeout(doSearch, debounceMs);
        });
    }
    if (searchBtn) searchBtn.addEventListener('click', doSearch);
    if (clearBtn) clearBtn.addEventListener('click', () => {
        if (!searchBox) return;
        searchBox.value = '';
        renderResults([]);
        setStatus('');
        searchBox.focus();
    });

    document.querySelectorAll('.ad-lookup-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            currentTarget = {
                visibleId: btn.dataset.targetVisible,
                hiddenId: btn.dataset.targetHidden,
                hiddenId2: btn.dataset.targetHidden2, // may be undefined
                infoId: btn.dataset.targetInfo
            };
            if (!adModal) return;
            const currentVal = document.getElementById(currentTarget.visibleId)?.value || '';
            if (searchBox) searchBox.value = currentVal;
            renderResults([]);
            setStatus('');
            adModal.show();
            setTimeout(() => searchBox && searchBox.focus(), 150);
            if (currentVal) doSearch();
        });
    });

    const setupLookup = (visibleId, hiddenId, infoId) => {
        const visibleEl = document.getElementById(visibleId);
        const hiddenEl = document.getElementById(hiddenId);
        const infoEl = infoId ? document.getElementById(infoId) : null;
        if (!visibleEl || !hiddenEl) return;

        const resetState = () => {
            clearValidity(visibleEl);
            hiddenEl.value = '';
            if (infoEl) infoEl.textContent = '';
        };

        const resolveAndReplace = async (raw) => {
            try {
                const data = await resolveId(raw);
                if (data?.found) {
                    const user = {
                        displayName: data.displayName || raw,
                        samAccountName: data.username || data.fullUser || raw
                    };
                    currentTarget = { visibleId, hiddenId, infoId };
                    applySelection(user, currentTarget, adModal);
                    visibleEl.value = user.displayName + ' - ' + user.samAccountName;
                    clearValidity(visibleEl);
                } else {
                    markInvalid(visibleEl);
                    if (infoEl) infoEl.textContent = `❌ Não encontrado: ${raw}`;
                }
            } catch {
                markInvalid(visibleEl);
                if (infoEl) infoEl.textContent = '⚠️ Erro ao validar ID';
            }
        };

        let t = null;
        const schedule = () => {
            const raw = (visibleEl.value || '').trim();
            if (!raw) { resetState(); return; }
            if (!shouldResolve(raw)) {
                clearValidity(visibleEl);
                if (infoEl) infoEl.textContent = '';
                return;
            }
            clearTimeout(t);
            t = setTimeout(() => resolveAndReplace(raw), debounceMs);
        };

        visibleEl.addEventListener('input', schedule);
        visibleEl.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                const raw = (visibleEl.value || '').trim();
                if (shouldResolve(raw)) {
                    clearTimeout(t);
                    resolveAndReplace(raw);
                }
            }
        });
        visibleEl.addEventListener('blur', () => {
            const raw = (visibleEl.value || '').trim();
            if (shouldResolve(raw)) {
                clearTimeout(t);
                resolveAndReplace(raw);
            }
        });
    };

    setupIds.forEach(ids => setupLookup(...ids));
}
