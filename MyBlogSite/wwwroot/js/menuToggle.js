// menuToggle.js
document.addEventListener("DOMContentLoaded", () => {
   
    const icon = document.getElementById("hamburgerIcon");
    const dropdown = document.getElementById("hamburgerDropdown");

    icon.addEventListener("click", () => {
        dropdown.classList.toggle("active");
    });

    // Dışarı tıklanınca kapanması için
    document.addEventListener("click", (e) => {
        if (!icon.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.remove("active");
        }
    });
});
