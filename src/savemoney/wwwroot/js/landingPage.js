document.addEventListener("DOMContentLoaded", () => {
    
    // --- Módulo: Lógica do Modal de Boas-Vindas ---
    const welcomeModalEl = document.getElementById("welcomeModal");

    if (welcomeModalEl) {
        const welcomeModal = new bootstrap.Modal(welcomeModalEl, {
            keyboard: false,
        });

        // 'window.isUserAuthenticated' é a "ponte"
        if (window.isUserAuthenticated === true) {
            // Se logado, sempre mostra
            welcomeModal.show();
        } else {
            // Se deslogado, mostra uma vez por sessão
            const modalVisto = sessionStorage.getItem("welcomeModalVisto");
            if (!modalVisto) {
                welcomeModal.show();
                sessionStorage.setItem("welcomeModalVisto", "true");
            }
        }
    }

    // --- Módulo: Animação de Scroll (Observer) ---
    const animatedElements = document.querySelectorAll(".animate-on-scroll");

    if (animatedElements.length > 0) {
        // Cria o "observador"
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                // Se o elemento ESTÁ visível
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                } 
                // Se o elemento NÃO ESTÁ visível (para repetir a animação)
                else {
                    entry.target.classList.remove("is-visible");
                }
            });
        }, {
            threshold: 0.1 // Ativa quando 10% do elemento estiver visível
        });

        // "Observa" cada elemento que tem a classe
        animatedElements.forEach(el => {
            observer.observe(el);
        });
    }

});