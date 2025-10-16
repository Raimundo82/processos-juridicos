/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeAll, beforeEach } from 'vitest';

let markFileForDeletion, toggleReportToPjm;

beforeAll(async () => {
    const cache = new Map();

    const makeElementMock = (selector) => {
        const el = () => document.querySelector(selector); // lazy lookup
        return {
            prop: vi.fn((prop, val) => {
                const node = el();
                if (node) {
                    if (prop === 'checked' && val !== undefined) node.checked = val;
                    if (prop === 'disabled' && val !== undefined) node.disabled = val;
                }
                return cache.get(selector);
            }),
            on: vi.fn((event, handler) => {
                const node = el();
                if (node) node.addEventListener(event, handler);
            }),
            text: () => el()?.textContent || '',
        };
    };


    const fake$ = (selector) => {
        if (selector === document) {
            if (!cache.has('document')) {
                cache.set('document', { ready: (cb) => cb() });
            }
            return cache.get('document');
        }

        if (cache.has(selector)) return cache.get(selector);

        let mock;
        if (selector === '#ProcessTypeId option:selected') {
            const sel = document.querySelector('#ProcessTypeId');
            return { text: () => sel?.selectedOptions[0]?.textContent || '' };
        } else if (typeof selector === 'string' && selector.startsWith('#')) {
            mock = makeElementMock(selector);
        } else if (selector instanceof HTMLElement) {
            // If code calls $(element), just return a minimal mock
            mock = makeElementMock('#' + selector.id);
        } else {
            mock = {};
        }

        cache.set(selector, mock);
        return mock;
    };

    global.$ = fake$;

    // Import after $ is defined
    const mod = await import('../../Processos-Juridicos/wwwroot/js/forms.js');
    markFileForDeletion = mod.markFileForDeletion;
    toggleReportToPjm = mod.toggleReportToPjm;
});

describe('forms.js', () => {
    beforeEach(() => {
        document.body.innerHTML = `
      <div id="deletedFilesContainer"></div>
      <table>
        <tr id="file-row-42"><td>file</td></tr>
      </table>
      <select id="ProcessTypeId">
        <option>Other</option>
        <option>Acidentes em serviço</option>
      </select>
      <input id="ComunicatedToPjm" type="checkbox" />
    `;
        vi.clearAllMocks();
    });

    test('markFileForDeletion removes row and appends hidden input', () => {
        markFileForDeletion(42);

        expect(document.getElementById('file-row-42')).toBeNull();

        const hidden = document.querySelector('#deletedFilesContainer input[type="hidden"]');
        expect(hidden).not.toBeNull();
        expect(hidden.name).toBe('FilesToRemove');
        expect(hidden.value).toBe('42');
    });

    test('toggleReportToPjm disables and unchecks when type is "Acidentes em serviço"', () => {
        // Select the second option
        const opts = document.querySelectorAll('#ProcessTypeId option');
        opts[0].selected = false;
        opts[1].selected = true;

        toggleReportToPjm();

        const checkbox = document.getElementById('ComunicatedToPjm');
        expect(checkbox.disabled).toBe(true);
        expect(checkbox.checked).toBe(false);
    });

    test('toggleReportToPjm enables when type is not "Acidentes em serviço"', () => {
        // Select the first option
        const opts = document.querySelectorAll('#ProcessTypeId option');
        opts[0].selected = true;
        opts[1].selected = false;

        toggleReportToPjm();

        const checkbox = document.getElementById('ComunicatedToPjm');
        expect(checkbox.disabled).toBe(false);
    });
});
