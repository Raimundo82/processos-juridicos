import { initAdLookup } from './adLookup.js';

const $ = id => document.getElementById(id);
const setText = (el, text = '') => el && (el.textContent = text);

const createBadge = (display, id) => {
    const wrapper = document.createElement('span');
    wrapper.className = 'badge bg-primary m-1 d-inline-flex align-items-center';

    const input = Object.assign(document.createElement('input'), {
        type: 'hidden',
        name: 'ResponsibleUserIds',
        value: id
    });

    const label = document.createElement('span');
    label.textContent = display;

    const removeBtn = Object.assign(document.createElement('button'), {
        type: 'button',
        className: 'btn-close btn-close-white ms-2'
    });
    removeBtn.setAttribute('aria-label', 'Remove');
    removeBtn.addEventListener('click', () => wrapper.remove());

    wrapper.append(input, label, removeBtn);
    return wrapper;
};

const applySelectionBadge = (user, target, adModal) => {
    const hiddenContainer = $(target.hiddenId);
    const info = $(target.infoId);

    const display = user.displayName || user.cn || user.name || '';
    const id = user.samAccountName || user.userPrincipalName || user.employeeId || '';

    if (hiddenContainer.querySelector(`input[value="${id}"]`)) {
        adModal?.hide();
        return;
    }


    const input = document.querySelector(`input[value="${user.employeeId}"]`);

    if (input) {
        hiddenContainer.appendChild(createBadge(display, id));
        setText(info, `✅ Added ${display} (${id})`);
        adModal?.hide();
    } else {
        setText(info, `User ${display} (${id}) was already added `);
    }

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

export { createBadge, applySelectionBadge };
