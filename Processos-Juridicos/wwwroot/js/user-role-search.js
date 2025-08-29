import { markValid, markInvalid, clearValidity, shouldResolve, searchUsers, resolveId } from './lookup-utils.js';

document.addEventListener('DOMContentLoaded', () => {
    const DEBOUNCE_MS = 1000;

    // Elements
    const modalEl = document.getElementById('adPickerModal');
    const adModal = modalEl ? new bootstrap.Modal(modalEl) : null;
    const searchBox = document.getElementById('adSearchBox');
    const searchBtn = document.getElementById('adSearchBtn');
    const resultsBody = document.getElementById('adResultsBody');
    const statusEl = document.getElementById('adSearchStatus');
    const clearBtn = document.getElementById('adClearBtn');

    const niiInputEl = document.getElementById('nii-lookup');
    const hiddenNiiEl = document.getElementById('UserNii');
    const nameDisplayEl = document.getElementById('user-name');
    const infoEl = document.getElementById('UserInfo');

    const setStatus = (msg) => { if (statusEl) statusEl.textContent = msg || ''; };

    // Render modal results
    const renderResults = (items) => {
        resultsBody.innerHTML = '';
        if (!items?.length) {
            resultsBody.innerHTML = '<tr><td colspan="5" class="text-muted">Sem resultados</td></tr>';
            return;
        }
        for (const u of items) {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${u.displayName || ''}</td>
                <td>${u.samAccountName || ''}</td>
                <td>${u.email || ''}</td>
                <td>${u.department || u.company || ''}</td>
                <td><button type="button" class="btn btn-sm btn-success">Selecionar</button></td>
            `;
            tr.addEventListener('dblclick', () => applySelection(u));
            tr.querySelector('button').addEventListener('click', () => applySelection(u));
            resultsBody.appendChild(tr);
        }
    };

    // Apply selection from modal
    const applySelection = (user) => {
        const display = user.displayName || user.cn || user.name || '';
        const niiValue = user.nii || user.samAccountName || user.userPrincipalName || '';

        // Only add "m" if it doesn't already start with it (case-insensitive)
        const prefixedNii = /^m/i.test(niiValue) ? niiValue : `m${niiValue}`;

        if (nameDisplayEl) nameDisplayEl.textContent = display;
        if (hiddenNiiEl) hiddenNiiEl.value = prefixedNii;
        if (niiInputEl) {
            niiInputEl.value = prefixedNii;
            markValid(niiInputEl);
        }
        if (infoEl) infoEl.textContent = `✅ ${display} (${prefixedNii})`;

        if (adModal) adModal.hide();
    };

    // Search handling
    const doSearch = async () => {
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

    let searchTimer = null;
    if (searchBox) {
        searchBox.addEventListener('input', () => {
            clearTimeout(searchTimer);
            searchTimer = setTimeout(doSearch, DEBOUNCE_MS);
        });
    }
    if (searchBtn) searchBtn.addEventListener('click', doSearch);
    if (clearBtn) clearBtn.addEventListener('click', () => {
        searchBox.value = '';
        renderResults([]);
        setStatus('');
        searchBox.focus();
    });

    // Open modal
    document.querySelectorAll('.ad-lookup-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            renderResults([]);
            setStatus('');
            adModal.show();
            setTimeout(() => searchBox && searchBox.focus(), 150);
        });
    });

    // Inline resolve for manual NII typing
    const resetState = () => {
        clearValidity(niiInputEl);
        if (hiddenNiiEl) hiddenNiiEl.value = '';
        if (nameDisplayEl) nameDisplayEl.textContent = 'Nenhum utilizador selecionado';
        if (infoEl) infoEl.textContent = '';
    };

    const resolveAndReplace = async (raw) => {
        try {
            const data = await resolveId(raw);

            if (data?.found) {
                const prefixedNii = raw.startsWith('m') ? raw : `m${raw}`;
                if (hiddenNiiEl) hiddenNiiEl.value = prefixedNii;
                if (niiInputEl) {
                    niiInputEl.value = prefixedNii;
                    markValid(niiInputEl);
                }
                if (nameDisplayEl) nameDisplayEl.textContent = data.displayName || raw;
                if (infoEl) infoEl.textContent = `✅ ${data.displayName || ''} (${prefixedNii})`;
                return;
            }

            // Not found
            markInvalid(niiInputEl);
            resetState();
            if (infoEl) infoEl.textContent = `❌ Não encontrado: ${raw}`;

        } catch {
            markInvalid(niiInputEl);
            if (infoEl) infoEl.textContent = '⚠️ Erro ao validar ID';
        }
    };

    let t = null;
    if (niiInputEl) {
        niiInputEl.addEventListener('input', () => {
            const raw = (niiInputEl.value || '').trim();
            if (!raw) { resetState(); return; }
            if (!shouldResolve(raw)) {
                clearValidity(niiInputEl);
                if (infoEl) infoEl.textContent = '';
                return;
            }
            clearTimeout(t);
            t = setTimeout(() => resolveAndReplace(raw), DEBOUNCE_MS);
        });
        niiInputEl.addEventListener('blur', () => {
            const raw = (niiInputEl.value || '').trim();
            if (shouldResolve(raw)) {
                clearTimeout(t);
                resolveAndReplace(raw);
            } else if (!raw) {
                resetState();
            }
        });
    }
});
