/**
 * @vitest-environment jsdom
 */

import { describe, test, expect, vi, beforeEach } from 'vitest';

// --- Mock the lookup-utils functions ---
vi.mock('../../Processos-Juridicos/wwwroot/js/lookup-utils.js', () => ({
    clearValidity: vi.fn(),
    shouldResolve: vi.fn().mockReturnValue(true),
    searchUsers: vi.fn(),
    resolveId: vi.fn(),
    markInvalid: vi.fn()
}));

// --- Import the module under test ---
import { escapeHtml, initAdLookup } from '../../Processos-Juridicos/wwwroot/js/adLookup.js';
import * as utils from '../../Processos-Juridicos/wwwroot/js/lookup-utils.js';

// --- Tests for escapeHtml ---
describe('escapeHtml', () => {
    test('escapes special characters', () => {
        expect(escapeHtml('<div>& " \' >')).toBe('&lt;div&gt;&amp; &quot; &#39; &gt;');
    });

    test('handles null/undefined', () => {
        expect(escapeHtml(null)).toBe('');
        expect(escapeHtml(undefined)).toBe('');
    });
});

// --- Tests for initAdLookup ---
describe('initAdLookup', () => {
    let applySelection;

    beforeEach(() => {
        document.body.innerHTML = `
      <div id="adPickerModal"></div>
      <input id="adSearchBox" />
      <button id="adSearchBtn"></button>
      <table><tbody id="adResultsBody"></tbody></table>
      <div id="adSearchStatus"></div>
      <button id="adClearBtn"></button>
      <input id="visible" />
      <input id="hidden" />
      <div id="info"></div>
      <button class="ad-lookup-btn"
        data-target-visible="visible"
        data-target-hidden="hidden"
        data-target-info="info"></button>
    `;

        // Fake bootstrap.Modal
        global.bootstrap = {
            Modal: vi.fn().mockImplementation(() => ({ show: vi.fn(), hide: vi.fn() }))
        };

        applySelection = vi.fn();
        vi.clearAllMocks();
    });

    test('clicking search button calls searchUsers and renders results', async () => {
        utils.searchUsers.mockResolvedValueOnce([
            { displayName: 'John Doe', samAccountName: 'jdoe', email: 'jdoe@example.com' }
        ]);

        initAdLookup({ applySelection, setupIds: [['visible', 'hidden', 'info']], debounceMs: 0 });

        const searchBox = document.getElementById('adSearchBox');
        searchBox.value = 'jdoe';
        document.getElementById('adSearchBtn').click();

        // wait for microtasks
        await Promise.resolve();

        expect(utils.searchUsers).toHaveBeenCalledWith('jdoe');
        expect(document.getElementById('adResultsBody').textContent).toContain('John Doe');
        expect(document.getElementById('adSearchStatus').textContent).toMatch(/Encontrados 1/);
    });

    test('clear button empties search and results', () => {
        initAdLookup({ applySelection, setupIds: [['visible', 'hidden', 'info']], debounceMs: 0 });

        const searchBox = document.getElementById('adSearchBox');
        const resultsBody = document.getElementById('adResultsBody');
        const statusEl = document.getElementById('adSearchStatus');

        searchBox.value = 'something';
        resultsBody.innerHTML = '<tr><td>Old</td></tr>';
        statusEl.textContent = 'Old status';

        document.getElementById('adClearBtn').click();

        expect(searchBox.value).toBe('');
        expect(resultsBody.textContent).toContain('Sem resultados');
        expect(statusEl.textContent).toBe('');
    });

    test('blur on visible input calls resolveId and marks valid', async () => {
        utils.resolveId.mockResolvedValueOnce({ displayName: 'Jane', nii: 'jjane' });

        initAdLookup({ applySelection, setupIds: [['visible', 'hidden', 'info']], debounceMs: 0 });

        const visible = document.getElementById('visible');
        visible.value = 'jjane';
        visible.dispatchEvent(new Event('blur'));

        await Promise.resolve();

        expect(utils.resolveId).toHaveBeenCalledWith('jjane');
        expect(applySelection).toHaveBeenCalled();
    });

    test('blur on visible input marks invalid when not found', async () => {
        initAdLookup({ applySelection, setupIds: [['visible', 'hidden', 'info']], debounceMs: 0 });

        const visible = document.getElementById('visible');
        visible.value = 'unknown';
        visible.dispatchEvent(new Event('blur'));

        await Promise.resolve();

        expect(utils.markInvalid).toHaveBeenCalledWith(visible);
        expect(document.getElementById('info').textContent).toContain('❌ Não encontrado');
    });
});
