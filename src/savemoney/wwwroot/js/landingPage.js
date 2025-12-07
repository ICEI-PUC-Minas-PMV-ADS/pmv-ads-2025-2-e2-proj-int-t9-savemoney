document.addEventListener("DOMContentLoaded", () => {

    // Verificar preferência de animações do usuário
    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    /* ==========================================
       Modal de Boas-Vindas
       ========================================== */
    const welcomeModalEl = document.getElementById("welcomeModal");

    if (welcomeModalEl) {
        try {
            // Verificar se Bootstrap está disponível
            if (typeof bootstrap === "undefined") {
                console.warn("Bootstrap não carregado. Modal não será exibido.");
                return;
            }

            const welcomeModal = new bootstrap.Modal(welcomeModalEl, {
                keyboard: true,
                backdrop: true
            });

            const isAuthenticated = window.isUserAuthenticated === true;

            if (isAuthenticated) {
                // Usuário logado: sempre mostrar
                welcomeModal.show();
            } else {
                // Usuário deslogado: mostrar uma vez por sessão
                const hasSeenModal = sessionStorage.getItem("welcomeModalVisto");

                if (!hasSeenModal) {
                    welcomeModal.show();
                    sessionStorage.setItem("welcomeModalVisto", "true");
                }
            }

        } catch (error) {
            console.error("Erro ao inicializar modal:", error);
        }
    }

    /* ==========================================
       Animações no Scroll (Intersection Observer)
       ========================================== */
    const animatedElements = document.querySelectorAll(".animate-on-scroll");

    if (animatedElements.length > 0) {

        // Respeitar preferência de animações reduzidas
        if (prefersReducedMotion) {
            animatedElements.forEach(el => el.classList.add("is-visible"));
            return;
        }

        // Verificar suporte ao IntersectionObserver
        if (!("IntersectionObserver" in window)) {
            // Fallback: mostrar todos os elementos imediatamente
            animatedElements.forEach(el => el.classList.add("is-visible"));
            return;
        }

        // Configurações do Observer
        const observerOptions = {
            threshold: 0.15, // 15% visível para animar
            rootMargin: "0px 0px -80px 0px" // Anima antes de entrar totalmente
        };

        // Criar observer
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    // Opcional: parar de observar para melhor performance
                    // observer.unobserve(entry.target);
                } else {
                    // Remove classe para permitir re-animação ao voltar
                    entry.target.classList.remove("is-visible");
                }
            });
        }, observerOptions);

        // Observar cada elemento
        animatedElements.forEach(el => observer.observe(el));

        // Cleanup ao sair da página
        window.addEventListener("beforeunload", () => {
            observer.disconnect();
        });
    }

    /* ==========================================
       Smooth Scroll para Links Âncora
       ========================================== */
    const anchorLinks = document.querySelectorAll('a[href^="#"]');

    anchorLinks.forEach(link => {
        link.addEventListener("click", (e) => {
            const targetId = link.getAttribute("href");

            // Ignorar links vazios ou modais
            if (!targetId || targetId === "#" || targetId === "#!") {
                return;
            }

            const targetElement = document.querySelector(targetId);

            if (targetElement) {
                e.preventDefault();

                // Calcular offset do header fixo
                const header = document.getElementById("main-header");
                const headerHeight = header ? header.offsetHeight : 0;
                const targetPosition = targetElement.getBoundingClientRect().top + window.pageYOffset - headerHeight - 20;

                window.scrollTo({
                    top: targetPosition,
                    behavior: prefersReducedMotion ? "auto" : "smooth"
                });

                // Atualizar URL
                if (history.pushState) {
                    history.pushState(null, null, targetId);
                }

                // Focar no elemento para acessibilidade
                targetElement.setAttribute("tabindex", "-1");
                targetElement.focus();
            }
        });
    });

    /* ==========================================
       Otimização: Adicionar classe ao body no scroll
       ========================================== */
    let lastScrollY = window.scrollY;
    let ticking = false;

    const updateScrollDirection = () => {
        const currentScrollY = window.scrollY;

        if (currentScrollY > lastScrollY && currentScrollY > 150) {
            document.body.classList.add("scrolling-down");
            document.body.classList.remove("scrolling-up");
        } else if (currentScrollY < lastScrollY) {
            document.body.classList.add("scrolling-up");
            document.body.classList.remove("scrolling-down");
        }

        // Adicionar classe quando passou do hero
        if (currentScrollY > 100) {
            document.body.classList.add("scrolled");
        } else {
            document.body.classList.remove("scrolled");
        }

        lastScrollY = currentScrollY;
        ticking = false;
    };

    window.addEventListener("scroll", () => {
        if (!ticking) {
            window.requestAnimationFrame(updateScrollDirection);
            ticking = true;
        }
    }, { passive: true });

});