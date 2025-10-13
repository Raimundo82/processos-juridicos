// DataTable Initializer

export { createDataTable, loadProcessFilters, fillSelect };

function createDataTable(elementId) {
    const table = $(elementId);
    if (table.length && table.find('tbody tr').length > 0) {

        const options = {
            "language": {
                "sEmptyTable": "Nenhum registo disponível",
                "sInfo": "A mostrar _START_ a _END_ de um total de _TOTAL_ registos",
                "sInfoEmpty": "A mostrar 0 a 0 de um total de 0 registos",
                "sInfoFiltered": "(filtrado de um total de _MAX_ registos)",
                "sLengthMenu": "Mostrar _MENU_ registos",
                "sLoadingRecords": "A carregar...",
                "sProcessing": "A processar...",
                "sZeroRecords": "Não foram encontrados resultados",
                "sSearch": "Procurar:",
                "oPaginate": {
                    "sFirst": "Primeiro",
                    "sPrevious": "Anterior",
                    "sNext": "Seguinte",
                    "sLast": "Último"
                },
                "oAria": {
                    "sSortAscending": ": Ordenar colunas de forma ascendente",
                    "sSortDescending": ": Ordenar colunas de forma descendente"
                }
            },
            "infoCallback": function(settings, start, end, max, total, pre) {
                return pre.replace(/(\d+)/g, '<strong>$1</strong>');
            }
        };

        if (elementId !== '#tableStates') {
            options.columnDefs = [{ orderable: false, targets: -1 }];
        }

        table.DataTable(options);
    }
}

// Filters to Process Table 
function loadProcessFilters(apiUrl, unitSelector, typeSelector, stateSelector) {
        $.getJSON(apiUrl, function (data) {
            fillSelect(unitSelector, data.units);
            fillSelect(typeSelector, data.types);
            fillSelect(stateSelector, data.states);
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

    loadProcessFilters('/Process/GetFilterValues', '#unitFilter', '#typeFilter', '#stateFilter');

    $('#unitFilter').on('change', function () {
        table.column(2).search(this.value).draw();
    });
    $('#typeFilter').on('change', function () {
        table.column(1).search(this.value).draw();
    });
    $('#stateFilter').on('change', function () {
        table.column(8).search(this.value).draw();
    });
});

// Select2 Input Filters to Process Table
$('#unitFilter').select2({
    placeholder: "Todas as unidades",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});
$('#typeFilter').select2({
    placeholder: "Todas os tipos",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});
$('#stateFilter').select2({
    placeholder: "Todas os estados",
    allowClear: true,
    theme: 'bootstrap-5',
    width: 'resolve'
});
