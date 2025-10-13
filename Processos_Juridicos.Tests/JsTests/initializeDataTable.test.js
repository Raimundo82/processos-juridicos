/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeAll, beforeEach } from 'vitest';

let createDataTable, loadProcessFilters, fillSelect;

beforeAll(async () => {
    //ensure $(selector) returns the SAME object for that selector
    const cache = new Map();

    const makeTableMock = (selector) => {
        const obj = {
            // jQuery-like length property reflecting DOM
            length: () => {
                const el = document.querySelector(selector);
                return el ? 1 : 0;
            },
            find: (sel) => {
                const el = document.querySelector(selector);
                return { length: el ? el.querySelectorAll(sel).length : 0 };
            },
            DataTable: vi.fn(),
        };
        return obj;
    };

    const makeElementMock = (selector) => {
        const obj = {
            empty: vi.fn().mockReturnThis(),
            append: vi.fn().mockReturnThis(),
            on: vi.fn(),
            select2: vi.fn(),
            // DataTable return object used in ready() block
            DataTable: vi.fn().mockReturnValue({
                column: vi.fn().mockReturnThis(),
                search: vi.fn().mockReturnThis(),
                draw: vi.fn().mockReturnThis(),
            }),
            // jQuery-like helpers
            length: () => (document.querySelector(selector) ? 1 : 0),
            find: (sel) => ({
                length: document.querySelector(selector)
                    ? document.querySelector(selector).querySelectorAll(sel).length
                    : 0,
            }),
        };
        return obj;
    };

    const fake$ = (selector) => {
        // $(document).ready(...)
        if (selector === document) {
            if (!cache.has('document')) {
                cache.set('document', {
                    ready: (cb) => cb(),
                });
            }
            return cache.get('document');
        }

        // Ensure we reuse the same mock per selector
        if (cache.has(selector)) return cache.get(selector);

        let mock;
        if (typeof selector === 'string' && (selector === '#myTable' || selector === '#tableStates')) {
            mock = makeTableMock(selector);
        } else if (typeof selector === 'string' && selector.startsWith('#')) {
            mock = makeElementMock(selector);
        } else {
            mock = {
                length: () => 0,
                find: () => ({ length: 0 }),
                DataTable: vi.fn(),
            };
        }

        cache.set(selector, mock);
        return mock;
    };

    // Plugin stubs
    fake$.getJSON = vi.fn();
    fake$.fn = { select2: vi.fn() };

    global.$ = fake$;

    // Dynamically import AFTER stubbing $
    const mod = await import('../../Processos-Juridicos/wwwroot/js/initializeDataTable.js');
    createDataTable = mod.createDataTable;
    loadProcessFilters = mod.loadProcessFilters;
    fillSelect = mod.fillSelect;
});

describe('initializeDataTable.js', () => {
    beforeEach(() => {
        // Set up DOM reflecting your code’s expectations
        document.body.innerHTML = `
      <table id="myTable"><tbody><tr><td>row</td></tr></tbody></table>
      <table id="tableStates"><tbody><tr><td>row</td></tr></tbody></table>
      <table id="processesTable"><tbody><tr><td>row</td></tr></tbody></table>
      <select id="unit"></select>
      <select id="type"></select>
      <select id="state"></select>
      <select id="unitFilter"></select>
      <select id="typeFilter"></select>
      <select id="stateFilter"></select>
    `;
        vi.clearAllMocks();
    });

    test('createDataTable calls DataTable with options', () => {
        const tableMock = global.$('#myTable'); // cached instance
        createDataTable('#myTable');

        expect(tableMock.DataTable).toHaveBeenCalledTimes(1);
        const opts = tableMock.DataTable.mock.calls[0][0];

        expect(opts).toMatchObject({
            language: expect.any(Object),
            infoCallback: expect.any(Function),
            columnDefs: [{ orderable: false, targets: -1 }],
        });

        // Verify infoCallback wraps numbers
        const rendered = opts.infoCallback(null, 1, 10, 100, 100, 'A mostrar 1 a 10 de 100 registos');
        expect(rendered).toContain('<strong>1</strong>');
        expect(rendered).toContain('<strong>10</strong>');
        expect(rendered).toContain('<strong>100</strong>');
    });

    test('createDataTable does not add columnDefs for #tableStates', () => {
        const tableMock = global.$('#tableStates'); // cached instance
        createDataTable('#tableStates');

        expect(tableMock.DataTable).toHaveBeenCalledTimes(1);
        const opts = tableMock.DataTable.mock.calls[0][0];
        expect(opts.columnDefs).toBeUndefined();
    });

    test('fillSelect empties and appends options', () => {
        const selectMock = global.$('#unit'); // cached instance

        fillSelect('#unit', ['A', 'B']);

        expect(selectMock.empty).toHaveBeenCalledTimes(1);
        expect(selectMock.append).toHaveBeenCalledWith('<option value="">Todos</option>');
        expect(selectMock.append).toHaveBeenCalledWith('<option value="A">A</option>');
        expect(selectMock.append).toHaveBeenCalledWith('<option value="B">B</option>');
    });

    test('loadProcessFilters calls getJSON and fills selects', () => {
        const data = { units: ['U'], types: ['T'], states: ['S'] };
        global.$.getJSON.mockImplementation((url, cb) => cb(data));

        loadProcessFilters('/Process/GetFilterValues', '#unit', '#type', '#state');

        expect(global.$.getJSON).toHaveBeenCalledWith('/Process/GetFilterValues', expect.any(Function));

        const unitMock = global.$('#unit');   // same cached instance used internally
        const typeMock = global.$('#type');
        const stateMock = global.$('#state');

        // Default option + each value should be appended
        expect(unitMock.append).toHaveBeenCalledWith('<option value="">Todos</option>');
        expect(typeMock.append).toHaveBeenCalledWith('<option value="">Todos</option>');
        expect(stateMock.append).toHaveBeenCalledWith('<option value="">Todos</option>');

        expect(unitMock.append).toHaveBeenCalledWith('<option value="U">U</option>');
        expect(typeMock.append).toHaveBeenCalledWith('<option value="T">T</option>');
        expect(stateMock.append).toHaveBeenCalledWith('<option value="S">S</option>');
    });
});
