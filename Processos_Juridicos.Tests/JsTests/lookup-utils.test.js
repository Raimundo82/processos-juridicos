/**
 * @vitest-environment jsdom
 */
import { describe, test, expect, vi, beforeEach } from 'vitest';
import {
    markValid,
    markInvalid,
    clearValidity,
    shouldResolve,
    searchUsers,
    resolveId,
} from '../../Processos-Juridicos/wwwroot/js/lookup-utils.js';

describe('lookup-utils.js', () => {
    let el;

    beforeEach(() => {
        el = document.createElement('input');
        el.className = '';
        vi.restoreAllMocks();
    });

    describe('markValid / markInvalid / clearValidity', () => {
        test('markValid adds is-valid and removes is-invalid', () => {
            el.classList.add('is-invalid');
            markValid(el);
            expect(el.classList.contains('is-valid')).toBe(true);
            expect(el.classList.contains('is-invalid')).toBe(false);
        });

        test('markInvalid adds is-invalid and removes is-valid', () => {
            el.classList.add('is-valid');
            markInvalid(el);
            expect(el.classList.contains('is-invalid')).toBe(true);
            expect(el.classList.contains('is-valid')).toBe(false);
        });

        test('clearValidity removes both classes', () => {
            el.classList.add('is-valid', 'is-invalid');
            clearValidity(el);
            expect(el.classList.contains('is-valid')).toBe(false);
            expect(el.classList.contains('is-invalid')).toBe(false);
        });
    });

    describe('shouldResolve', () => {
        test('returns false for empty or short values', () => {
            expect(shouldResolve('')).toBe(false);
            expect(shouldResolve('ab')).toBe(false);
        });

        test('returns true for email-like values', () => {
            expect(shouldResolve('user@example.com')).toBe(true);
        });

        test('returns true for domain\\user values', () => {
            expect(shouldResolve('DOMAIN\\user')).toBe(true);
        });

        test('returns false for values with spaces', () => {
            expect(shouldResolve('john doe')).toBe(false);
        });

        test('returns true for plain strings without spaces', () => {
            expect(shouldResolve('jdoe')).toBe(true);
        });
    });

    describe('searchUsers', () => {
        test('calls fetch with encoded query and returns json', async () => {
            const fakeJson = [{ displayName: 'Alice' }];
            global.fetch = vi.fn().mockResolvedValue({
                ok: true,
                json: () => Promise.resolve(fakeJson),
            });

            const result = await searchUsers('a b');
            expect(global.fetch).toHaveBeenCalledWith('/api/directory/search?query=a%20b');
            expect(result).toEqual(fakeJson);
        });

        test('throws on non-ok response', async () => {
            global.fetch = vi.fn().mockResolvedValue({ ok: false, status: 500 });
            await expect(searchUsers('x')).rejects.toThrow(/HTTP 500/);
        });
    });

    describe('resolveId', () => {
        test('calls fetch with encoded id and returns json', async () => {
            const fakeJson = { found: true };
            global.fetch = vi.fn().mockResolvedValue({
                ok: true,
                json: () => Promise.resolve(fakeJson),
            });

            const result = await resolveId('abc/123');
            expect(global.fetch).toHaveBeenCalledWith('/api/directory/resolve/abc%2F123');
            expect(result).toEqual(fakeJson);
        });

        test('throws on non-ok response', async () => {
            global.fetch = vi.fn().mockResolvedValue({ ok: false, status: 404 });
            await expect(resolveId('bad')).rejects.toThrow(/HTTP 404/);
        });
    });
});
