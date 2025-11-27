document.addEventListener("DOMContentLoaded", () => {

    // Pull Tab - Desktop
    const header = document.getElementById("main-header");
    const tabButton = document.getElementById("header-toggle-tab");
    const storageKey = "savemoney_header_hidden";

    if (header && tabButton) {
        const tabIcon = tabButton.querySelector("i");

        function updateHeaderTabState() {
            const isHidden = header.classList.contains("header-hidden");
            if (tabIcon) {
                tabIcon.className = isHidden ? "bi bi-chevron-down" : "bi bi-chevron-up";
            }
            tabButton.title = isHidden ? "Mostrar Menu" : "Esconder Menu";
        }

        // Carregar estado salvo
        if (localStorage.getItem(storageKey) === "true") {
            header.classList.add("header-hidden");
        }
        updateHeaderTabState();

        // Toggle ao clicar
        tabButton.addEventListener("click", () => {
            header.classList.toggle("header-hidden");
            const isHidden = header.classList.contains("header-hidden");
            localStorage.setItem(storageKey, isHidden);
            updateHeaderTabState();
        });
    }

    // Menu Mobile
    const mobileToggle = document.getElementById("mobile-menu-toggle");
    const mobileDrawer = document.getElementById("mobile-nav-drawer");
    const body = document.body;

    if (mobileToggle && mobileDrawer) {
        function toggleMobileMenu() {
            const isOpen = mobileDrawer.classList.toggle("is-open");
            mobileToggle.querySelector("i").className = isOpen ? "bi bi-x-lg" : "bi bi-list";
            mobileToggle.setAttribute("aria-expanded", isOpen);
            body.classList.toggle("no-scroll", isOpen);
        }

        mobileToggle.addEventListener("click", toggleMobileMenu);

        // Fechar ao clicar em links
        mobileDrawer.querySelectorAll("[data-dismiss='mobile-nav']").forEach(link => {
            link.addEventListener("click", () => {
                setTimeout(toggleMobileMenu, 100);
            });
        });

        // Dropdowns Mobile
        const mobileDropdownToggles = mobileDrawer.querySelectorAll(".mobile-dropdown-toggle");
        mobileDropdownToggles.forEach(toggle => {
            toggle.addEventListener("click", () => {
                const parentLi = toggle.closest(".mobile-dropdown");
                const isOpen = parentLi.classList.contains("is-open");

                // Fechar outros dropdowns
                mobileDropdownToggles.forEach(otherToggle => {
                    const otherParent = otherToggle.closest(".mobile-dropdown");
                    if (otherParent !== parentLi) {
                        otherParent.classList.remove("is-open");
                        otherToggle.setAttribute("aria-expanded", "false");
                    }
                });

                // Toggle do atual
                parentLi.classList.toggle("is-open");
                toggle.setAttribute("aria-expanded", !isOpen);
            });
        });
    }
});