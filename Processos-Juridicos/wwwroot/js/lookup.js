// page-single.js
import { markValid } from './lookup-utils.js';
import { initAdLookup } from './adLookup.js';

// Fills one visible + one hidden input, and sets info text
const applySelectionSingle = (user, target, adModal) => {
    const visible = document.getElementById(target.visibleId);
    const hidden1 = document.getElementById(target.hiddenId);   // always present
    const hidden2 = target.hiddenId2 ? document.getElementById(target.hiddenId2) : null;
    const info = document.getElementById(target.infoId);

    const display = user.displayName || user.cn || user.name || '';
    const empId = user.employeeId || user.samAccountName || user.userPrincipalName || '';

    if (visible) visible.value = `${display} - M${empId}`;
    hidden1.value = `${display}`;

    if (hidden2) {
        hidden2.value = `M${empId}`;   // second hidden gets the employee ID
    }

    if (visible) markValid(visible);
    if (info) info.textContent = empId ? `✅ ${display} (${empId})` : `✅ ${display}`;

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
