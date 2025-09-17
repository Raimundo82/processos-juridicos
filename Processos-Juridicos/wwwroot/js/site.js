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

// DataTable initializer
function createDataTable(elementId) {
    const table = $(elementId);
    if (table.length && table.find('tbody tr').length > 0) {
        table.DataTable({
            responsive: true,
            ordering: true,
            paging: true,
            searching: true,
            oLanguage: {
                sEmptyTable: 'Nenhum registo encontrado',
                sInfo: 'Mostrar _START_ até _END_ de _TOTAL_ registos',
                sInfoEmpty: 'Mostrar 0 até 0 de 0 Registos',
                sInfoFiltered: '(Filtrar de _MAX_ total registos)',
                sInfoPostFix: '',
                sInfoThousands: '.',
                sLengthMenu: 'Mostrar _MENU_ registos por pagina',
                sLoadingRecords: 'a carregar...',
                sProcessing: 'a processar...',
                sZeroRecords: 'Nenhum registos encontrado',
                sSearch: 'Pesquisar',
                oPaginate: {
                    sNext: 'Próximo',
                    sPrevious: 'Anterior',
                    sFirst: 'Primeiro',
                    sLast: 'Último'
                },
                oAria: {
                    sSortAscending: ': Ordenar colunas de forma ascendente',
                    sSortDescending: ': Ordenar colunas de forma descendente'
                }
            },
            pageLength: 25
        });
    }
}

// Select2 initializers
$(function () {
    $('#unit-id').select2({
        allowClear: true
    });
    $('#articles-infringed').select2({
        allowClear: true, closeOnSelect: false
    });
});
