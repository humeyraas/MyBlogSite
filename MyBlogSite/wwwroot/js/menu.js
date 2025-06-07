function toggleMenu() {
    const menu = document.getElementById('categoryMenu');
    menu.classList.toggle('active');
}

function filterCategory(category) {
    const posts = document.querySelectorAll('.post');
    posts.forEach(post => {
        const postCategory = post.getAttribute('data-category');
        if (category === 'Hepsi' || postCategory === category) {
            post.classList.remove('hidden');
        } else {
            post.classList.add('hidden');
        }
    });
}
