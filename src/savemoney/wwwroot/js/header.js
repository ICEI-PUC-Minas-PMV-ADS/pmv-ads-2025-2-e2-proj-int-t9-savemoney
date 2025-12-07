/**
 * header.js - Gerenciador do Header
 */

// =================================================================
// TOGGLE PF/PJ/AMBOS
// =================================================================

function togglePerfil(tipo) {
    console.log('Alterando para:', tipo);

    atualizarBotoesVisuais(tipo);
    mostrarToastContexto(tipo, 'loading');

    fetch('/Contexto/DefinirContexto', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include', // ← ISSO AQUI FAZ ENVIAR O COOKIE
        body: JSON.stringify(tipo)
    })
        .then(response => {
            if (!response.ok) throw new Error('Erro HTTP: ' + response.status);
            return response.json();
        })
        .then(data => {
            console.log('Salvo:', data);
            mostrarToastContexto(tipo, 'success');
            setTimeout(() => window.location.reload(), 500);
        })
        .catch(error => {
            console.error('Erro:', error);
            mostrarToastContexto(tipo, 'error');
        });
}

function atualizarBotoesVisuais(tipo) {
    const botoes = document.querySelectorAll('.profile-toggle .toggle-btn');
    botoes.forEach(btn => {
        btn.classList.remove('active');
        if (btn.dataset.type === tipo) {
            btn.classList.add('active');
        }
    });
}

// =================================================================
// DROPDOWN DO USUÁRIO
// =================================================================

function toggleDropdown() {
    const dropdown = document.getElementById('userDropdown');
    if (dropdown) {
        dropdown.classList.toggle('active');
    }
}

window.addEventListener('click', function (event) {
    if (!event.target.closest('.dropdown-menu-container')) {
        const dropdown = document.getElementById('userDropdown');
        if (dropdown && dropdown.classList.contains('active')) {
            dropdown.classList.remove('active');
        }
    }
});

// =================================================================
// INICIALIZAÇÃO
// =================================================================

document.addEventListener("DOMContentLoaded", () => {
    console.log('header.js carregado');

    // Sincroniza toggle com contexto do servidor
    const contextoServidor = document.body.dataset.userContext || 'pf';
    console.log('Contexto do servidor:', contextoServidor);
    atualizarBotoesVisuais(contextoServidor);

    // Pull Tab
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
        }

        if (localStorage.getItem(storageKey) === "true") {
            header.classList.add("header-hidden");
        }
        updateHeaderTabState();

        tabButton.addEventListener("click", () => {
            header.classList.toggle("header-hidden");
            localStorage.setItem(storageKey, header.classList.contains("header-hidden"));
            updateHeaderTabState();
        });
    }

    // Menu Mobile
    const mobileToggle = document.getElementById("mobile-menu-toggle");
    const mobileDrawer = document.getElementById("mobile-nav-drawer");

    if (mobileToggle && mobileDrawer) {
        mobileToggle.addEventListener("click", () => {
            const isOpen = mobileDrawer.classList.toggle("is-open");
            const icon = mobileToggle.querySelector("i");
            if (icon) {
                icon.className = isOpen ? "bi bi-x-lg" : "bi bi-list";
            }
            document.body.classList.toggle("no-scroll", isOpen);
        });

        mobileDrawer.querySelectorAll("[data-dismiss='mobile-nav']").forEach(link => {
            link.addEventListener("click", () => {
                mobileDrawer.classList.remove("is-open");
                document.body.classList.remove("no-scroll");
            });
        });

        // Dropdowns Mobile
        mobileDrawer.querySelectorAll(".mobile-dropdown-toggle").forEach(toggle => {
            toggle.addEventListener("click", () => {
                const parent = toggle.closest(".mobile-dropdown");
                mobileDrawer.querySelectorAll(".mobile-dropdown").forEach(other => {
                    if (other !== parent) other.classList.remove("is-open");
                });
                parent.classList.toggle("is-open");
            });
        });
    }
});