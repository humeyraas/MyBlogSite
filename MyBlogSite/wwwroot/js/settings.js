document.addEventListener("DOMContentLoaded", function () {
    const profileInput = document.getElementById("profileImageInput");
    const preview = document.getElementById("preview");

    profileInput.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                preview.setAttribute("src", e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
});
