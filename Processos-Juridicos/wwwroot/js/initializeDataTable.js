export { createDataTable, loadProcessFilters, fillSelect };

// DataTable Initializer
function createDataTable(elementId, extraOptions = {}) {
    const table = $(elementId);
    if (table.length) {

        const baseOptions = {
            language: {
                sEmptyTable: "Nenhum registo disponível",
                sInfo: "A mostrar _START_ a _END_ de um total de _TOTAL_ registos",
                sInfoEmpty: "A mostrar 0 a 0 de um total de 0 registos",
                sInfoFiltered: "(filtrado de um total de _MAX_ registos)",
                sLengthMenu: "Mostrar _MENU_ registos",
                sLoadingRecords: "A carregar...",
                sProcessing: "A processar...",
                sZeroRecords: "Não foram encontrados resultados",
                sSearch: "Procurar:",
                oPaginate: {
                    sFirst: "Primeiro",
                    sPrevious: "Anterior",
                    sNext: "Seguinte",
                    sLast: "Último"
                },
                oAria: {
                    sSortAscending: ": Ordenar colunas de forma ascendente",
                    sSortDescending: ": Ordenar colunas de forma descendente"
                }
            },
            "infoCallback": function (settings, start, end, max, total, pre) {
                return pre.replaceAll(/(\d+)/g, '<strong>$1</strong>');
            }
        };

        if (elementId !== '#tableStates') {
            baseOptions.columnDefs = [{ orderable: false, targets: -1 }];
        }

        const options = $.extend(true, {}, baseOptions, extraOptions);

        return table.DataTable(options);
    }
    return null;
}

function loadProcessFilters(apiUrl, unitSelector, typeSelector, stateSelector, yearSelector) {
    return $.getJSON(apiUrl).then(function (data) {
        fillSelect(unitSelector, data.units);
        fillSelect(typeSelector, data.types);
        fillSelect(stateSelector, data.states);
        fillSelect(yearSelector, data.years);
    });
}

function fillSelect(selector, values) {
    const $select = $(selector);
    $select.empty().append('<option value="">Todos</option>');
    (values || []).forEach(function (v) {
        if (v) {
            $select.append('<option value="' + v + '">' + v + '</option>');
        }
    });
}

$(document).ready(function () {
    const table = $('#processesTable').DataTable();
    let restoringFilters = false;

    function saveFilters() {
        if (restoringFilters) return;
        const filters = {
            unit: $('#unitFilter').val() ?? '',
            type: $('#typeFilter').val() ?? '',
            state: $('#stateFilter').val() ?? '',
            year: $('#yearFilter').val() ?? ''
        };
        localStorage.setItem('processFilters', JSON.stringify(filters));
    }

    function restoreFilters() {
        const saved = localStorage.getItem('processFilters');
        if (!saved) return;

        const filters = JSON.parse(saved);
        restoringFilters = true;

        $('#unitFilter').val(filters.unit).trigger('change.select2');
        table.column(2).search(filters.unit || '').draw();

        $('#typeFilter').val(filters.type).trigger('change.select2');
        table.column(1).search(filters.type || '').draw();

        $('#stateFilter').val(filters.state).trigger('change.select2');
        table.column(8).search(filters.state || '').draw();

        $('#yearFilter').val(filters.year).trigger('change.select2');
        table.column(5).search(filters.year || '').draw();

        restoringFilters = false;
    }

    loadProcessFilters('/Process/GetFilterValues', '#unitFilter', '#typeFilter', '#stateFilter', '#yearFilter')
        .then(() => restoreFilters());

    $('#unitFilter').on('change', function () {
        table.column(2).search(this.value || '').draw();
        saveFilters();
    });
    $('#typeFilter').on('change', function () {
        table.column(1).search(this.value || '').draw();
        saveFilters();
    });
    $('#stateFilter').on('change', function () {
        table.column(8).search(this.value || '').draw();
        saveFilters();
    });
    $('#yearFilter').on('change', function () {
        table.column(5).search(this.value || '').draw();
        saveFilters();
    });
});

$('#unitFilter').select2({
    placeholder: "Todas as Unidades",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});
$('#typeFilter').select2({
    placeholder: "Todos os Tipos",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});
$('#stateFilter').select2({
    placeholder: "Todas os Estados",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});
$('#yearFilter').select2({
    placeholder: "Todos os Anos",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});