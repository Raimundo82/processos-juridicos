document.addEventListener('DOMContentLoaded', () => {
    const resolveId = async (rawId) => {
        const res = await fetch(`/api/directory/resolve/${encodeURIComponent(rawId)}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
    };

    // --- Elements ---
    const niiEl = document.getElementById('user-nii');
    const nameEl = document.getElementById('user-name');

    if (!niiEl || !nameEl) return;

    // --- Grab and clean the NII ---
    const rawText = (niiEl.textContent || '').trim();
    const nii = rawText.includes('-') ? rawText.split('-').pop().trim() : rawText;

    if (!nii) {
        nameEl.textContent = 'Nenhum NII encontrado';
        return;
    }

    // --- Resolve on page load ---
    (async () => {
        try {
            const data = await resolveId(nii);
            if (data?.found) {
                const display = data.displayName || data.cn || data.name || nii;
                const id = data.username || data.fullUser || nii;
                nameEl.textContent = id ? `${display} (${id})` : ` ${display}`;
            } else {
                nameEl.textContent = `❌ Utilizador não encontrado: ${nii}`;
            }
        } catch (err) {
            console.error(err);
            nameEl.textContent = '⚠️ Erro ao obter o nome do utilizador';
        }
    })();
});
