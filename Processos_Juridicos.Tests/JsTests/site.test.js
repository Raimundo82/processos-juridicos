/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeEach } from 'vitest';

describe('delete/reject modals and loader script', () => {
    let showSpy;

    beforeEach(async () => {
        document.body.innerHTML = `
      <div id="deleteModal"></div>
      <span id="deleteEntity"></span>
      <span id="deleteName"></span>
      <input id="deleteId" />
      <form id="deleteForm"></form>
      <button class="btn-delete"
        data-entity="User"
        data-name="Alice"
        data-id="42"
        data-controller="Users"
        data-action="Delete"></button>

      <div id="rejectModal"></div>
      <button class="btn-reject"></button>
      <button id="btnConfirmReject"></button>

      <div id="tableLoader"></div>
      <div id="tableContent" class="d-none"></div>

      <select id="UnitId"></select>
      <select id="Infringements"></select>
    `;

        // Mock bootstrap.Modal
        showSpy = vi.fn();
        global.bootstrap = {
            Modal: vi.fn().mockImplementation(() => ({ show: showSpy })),
        };

        // Mock jQuery select2
        global.$ = (sel) => ({
            select2: vi.fn(),
        });

        // Mock history.back
        vi.spyOn(window.history, 'back').mockImplementation(() => { });

        // Import the module fresh
        await import('../../Processos-Juridicos/wwwroot/js/site.js');
        document.dispatchEvent(new Event('DOMContentLoaded'));
    });

    test('clicking .btn-delete fills form and shows modal', () => {
        const btn = document.querySelector('.btn-delete');
        btn.click();

        expect(document.getElementById('deleteId').value).toBe('42');
        expect(document.getElementById('deleteForm').action).toContain('/Users/Delete');
        expect(document.getElementById('deleteEntity').textContent).toBe('User');
        expect(document.getElementById('deleteName').textContent).toBe(' Alice');
        expect(showSpy).toHaveBeenCalled();
    });

    test('clicking .btn-reject shows reject modal', () => {
        const btn = document.querySelector('.btn-reject');
        btn.click();
        expect(showSpy).toHaveBeenCalled();
    });

    test('clicking #btnConfirmReject calls history.back', () => {
        document.getElementById('btnConfirmReject').click();
        expect(window.history.back).toHaveBeenCalled();
    });

    test('loader hides tableLoader and shows tableContent', () => {
        expect(document.getElementById('tableLoader').classList.contains('d-none')).toBe(true);
        expect(document.getElementById('tableContent').classList.contains('d-none')).toBe(false);
    });
});
