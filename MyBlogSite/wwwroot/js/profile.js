function showTab(tabId) {
    const tabs = document.querySelectorAll('.tab-content');
    const buttons = document.querySelectorAll('.tab-buttons button');

    tabs.forEach(tab => tab.classList.remove('active'));
    buttons.forEach(btn => btn.classList.remove('active'));

    document.getElementById(tabId).classList.add('active');
    if (el) el.classList.add('active');


}
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.like-button').forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            const blogId = button.getAttribute('data-blog-id');

            const response = await fetch('/Blog/ToggleLike', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ blogId: parseInt(blogId) })
            });

            if (response.ok) {
                // Opsiyonel: Kalbi değiştir
                button.classList.remove('liked');
                // Veya sayfayı yeniden yükle
                location.reload(); // Kaldırmak istersen sekmeden çıkartırsın
            } else {
                alert("İşlem başarısız.");
            }
        });
    });
});
