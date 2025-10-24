/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeEach } from 'vitest';

// Mock lookup-utils and adLookup
vi.mock('../../Processos-Juridicos/wwwroot/js/lookup-utils.js', () => ({
    markValid: vi.fn(),
}));
vi.mock('../../Processos-Juridicos/wwwroot/js/adLookup.js', () => ({
    initAdLookup: vi.fn(),
}));

import { markValid } from '../../Processos-Juridicos/wwwroot/js/lookup-utils.js';
import { initAdLookup } from '../../Processos-Juridicos/wwwroot/js/adLookup.js';

describe('lookup.js', () => {
    let applySelection;

    beforeEach(async () => {
        document.body.innerHTML = `
      <input id="visible" />
      <input id="hidden1" />
      <input id="hidden2" />
      <div id="info"></div>
    `;

        vi.clearAllMocks();

        // Import the module under test – this will call initAdLookup with applySelectionSingle
        await import('../../Processos-Juridicos/wwwroot/js/lookup.js');

        // Fire DOMContentLoaded so initAdLookup is actually called
        document.dispatchEvent(new Event('DOMContentLoaded'));

        // Grab the applySelection function that was passed into initAdLookup
        applySelection = initAdLookup.mock.calls[0][0].applySelection;
    });

    test('fills visible, hidden, info and calls markValid', () => {
        const visible = document.getElementById('visible');
        const hidden1 = document.getElementById('hidden1');
        const hidden2 = document.getElementById('hidden2');
        const info = document.getElementById('info');
        const adModal = { hide: vi.fn() };

        const user = { displayName: 'Alice', nii: 'M123' };
        const target = { visibleId: 'visible', hiddenId: 'hidden1', hiddenId2: 'hidden2', infoId: 'info' };

        applySelection(user, target, adModal);

        expect(visible.value).toBe('Alice - M123');
        expect(hidden1.value).toBe('Alice');
        expect(hidden2.value).toBe('M123');
        expect(info.textContent).toBe('✅ Alice (M123)');
        expect(markValid).toHaveBeenCalledWith(visible);
        expect(adModal.hide).toHaveBeenCalled();
    });

    test('handles missing employeeId gracefully', () => {
        const visible = document.getElementById('visible');
        const hidden1 = document.getElementById('hidden1');
        const info = document.getElementById('info');

        const user = { displayName: 'Bob' };
        const target = { visibleId: 'visible', hiddenId: 'hidden1', infoId: 'info' };

        applySelection(user, target, null);

        expect(visible.value).toBe('Bob - ');
        expect(hidden1.value).toBe('Bob');
        expect(info.textContent).toBe('✅ Bob');
    });

    test('DOMContentLoaded handler calls initAdLookup with correct config', () => {
        expect(initAdLookup).toHaveBeenCalledWith(
            expect.objectContaining({
                applySelection: expect.any(Function),
                setupIds: [
                    ['instructor-lookup', 'OficialInstName', 'OficialInstInfo'],
                    ['investigated-lookup', 'InvestigatedName', 'InvestigatedInfo'],
                ],
                debounceMs: 1000,
            })
        );
    });
});
