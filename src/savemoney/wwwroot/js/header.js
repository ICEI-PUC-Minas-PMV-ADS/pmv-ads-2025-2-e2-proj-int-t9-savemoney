document.addEventListener("DOMContentLoaded", () => {
    
    // --- Módulo: Lógica do "Pull Tab" do Header ---
    const header = document.getElementById("main-header");
    const tabButton = document.getElementById("header-toggle-tab"); 

    if (header && tabButton) {
        const tabIcon = tabButton.querySelector("i"); 
        const storageKey = "headerHidden"; 

        function updateHeaderState() {
            if (header.classList.contains("header-hidden")) {
                // ESTADO: ESCONDIDO
                if (tabIcon) {
                    tabIcon.classList.remove("bi-chevron-up");
                    tabIcon.classList.add("bi-chevron-down");
                }
                tabButton.title = "Mostrar Menu";
            } else {
                // ESTADO: VISÍVEL
                if (tabIcon) {
                    tabIcon.classList.remove("bi-chevron-down");
                    tabIcon.classList.add("bi-chevron-up");
                }
                tabButton.title = "Esconder Menu";
            }
        }

        // 1. Verificar estado salvo no localStorage
        if (localStorage.getItem(storageKey) === "true") {
            header.classList.add("header-hidden");
        }
        updateHeaderState(); 

        // 2. Adicionar o clique no botão "pull tab"
        tabButton.addEventListener("click", () => {
            header.classList.toggle("header-hidden");
            
            // 3. Salvar a preferência
            const isHidden = header.classList.contains("header-hidden");
            localStorage.setItem(storageKey, isHidden);
            
            // 4. Atualizar o ícone
            updateHeaderState();
        });
    }
});