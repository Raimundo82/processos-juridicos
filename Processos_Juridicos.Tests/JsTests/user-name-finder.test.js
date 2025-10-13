/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeEach } from 'vitest';

describe('user-name-finder.js', () => {
    beforeEach(() => {
        // Reset DOM
        document.body.innerHTML = `
      <div id="user-nii"></div>
      <div id="user-name"></div>
    `;

        // Reset fetch mock
        global.fetch = vi.fn();
    });

    async function loadModuleAndTrigger() {
        // Import the module fresh each time
        await import('../../Processos-Juridicos/wwwroot/js/user-name-finder.js');
        // Fire DOMContentLoaded so the code runs
        document.dispatchEvent(new Event('DOMContentLoaded'));

        await Promise.resolve();
    }

    test('shows "Nenhum NII encontrado" when no NII text', async () => {
        document.getElementById('user-nii').textContent = '';

        await loadModuleAndTrigger();

        expect(document.getElementById('user-name').textContent).toBe('Nenhum NII encontrado');
    });

    test('shows user display and id when resolveId returns found', async () => {
        document.getElementById('user-nii').textContent = 'abc-123';

        const jsonPromise = Promise.resolve({ found: true, displayName: 'Jane Doe', username: 'jdoe' });
        global.fetch.mockResolvedValueOnce({ ok: true, json: () => jsonPromise });

        await loadModuleAndTrigger();
        await jsonPromise; 
        await new Promise(setImmediate);

        expect(global.fetch).toHaveBeenCalledWith('/api/directory/resolve/123');
        expect(document.getElementById('user-name').textContent).toBe('Jane Doe (jdoe)');
    });

    test('shows not found message when resolveId returns not found', async () => {
        document.getElementById('user-nii').textContent = 'abc-999';

        const jsonPromise = Promise.resolve({ found: false });
        global.fetch.mockResolvedValueOnce({ ok: true, json: () => jsonPromise });

        await loadModuleAndTrigger();
        await jsonPromise;
        await new Promise(setImmediate);

        expect(document.getElementById('user-name').textContent)
            .toContain('❌ Utilizador não encontrado: 999');
    });


    test('shows error message when fetch fails', async () => {
        document.getElementById('user-nii').textContent = 'abc-err';

        global.fetch.mockResolvedValueOnce({ ok: false, status: 500 });

        await loadModuleAndTrigger();

        expect(document.getElementById('user-name').textContent)
            .toBe('⚠️ Erro ao obter o nome do utilizador');
    });
});
