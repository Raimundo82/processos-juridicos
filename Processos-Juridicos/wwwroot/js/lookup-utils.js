// adLookupUtils.js
export const markValid = (el) => { el.classList.add('is-valid'); el.classList.remove('is-invalid'); };
export const markInvalid = (el) => { el.classList.add('is-invalid'); el.classList.remove('is-valid'); };
export const clearValidity = (el) => el.classList.remove('is-valid', 'is-invalid');

export const shouldResolve = (v) => {
    if (!v) return false;
    const raw = v.trim();
    if (raw.length < 3) return false;
    return raw.includes('@') || raw.includes('\\') || !raw.includes(' ');
};

export const searchUsers = async (q) => {
    const res = await fetch(`/api/directory/search?query=${encodeURIComponent(q)}`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    return res.json();
};

export const resolveId = async (rawId) => {
    const res = await fetch(`/api/directory/resolve/${encodeURIComponent(rawId)}`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    return res.json();
};
