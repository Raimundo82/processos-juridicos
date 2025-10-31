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
        // disable and uncheck the checkbox
        $("#ComunicatedToPjm").prop("checked", false).prop("disabled", true);

        // disable and clear the date input
        $("#ComunicationDate")
            .val("")              // clear the value
            .prop("disabled", true);
    } else {
        // enable both fields
        $("#ComunicatedToPjm").prop("disabled", false);
        $("#ComunicationDate").prop("disabled", false);
    }
}


$(document).ready(function () {
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
