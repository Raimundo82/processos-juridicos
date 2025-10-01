function markFileForDeletion(fileId) {
  // Remove the table row so it's no longer visible
  let row = document.getElementById('file-row-' + fileId);
  if (row) {
    row.parentNode.removeChild(row);
  }

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
});

