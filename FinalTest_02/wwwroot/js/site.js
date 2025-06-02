// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const submitButton = document.getElementById("checkoutButton");
    const checkboxes = document.querySelectorAll('input[name="selectedDetailIds"]');

    function toggleSubmitButton() {
        const anyChecked = Array.from(checkboxes).some(cb => cb.checked);
        submitButton.disabled = !anyChecked;
    }

    checkboxes.forEach(cb => cb.addEventListener("change", toggleSubmitButton));

    toggleSubmitButton(); // 初始化檢查
});

