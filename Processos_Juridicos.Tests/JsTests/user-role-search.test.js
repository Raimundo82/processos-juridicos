// @vitest-environment jsdom
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

      <input type="hidden" id="UserNii" />
      <input type="hidden" id="UserName" />
      <input id="user-lookup" />
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

    test('blur on user-lookup calls resolveId and marks valid when found', async () => {
        utils.resolveId.mockResolvedValueOnce({
            userName: 'jjane',          // must include userName for branch to run
            displayName: 'Jane'
        });

        const visibleInput = document.getElementById('user-lookup');
        visibleInput.value = 'jjane';
        visibleInput.dispatchEvent(new Event('blur'));

        await Promise.resolve();
        await new Promise(setImmediate);

        expect(utils.resolveId).toHaveBeenCalledWith('jjane');       // string
        expect(utils.markValid).toHaveBeenCalledWith(visibleInput);  // element
        expect(document.getElementById('UserInfo').textContent).toContain('✅ Jane');
        expect(document.getElementById('UserNii').value).toBe('Mjjane'); // note M prefix
    });

    test('blur on user-lookup marks invalid when not found', async () => {
        utils.resolveId.mockResolvedValueOnce({}); // no userName property

        const visibleInput = document.getElementById('user-lookup');
        visibleInput.value = 'unknown';
        visibleInput.dispatchEvent(new Event('blur'));

        await Promise.resolve();
        await new Promise(setImmediate);

        expect(utils.markInvalid).toHaveBeenCalledWith(visibleInput); // element
        expect(document.getElementById('UserInfo').textContent).toContain('❌ Não encontrado');
        expect(document.getElementById('UserNii').value).toBe(''); // reset hidden
    });
});
