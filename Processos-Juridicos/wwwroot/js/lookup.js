import { markValid, markInvalid, clearValidity, shouldResolve, searchUsers, resolveId } from './lookup-utils.js';

document.addEventListener('DOMContentLoaded', () => {
    const DEBOUNCE_MS = 1000;

    // Page-specific utility
    const escapeHtml = (s) => {
        const str = s == null ? '' : String(s);
        return str.replace(/[&<>"']/g, c => (
            { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]
        ));
    };

    // Modal search
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
        if (!items || !items.length) {
            resultsBody.innerHTML = '<tr><td colspan="5" class="text-muted">Sem resultados</td></tr>';
            return;
        }
        for (const u of items) {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${escapeHtml(u.displayName || '')}</td>
                <td>${escapeHtml(u.samAccountName || '')}</td>
                <td>${escapeHtml(u.email || '')}</td>
                <td>${escapeHtml(u.department || u.company || '')}</td>
                <td><button type="button" class="btn btn-sm btn-success">Selecionar</button></td>
            `;
            tr.addEventListener('dblclick', () => applySelection(u));
            tr.querySelector('button').addEventListener('click', () => applySelection(u));
            resultsBody.appendChild(tr);
        }
    };

    const applySelection = (user) => {
        if (!currentTarget) return;
        const visible = document.getElementById(currentTarget.visibleId);
        const hidden = document.getElementById(currentTarget.hiddenId);
        const info = document.getElementById(currentTarget.infoId);
        const display = user.displayName || user.cn || user.name || '';
        const id = user.samAccountName || user.userPrincipalName || user.employeeId || '';

        if (visible) visible.value = `${display} - ${id}`;
        if (hidden) hidden.value = `${display} - ${id}`;
        if (visible) markValid(visible);
        if (info) info.textContent = id ? `✅ ${display} (${id})` : `✅ ${display}`;
        if (adModal) adModal.hide();
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
            searchTimer = setTimeout(doSearch, DEBOUNCE_MS);
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

    // Inline resolve (auto replace)
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
                if (data && data.found) {
                    const display = data.displayName || raw;
                    const id = data.username || data.fullUser || raw;
                    visibleEl.value = `${display} - ${id}`;
                    hiddenEl.value = `${display} - ${id}`;
                    markValid(visibleEl);
                    if (infoEl) infoEl.textContent = id ? `✅ ${display} (${id})` : `✅ ${display}`;
                } else {
                    markInvalid(visibleEl);
                    hiddenEl.value = '';
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
            if (!shouldResolve(raw)) { clearValidity(visibleEl); if (infoEl) infoEl.textContent = ''; return; }
            clearTimeout(t);
            t = setTimeout(() => resolveAndReplace(raw), DEBOUNCE_MS);
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

    setupLookup('instructor-lookup', 'OficialInstName', 'OficialInstInfo');
    setupLookup('investigated-lookup', 'InvestigatedName', 'InvestigatedInfo');
});
