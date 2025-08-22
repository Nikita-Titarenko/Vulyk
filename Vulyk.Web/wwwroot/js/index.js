const commentsWrapper = document.getElementById('comments-wrapper');

const previousComment = document.getElementById('previous-comment').addEventListener('click', () => {
    const style = getComputedStyle(document.querySelector('.comment'));
    scroll(-(parseFloat(style.width) + parseFloat(style.marginLeft) + parseFloat(style.marginRight)));
});

const nextComment = document.getElementById('next-comment').addEventListener('click', () => {
    const style = getComputedStyle(document.querySelector('.comment'));
    scroll(parseFloat(style.width) + parseFloat(style.marginLeft) + parseFloat(style.marginRight));
});

function scroll(scroll) {
    commentsWrapper.scrollBy({ left: scroll, behavior: 'smooth' });
}