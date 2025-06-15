function toggleLike(blogId) {
    fetch('/Blog/ToggleLike', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
            // Eğer token gerekiyorsa buraya ekle
        },
        body: JSON.stringify({ blogId } )
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Ağ hatası: ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            const likeIcon = document.getElementById("heart-icon");
            const likeCount = document.getElementById("like-count");

            if (likeIcon && likeCount) {
                if (data.liked) {
                    likeIcon.classList.remove("fa-regular");
                    likeIcon.classList.add("fa-solid");
                } else {
                    likeIcon.classList.remove("fa-solid");
                    likeIcon.classList.add("fa-regular");
                }

                likeCount.textContent = data.count + " Beğeni";
            }
        })
        .catch(err => {
            console.error("Like işlemi sırasında hata oluştu:", err);
        });
}
