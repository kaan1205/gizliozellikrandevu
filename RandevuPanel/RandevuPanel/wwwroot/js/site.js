// Sidebar toggle
document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('sidebarToggle');
    const wrapper = document.getElementById('wrapper');

    if (toggleBtn && wrapper) {
        toggleBtn.addEventListener('click', function () {
            if (window.innerWidth <= 768) {
                wrapper.classList.toggle('sidebar-mobile-open');
            } else {
                wrapper.classList.toggle('sidebar-collapsed');
            }
        });

        // Mobilde sidebar dışına tıklanınca kapat
        document.addEventListener('click', function (e) {
            if (window.innerWidth <= 768
                && wrapper.classList.contains('sidebar-mobile-open')
                && !e.target.closest('#sidebar-wrapper')
                && !e.target.closest('#sidebarToggle')) {
                wrapper.classList.remove('sidebar-mobile-open');
            }
        });
    }
});
