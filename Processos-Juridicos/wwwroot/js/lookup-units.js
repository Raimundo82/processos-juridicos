// page-badge.js
import { initAdLookup } from './adLookup.js';

// Adds a removable badge with hidden input to a container
const applySelectionBadge = (user, target, adModal) => {
    const hiddenContainer = document.getElementById(target.hiddenId);
    const info = document.getElementById(target.infoId);

    const display = user.displayName || user.cn || user.name || '';
    const id = user.samAccountName || user.userPrincipalName || user.employeeId || '';

    // Prevent duplicates
    if (hiddenContainer.querySelector(`input[value="${id}"]`)) {
        if (adModal) adModal.hide();
        return;
    }

    // Badge wrapper
    const wrapper = document.createElement("span");
    wrapper.className = "badge bg-primary m-1 d-inline-flex align-items-center";

    // Hidden input for form submission
    const input = document.createElement("input");
    input.type = "hidden";
    input.name = "ResponsibleUserIds";
    input.value = id;
    wrapper.appendChild(input);

    // Text label
    const label = document.createElement("span");
    label.textContent = display;
    wrapper.appendChild(label);

    // Remove button
    const removeBtn = document.createElement("button");
    removeBtn.type = "button";
    removeBtn.className = "btn-close btn-close-white ms-2";
    removeBtn.setAttribute("aria-label", "Remove");
    removeBtn.addEventListener("click", () => wrapper.remove());
    wrapper.appendChild(removeBtn);

    // Add to container
    hiddenContainer.appendChild(wrapper);

    // Optional info text
    if (info) {
        info.textContent = `✅ Added ${display} (${id})`;
    }

    if (adModal) adModal.hide();
};

document.addEventListener('DOMContentLoaded', () => {
    initAdLookup({
        applySelection: applySelectionBadge,
        setupIds: [
            ['instructor-lookup', 'OficialInstName', 'OficialInstInfo'],
            ['investigated-lookup', 'InvestigatedName', 'InvestigatedInfo'],
            ['responsible-users-visible', 'responsible-users-container', 'responsible-users-info']
        ],
        debounceMs: 1000
    });
});
