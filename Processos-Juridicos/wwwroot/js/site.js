// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Generic confirmation helper (event delegation + sensible defaults + data-* overrides)
function attachConfirmAction(selector, baseConfig = {}) {
    const defaults = {
        icon: 'warning',
        theme: 'light',          // keep if you're using sweetalert2-themes; harmless otherwise
        showCancelButton: true,
        confirmButtonText: 'Confirmar',
        cancelButtonText: 'Cancelar'
    };

    const clickHandler = async (e) => {
        const btn = e.target.closest(selector);
        if (!btn) return;

        e.preventDefault();

        // Per-button overrides via data-* attributes (optional)
        const cfg = {
            ...defaults,
            ...baseConfig,
            title: btn.dataset.confirmTitle ?? baseConfig.title,
            text: btn.dataset.confirmText ?? baseConfig.text,
            icon: btn.dataset.confirmIcon ?? baseConfig.icon ?? defaults.icon,
            confirmButtonText: btn.dataset.confirmConfirmText ?? baseConfig.confirmButtonText ?? defaults.confirmButtonText,
            cancelButtonText: btn.dataset.confirmCancelText ?? baseConfig.cancelButtonText ?? defaults.cancelButtonText,
            confirmButtonColor: btn.dataset.confirmButtonColor ?? baseConfig.confirmButtonColor,
            cancelButtonColor: btn.dataset.cancelButtonColor ?? baseConfig.cancelButtonColor
        };

        const result = await Swal.fire(cfg);
        if (result.isConfirmed) {
            if (typeof onConfirm === 'function') {
                onConfirm(btn);
            } else {
                // Default: submit surrounding form or a custom selector
                const formSelector = btn.dataset.submitSelector;
                const form = formSelector ? document.querySelector(formSelector) : btn.closest('form');
                if (form) form.submit();
            }
        }
    };

    document.addEventListener('click', clickHandler);
    // Return an unsubscribe if you ever need to detach
    return () => document.removeEventListener('click', clickHandler);
}

// Delete confirmation
attachConfirmAction('.js-delete-btn', {
    title: 'Tem a certeza que pretende apagar?',
    text: 'Esta ação é irreverssível!',
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Apagar',
    cancelButtonText: 'Cancelar'
});

// Add-role confirmation
attachConfirmAction('.js-add-role-btn', {
    title: 'Tem a certeza que pretende dar estas permissões ao utilizador?',
    text: 'Este utilizador passará a ter acesso às funcionalidades associadas à permissão selecionada.',
    confirmButtonColor: '#2D753C',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Aceitar',
    cancelButtonText: 'Cancelar'
});


// Select2 initializers
$(function () {
    $('#UnitId').select2({
        allowClear: true, theme: 'bootstrap-5', width: '100%',
    });
    $('#Infringements').select2({
        allowClear: true, closeOnSelect: false, theme: 'bootstrap-5', width: '100%',
    });
});

// Loader
document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("tableLoader").classList.add("d-none");
    document.getElementById("tableContent").classList.remove("d-none");
});