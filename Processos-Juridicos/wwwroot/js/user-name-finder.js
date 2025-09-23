document.addEventListener('DOMContentLoaded', () => {
    const $ = id => document.getElementById(id);
    const setText = (el, text = '') => el && (el.textContent = text);

    const resolveId = async rawId => {
        const res = await fetch(`/api/directory/resolve/${encodeURIComponent(rawId)}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
    };

    const niiEl = $('user-nii');
    const nameEl = $('user-name');
    if (!niiEl || !nameEl) return;

    const nii = (niiEl.textContent || '').trim().split('-').pop().trim();
    if (!nii) return setText(nameEl, 'Nenhum NII encontrado');

    (async () => {
        try {
            const data = await resolveId(nii);
            if (data?.found) {
                const display = data.displayName || data.cn || data.name || nii;
                const id = data.username || data.fullUser || nii;
                setText(nameEl, id ? `${display} (${id})` : display);
            } else {
                setText(nameEl, `❌ Utilizador não encontrado: ${nii}`);
            }
        } catch (err) {
            console.error(err);
            setText(nameEl, '⚠️ Erro ao obter o nome do utilizador');
        }
    })();
});
