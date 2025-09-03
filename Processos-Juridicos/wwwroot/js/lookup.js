// page-single.js
import { markValid } from './lookup-utils.js';
import { initAdLookup } from './adLookup.js';

// Fills one visible + one hidden input, and sets info text
const applySelectionSingle = (user, target, adModal) => {
    const visible = document.getElementById(target.visibleId);
    const hidden = document.getElementById(target.hiddenId);
    const info = document.getElementById(target.infoId);

    const display = user.displayName || user.cn || user.name || '';
    const id = user.samAccountName || user.userPrincipalName || user.employeeId || '';

    if (visible) visible.value = `${display} - ${id}`;
    if (hidden) hidden.value = `${display} - ${id}`;
    if (visible) markValid(visible);
    if (info) info.textContent = id ? `✅ ${display} (${id})` : `✅ ${display}`;

    if (adModal) adModal.hide();
};

document.addEventListener('DOMContentLoaded', () => {
    initAdLookup({
        applySelection: applySelectionSingle,
        setupIds: [
            ['instructor-lookup', 'OficialInstName', 'OficialInstInfo'],
            ['investigated-lookup', 'InvestigatedName', 'InvestigatedInfo']
        ],
        debounceMs: 1000
    });
});
