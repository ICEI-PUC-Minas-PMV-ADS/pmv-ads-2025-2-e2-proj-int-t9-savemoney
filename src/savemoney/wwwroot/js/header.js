document.addEventListener("DOMContentLoaded", () => {

    const header = document.getElementById("main-header");
    const tabButton = document.getElementById("header-toggle-tab");

    if (header && tabButton) {
        const tabIcon = tabButton.querySelector("i");
        const storageKey = "savemoney_header_hidden"; // Chave única para evitar conflito

        function updateHeaderState() {
            if (header.classList.contains("header-hidden")) {
                // ESTADO: ESCONDIDO -> Seta para baixo
                if (tabIcon) {
                    tabIcon.className = "bi bi-chevron-down";
                }
                tabButton.title = "Mostrar Menu";
            } else {
                // ESTADO: VISÍVEL -> Seta para cima
                if (tabIcon) {
                    tabIcon.className = "bi bi-chevron-up";
                }
                tabButton.title = "Esconder Menu";
            }
        }

        // 1. Verificar estado salvo
        if (localStorage.getItem(storageKey) === "true") {
            header.classList.add("header-hidden");
        }
        updateHeaderState();

        // 2. Click event
        tabButton.addEventListener("click", () => {
            header.classList.toggle("header-hidden");

            // 3. Salvar preferência
            const isHidden = header.classList.contains("header-hidden");
            localStorage.setItem(storageKey, isHidden);

            // 4. Atualizar ícone
            updateHeaderState();
        });
    }
});