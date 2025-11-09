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

    // --- [NOVO] Módulo: Animação de Scroll (Observer) ---
    // Este é o JS que procura as classes `.animate-on-scroll`
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