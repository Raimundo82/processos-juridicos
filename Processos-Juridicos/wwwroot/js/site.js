// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Modal to delete
document.addEventListener("DOMContentLoaded", function () {
    const deleteModalEl = document.getElementById("deleteModal");
    const deleteModal = new bootstrap.Modal(deleteModalEl);
    const entitySpan = document.getElementById("deleteEntity");
    const nameSpan = document.getElementById("deleteName");
    const idInput = document.getElementById("deleteId");
    const deleteForm = document.getElementById("deleteForm");

    document.addEventListener("click", function (e) {
        const button = e.target.closest(".btn-delete");
        if (!button) return;

        const entity = button.dataset.entity;
        const name = button.dataset.name?.trim();
        const id = button.dataset.id;
        const controller = button.dataset.controller;
        const action = button.dataset.action;

        idInput.value = id;
        deleteForm.action = `/${controller}/${action}`;
        entitySpan.textContent = entity;
        nameSpan.textContent = name ? " " + name : "";

        deleteModal.show();
    });
});

// Modal to confirm reject
document.addEventListener("DOMContentLoaded", function () {
    const rejectModalEl = document.getElementById("rejectModal");
    if (!rejectModalEl) {
        return;
    }
    const rejectModal = new bootstrap.Modal(rejectModalEl);
    const btnRejectChanges = document.querySelector(".btn-reject");
    const btnConfirmReject = document.getElementById("btnConfirmReject");


    btnRejectChanges.addEventListener("click", function () {
    rejectModal.show();

     btnConfirmReject.addEventListener("click", function () {
     globalThis.history.back();
        });
    })

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
    const loader = document.getElementById("tableLoader");
    const content = document.getElementById("tableContent");

    if (loader) loader.classList.add("d-none");
    if (content) content.classList.remove("d-none");
});
