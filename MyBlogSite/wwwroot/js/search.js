document.addEventListener('DOMContentLoaded', () => {
    const searchIcon = document.getElementById('searchIcon');
    const searchInput = document.getElementById('searchInput');
    const searchForm = document.getElementById('searchForm');

    if (!searchIcon || !searchInput || !searchForm) {
        console.error("Arama bileþenlerinden biri bulunamadý.");
        return;
    }

    searchIcon.addEventListener('click', (event) => {
        event.preventDefault();

        if (searchInput.style.display === 'none' || searchInput.style.display === '') {
            searchInput.style.display = 'inline-block';
            searchInput.focus();
        } else {
            if (searchInput.value.trim() !== '') {
                searchForm.submit();
            }
        }
    });

    searchInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            if (searchInput.value.trim() !== '') {
                searchForm.submit();
            }
        }
    });
});
