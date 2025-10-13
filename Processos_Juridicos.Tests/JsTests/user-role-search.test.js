/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeEach } from 'vitest';

// --- Mock lookup-utils functions ---
vi.mock('../../Processos-Juridicos/wwwroot/js/lookup-utils.js', () => ({
    clearValidity: vi.fn(),
    shouldResolve: vi.fn().mockReturnValue(true),
    searchUsers: vi.fn(),
    resolveId: vi.fn(),
    markInvalid: vi.fn(),
    markValid: vi.fn(),
}));

import * as utils from '../../Processos-Juridicos/wwwroot/js/lookup-utils.js';

describe('user-role-search.js', () => {
    beforeEach(async () => {
        document.body.innerHTML = `
      <div id="adPickerModal"></div>
      <input id="adSearchBox" />
      <button id="adSearchBtn"></button>
      <table><tbody id="adResultsBody"></tbody></table>
      <div id="adSearchStatus"></div>
      <button id="adClearBtn"></button>

      <input id="nii-lookup" />
      <input id="UserNii" />
      <div id="user-name"></div>
      <div id="UserInfo"></div>

      <button class="ad-lookup-btn"></button>
    `;

        global.bootstrap = {
            Modal: vi.fn().mockImplementation(() => ({ show: vi.fn(), hide: vi.fn() })),
        };

        vi.clearAllMocks();

        // Import the module (attaches DOMContentLoaded listener)
        await import('../../Processos-Juridicos/wwwroot/js/user-role-search.js');

        // Fire DOMContentLoaded so listeners are actually registered
        document.dispatchEvent(new Event('DOMContentLoaded'));
    });

    test('clicking search button calls searchUsers and renders results', async () => {
        utils.searchUsers.mockResolvedValueOnce([
            { displayName: 'John Doe', samAccountName: 'jdoe', email: 'jdoe@example.com' },
        ]);

        const searchBox = document.getElementById('adSearchBox');
        searchBox.value = 'jdoe';
        document.getElementById('adSearchBtn').click();

        await Promise.resolve();

        expect(utils.searchUsers).toHaveBeenCalledWith('jdoe');
        expect(document.getElementById('adResultsBody').textContent).toContain('John Doe');
        expect(document.getElementById('adSearchStatus').textContent).toMatch(/Encontrados 1/);
    });

    test('clear button empties search and results', () => {
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

    test('blur on nii-lookup calls resolveId and marks valid when found', async () => {
        utils.resolveId.mockResolvedValueOnce({ found: true, displayName: 'Jane' });

        const niiInput = document.getElementById('nii-lookup');
        niiInput.value = 'jjane';
        niiInput.dispatchEvent(new Event('blur'));

        await Promise.resolve();

        expect(utils.resolveId).toHaveBeenCalledWith('jjane');
        expect(utils.markValid).toHaveBeenCalledWith(niiInput);
        expect(document.getElementById('UserInfo').textContent).toContain('✅ Jane');
    });

    test('blur on nii-lookup marks invalid when not found', async () => {
        utils.resolveId.mockResolvedValueOnce({ found: false });

        const niiInput = document.getElementById('nii-lookup');
        niiInput.value = 'unknown';
        niiInput.dispatchEvent(new Event('blur'));

        await Promise.resolve();

        expect(utils.markInvalid).toHaveBeenCalledWith(niiInput);
        expect(document.getElementById('UserInfo').textContent).toContain('❌ Não encontrado');
    });
});
