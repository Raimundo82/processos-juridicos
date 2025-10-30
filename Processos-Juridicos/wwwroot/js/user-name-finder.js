document.addEventListener('DOMContentLoaded', () => {
    const $ = id => document.getElementById(id);
    const setText = (el, text = '') => el && (el.value = text);

    const resolveId = async rawId => {
        const res = await fetch(`/api/directory/resolve/${encodeURIComponent(rawId)}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
    };

    document.getElementById("user-lookup").disabled = true;
    document.querySelector('.ad-lookup-btn').style.visibility = 'hidden';

    const niiEl = $('UserNii');
    const nameEl = $('user-lookup');

    if (!niiEl || !nameEl) return;

    const nii = (niiEl.value || '').trim().split('-').pop().trim();
    if (!nii) return setText(nameEl, 'Nenhum NII encontrado');

    (async () => {
        try {
            const data = await resolveId(nii);
            if (data?.userName) {
                const display = data.displayName || data.cn || data.name || nii;
                const id = data.username || nii;
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
