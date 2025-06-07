function toggleLike(blogId) {
    fetch('/Blog/ToggleLike', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // varsa
        },
        body: JSON.stringify(blogId)
    })
        .then(response => response.json())
        .then(data => {
            const likeIcon = document.getElementById("heart-icon");
            const likeCount = document.getElementById("like-count");

            if (data.liked) {
                likeIcon.classList.remove("fa-regular");
                likeIcon.classList.add("fa-solid");
            } else {
                likeIcon.classList.remove("fa-solid");
                likeIcon.classList.add("fa-regular");
            }

            likeCount.textContent = data.count + " Beğeni";
        });
}