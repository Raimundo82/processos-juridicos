const toggle = document.getElementById('nightModeToggle');

toggle?.addEventListener('click', function () {
    document.body.classList.toggle('dark-mode');

    const theme = document.body.classList.contains('dark-mode') ? 'dark' : 'light';

    document.cookie = "theme=" + theme + "; path=/; max-age=" + 60 * 60 * 24 * 30;
});
