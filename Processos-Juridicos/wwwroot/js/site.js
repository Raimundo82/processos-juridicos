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

    document.querySelectorAll(".btn-delete").forEach(button => {
        button.addEventListener("click", function () {
            const entity = this.dataset.entity;
            const name = this.dataset.name?.trim();
            const id = this.dataset.id;
            const controller = this.dataset.controller;
            const action = this.dataset.action;

            idInput.value = id;
            deleteForm.action = `/${controller}/${action}`;

            entitySpan.textContent = entity;

            if (name) {
                nameSpan.textContent = " " + name;
            } else {
                nameSpan.textContent = "";
            }

            deleteModal.show();
        });
    });
});

// Modal to confirm reject
document.addEventListener("DOMContentLoaded", function () {
    const rejectModalEl = document.getElementById("rejectModal");
    const rejectModal = new bootstrap.Modal(rejectModalEl);
    const btnRejectChanges = document.querySelector(".btn-reject");
    const btnConfirmReject = document.getElementById("btnConfirmReject");

    btnRejectChanges.addEventListener("click", function () {
        rejectModal.show();
    });

    btnConfirmReject.addEventListener("click", function () {
        window.history.back();
    });
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
