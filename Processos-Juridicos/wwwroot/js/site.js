// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// datatable
function createDataTable(elmentId) {
  let table = $(elmentId);
  if (table.length && table.find('tbody tr').length > 0) {
    $(elmentId).DataTable({
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
          sLast: 'Último',
        },
        oAria: {
          sSortAscending: ': Ordenar colunas de forma ascendente',
          sSortDescending: ': Ordenar colunas de forma descendente',
        },
      },
      /*scrollCollapse: true*/
      /*scrollY: '300px',*/
      pageLength: 25,
    });
  }
}
