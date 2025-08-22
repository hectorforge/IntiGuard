/*!
* Start Bootstrap - Agency (adaptado para IntiGuard)
* Licensed under MIT
*/

window.addEventListener('DOMContentLoaded', () => {

    // Función para achicar navbar al hacer scroll
    const navbarShrink = () => {
        const navbarCollapsible = document.querySelector('#mainNav');
        if (!navbarCollapsible) return;

        if (window.scrollY === 0) {
            navbarCollapsible.classList.remove('navbar-shrink');
        } else {
            navbarCollapsible.classList.add('navbar-shrink');
        }
    };

    // Ejecutar al cargar
    navbarShrink();

    // Ejecutar en scroll
    document.addEventListener('scroll', navbarShrink);

    // Activar ScrollSpy de Bootstrap
    const mainNav = document.querySelector('#mainNav');
    if (mainNav) {
        new bootstrap.ScrollSpy(document.body, {
            target: '#mainNav',
            offset: 80 // antes usabas rootMargin, ahora correcto en BS5
        });
    }

    // Cerrar menú responsive al hacer clic en un link
    const navbarToggler = document.querySelector('.navbar-toggler');
    const responsiveNavItems = document.querySelectorAll('#navbarNav .nav-link');
    responsiveNavItems.forEach((navItem) => {
        navItem.addEventListener('click', () => {
            if (navbarToggler && window.getComputedStyle(navbarToggler).display !== 'none') {
                navbarToggler.click();
            }
        });
    });

});
