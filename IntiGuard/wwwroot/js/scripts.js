window.addEventListener('DOMContentLoaded', () => {

    // ==============================
    // NAVBAR
    // ==============================

    // Achicar navbar al hacer scroll
    const navbarShrink = () => {
        const navbarCollapsible = document.querySelector('#mainNav');
        if (!navbarCollapsible) return;
        if (window.scrollY === 0) {
            navbarCollapsible.classList.remove('navbar-shrink');
        } else {
            navbarCollapsible.classList.add('navbar-shrink');
        }
    };
    navbarShrink();
    document.addEventListener('scroll', navbarShrink);

    // Activar ScrollSpy de Bootstrap
    const mainNav = document.querySelector('#mainNav');
    if (mainNav) {
        new bootstrap.ScrollSpy(document.body, {
            target: '#mainNav',
            offset: 80
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

    // ==============================
    // CARRITO (interacciones front)
    // ==============================

    // Actualizar contador carrito dinámicamente
    const updateCartCount = (count) => {
        const cartCount = document.getElementById("cartCount");
        if (cartCount) {
            cartCount.textContent = count;
            cartCount.style.display = count > 0 ? "inline-block" : "none";
        }
    };

    // Escuchar botones "Agregar al carrito"
    document.querySelectorAll(".btn-add-to-cart").forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            const productId = btn.dataset.id;
            const cantidad = 1; // se puede extender con input cantidad

            fetch(`/Venta/AgregarAlCarrito?idProducto=${productId}&cantidad=${cantidad}`, {
                method: "POST"
            })
                .then(response => {
                    if (!response.ok) throw new Error("Error al agregar al carrito");
                    return response.text();
                })
                .then(() => {
                    // Incrementar contador (solo visual)
                    const current = parseInt(document.getElementById("cartCount").textContent || 0);
                    updateCartCount(current + 1);

                    // Feedback al usuario
                    const toast = document.createElement("div");
                    toast.className = "toast align-items-center text-bg-success border-0 show position-fixed bottom-0 end-0 m-3";
                    toast.innerHTML = `
                        <div class="d-flex">
                            <div class="toast-body">Producto agregado al carrito ✅</div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                        </div>`;
                    document.body.appendChild(toast);
                    setTimeout(() => toast.remove(), 2500);
                })
                .catch(err => console.error(err));
        });
    });

    // ==============================
    // EFECTOS EXTRAS
    // ==============================

    // Animación suave al hacer clic en links internos (scroll)
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener("click", function (e) {
            const targetId = this.getAttribute("href");
            if (targetId.length > 1) {
                e.preventDefault();
                document.querySelector(targetId).scrollIntoView({
                    behavior: "smooth"
                });
            }
        });
    });
});