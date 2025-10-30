export { markFileForDeletion, toggleReportToPjm };

function markFileForDeletion(fileId) {
    document.getElementById(`file-row-${fileId}`)?.remove();

    let container = document.getElementById('deletedFilesContainer');
    if (!container) {
        console.error('Container deletedFilesContainer não encontrado.');
        return;
    }

    let input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'FilesToRemove';
    input.value = fileId;
    container.appendChild(input);
}

function toggleReportToPjm() {
    let typeText = $("#ProcessTypeId option:selected").text();
    if (typeText === "Acidentes em serviço") {
        $("#ComunicatedToPjm").prop("checked", false).prop("disabled", true);
    } else {
        $("#ComunicatedToPjm").prop("disabled", false);
    }
}

$(function () {
    toggleReportToPjm();
    $("#ProcessTypeId").on("change", toggleReportToPjm);

    $(document).on("click", ".js-mark-for-deletion", function () {
        let fileId = $(this).data('file-id');

        if (fileId) {
            markFileForDeletion(fileId);
        } else {
            console.error('ERRO: ID do arquivo inválido.');
        }
    });
});
