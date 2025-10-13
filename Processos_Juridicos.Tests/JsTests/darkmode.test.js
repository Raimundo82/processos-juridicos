/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, beforeEach } from 'vitest';

describe('dark-mode toggle', () => {
    beforeEach(async () => {
        // Reset DOM
        document.body.innerHTML = `<button id="nightModeToggle">Toggle</button>`;
        document.body.className = '';
        document.cookie = '';

        // Import the module fresh each time so it re-attaches the listener
        await import('../../Processos-Juridicos/wwwroot/js/dark-mode.js');
    });

    test('removes dark-mode class and sets cookie to light on second click', () => {
        const btn = document.getElementById('nightModeToggle');

        // First click: add dark
        btn.click();
        // Second click: remove dark
        btn.click();

        expect(document.body.classList.contains('dark-mode')).toBe(false);

        // Check that the last theme cookie is light
        const cookies = document.cookie.split(';').map(c => c.trim());
        const themeCookie = cookies.filter(c => c.startsWith('theme=')).pop();
        expect(themeCookie).toBe('theme=light');
    });


    test('removes dark-mode class and sets cookie to light on second click', () => {
        const btn = document.getElementById('nightModeToggle');

        // First click: add dark
        btn.click();
        // Second click: remove dark
        btn.click();

        expect(document.body.classList.contains('dark-mode')).toBe(false);
        expect(document.cookie).toContain('theme=light');
    });
});
