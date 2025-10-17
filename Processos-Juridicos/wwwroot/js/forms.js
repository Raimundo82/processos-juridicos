function markFileForDeletion(fileId) {
  // Remove the table row so it's no longer visible
  let row = document.getElementById('file-row-' + fileId).remove();

  // Create a new hidden input to mark this file for deletion
  let container = document.getElementById('deletedFilesContainer');
  let input = document.createElement('input');
  input.type = 'hidden';
  input.name = 'FilesToRemove'; // This should match the DTO property name
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
});

