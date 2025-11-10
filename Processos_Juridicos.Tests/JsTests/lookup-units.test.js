/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeEach } from 'vitest';

// Mock initAdLookup so DOMContentLoaded wiring doesn't blow up
vi.mock('../../Processos-Juridicos/wwwroot/js/adLookup.js', () => ({
    initAdLookup: vi.fn(),
}));

import { initAdLookup } from '../../Processos-Juridicos/wwwroot/js/adLookup.js';
import {
    createBadge,
    applySelectionBadge,
} from '../../Processos-Juridicos/wwwroot/js/lookup-units.js'; // adjust path if needed

describe('createBadge', () => {
    test('creates a badge with hidden input, label, and remove button', () => {
        const badge = createBadge('Alice', '123');
        document.body.appendChild(badge); // append so remove works

        // wrapper span
        expect(badge.tagName).toBe('SPAN');
        expect(badge.className).toContain('badge');

        // hidden input
        const input = badge.querySelector('input[type="hidden"]');
        expect(input).toBeTruthy();
        expect(input.value).toBe('123');
        expect(input.name).toBe('ResponsibleUserIds');

        // label is the first child span inside wrapper
        const spans = badge.querySelectorAll('span');
        expect(spans[0].textContent).toBe('Alice (123)');

        // remove button
        const btn = badge.querySelector('button');
        expect(btn).toBeTruthy();
        btn.click();
        expect(document.body.contains(badge)).toBe(false);
    });
});

describe('applySelectionBadge', () => {
    let hiddenContainer, info, adModal;

    beforeEach(() => {
        document.body.innerHTML = `
      <div id="hidden"></div>
      <div id="info"></div>
    `;
        hiddenContainer = document.getElementById('hidden');
        info = document.getElementById('info');
        adModal = { hide: vi.fn() };
    });

    test('adds a new badge when no employeeId input exists', () => {
        document.body.innerHTML = `
      <div id="hidden"></div>
      <div id="info"></div>
    `;
        const hiddenContainer = document.getElementById('hidden');
        const info = document.getElementById('info');
        const adModal = { hide: vi.fn() };

        // Do NOT append an input with emp123
        const user = { displayName: 'Bob', employeeId: 'emp123', nii: 'bob' };
        const target = { hiddenId: 'hidden', infoId: 'info' };

        applySelectionBadge(user, target, adModal);

        const badge = hiddenContainer.querySelector('.badge');
        expect(badge).not.toBeNull();
        expect(info.textContent).toContain('✅ Adicionado Bob (bob)');
        expect(adModal.hide).toHaveBeenCalled();
    });


    test('does not add duplicate badge if one with same id already exists', () => {
        hiddenContainer.innerHTML = `<input type="hidden" value="bob" />`;

        const user = { displayName: 'Bob', nii: 'bob' };
        const target = { hiddenId: 'hidden', infoId: 'info' };

        applySelectionBadge(user, target, adModal);

        expect(hiddenContainer.querySelectorAll('.badge').length).toBe(0);
        expect(adModal.hide).toHaveBeenCalled();
    });

    test('sets info text when user already added (no matching employeeId input)', () => {
        hiddenContainer.innerHTML = `<input type="hidden" value="emp999" />`;

        const user = { displayName: 'Carol', employeeId: 'emp999', nii: 'carol' };
        const target = { hiddenId: 'hidden', infoId: 'info' };

        applySelectionBadge(user, target, adModal);

        expect(info.textContent).toContain('Utilizador Carol (carol) já é comando da unidade ou já foi adicionado');

    });
});

describe('DOMContentLoaded wiring', () => {
    test('calls initAdLookup with correct config', () => {
        document.dispatchEvent(new Event('DOMContentLoaded'));
        expect(initAdLookup).toHaveBeenCalledWith(
            expect.objectContaining({
                applySelection: expect.any(Function),
                setupIds: [
                    ['instructor-lookup', 'OficialInstName', 'OficialInstInfo'],
                    ['investigated-lookup', 'InvestigatedName', 'InvestigatedInfo'],
                    ['responsible-users-visible', 'responsible-users-container', 'responsible-users-info'],
                ],
                debounceMs: 1000,
            })
        );
    });
});
