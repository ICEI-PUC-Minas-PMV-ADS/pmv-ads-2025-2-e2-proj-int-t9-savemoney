document.addEventListener("DOMContentLoaded", () => {

    /* =========================================
     * 1. Lógica da Aba Pull Tab (Desktop)
     * ========================================= */
    const header = document.getElementById("main-header");
    const tabButton = document.getElementById("header-toggle-tab");
    const storageKey = "savemoney_header_hidden";

    if (header && tabButton) {
        const tabIcon = tabButton.querySelector("i");

        function updateHeaderTabState() {
            const isHidden = header.classList.contains("header-hidden");
            if (tabIcon) {
                // ESTADO: ESCONDIDO -> Seta para baixo, VISÍVEL -> Seta para cima
                tabIcon.className = isHidden ? "bi bi-chevron-down" : "bi bi-chevron-up";
            }
            tabButton.title = isHidden ? "Mostrar Menu" : "Esconder Menu";
        }

        // Carregar estado salvo
        if (localStorage.getItem(storageKey) === "true") {
            header.classList.add("header-hidden");
        }
        updateHeaderTabState();

        // Click event para a aba
        tabButton.addEventListener("click", () => {
            header.classList.toggle("header-hidden");
            // Salvar preferência
            const isHidden = header.classList.contains("header-hidden");
            localStorage.setItem(storageKey, isHidden);
            // Atualizar ícone
            updateHeaderTabState();
        });
    }

    /* =========================================
     * 2. Lógica do Menu Hamburguer (Mobile)
     * ========================================= */
    const mobileToggle = document.getElementById("mobile-menu-toggle");
    const mobileDrawer = document.getElementById("mobile-nav-drawer");
    const body = document.body;

    if (mobileToggle && mobileDrawer) {
        // Função para abrir/fechar
        function toggleMobileMenu() {
            const isOpen = mobileDrawer.classList.toggle("is-open");
            mobileToggle.querySelector("i").className = isOpen ? "bi bi-x-lg" : "bi bi-list";
            // Adicionar/remover classe para prevenir scroll no body (melhor UX)
            body.classList.toggle("no-scroll", isOpen);
        }

        // Click no botão de toggle
        mobileToggle.addEventListener("click", toggleMobileMenu);

        // Fechar ao clicar em um link (se houver o atributo data-dismiss)
        mobileDrawer.querySelectorAll("[data-dismiss='mobile-nav']").forEach(link => {
            link.addEventListener("click", () => {
                // Pequeno delay para a navegação começar
                setTimeout(toggleMobileMenu, 100);
            });
        });

        /* ---------------------------------
         * 3. Lógica dos Dropdowns Mobile
         * --------------------------------- */
        const mobileDropdownToggles = mobileDrawer.querySelectorAll(".mobile-dropdown-toggle");

        mobileDropdownToggles.forEach(toggle => {
            toggle.addEventListener("click", () => {
                const parentLi = toggle.closest(".mobile-dropdown");

                // Fechar todos os outros dropdowns
                mobileDropdownToggles.forEach(otherToggle => {
                    const otherParent = otherToggle.closest(".mobile-dropdown");
                    if (otherParent !== parentLi) {
                        otherParent.classList.remove("is-open");
                    }
                });

                // Abrir/Fechar o dropdown clicado
                parentLi.classList.toggle("is-open");
            });
        });
    }
});