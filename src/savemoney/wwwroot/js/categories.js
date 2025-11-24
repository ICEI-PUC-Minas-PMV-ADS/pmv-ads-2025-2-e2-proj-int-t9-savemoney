document.addEventListener("DOMContentLoaded", function () {
    console.log("Categories JS Loaded!");
});

const modalContainer = document.getElementById("modal-container");

// Função Faxineira (Impede tela travada)
function limparModais() {
    document.querySelectorAll('.modal.show').forEach(el => {
        const instance = bootstrap.Modal.getInstance(el);
        if (instance) instance.hide();
    });
    document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
}

function openBootstrapModal(modalId) {
    const modalEl = document.getElementById(modalId);
    if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
}

// --- CRUD ---

function carregarModalCriar() {
    limparModais(); // Limpa antes de abrir
    console.log("Abrindo modal Criar...");

    fetch("/Categories/Create")
        .then(res => {
            if (!res.ok) throw new Error("Erro ao carregar modal");
            return res.text();
        })
        .then(html => {
            modalContainer.innerHTML = `
                <div class="modal fade" id="categoryModal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content glass-panel" style="border: 1px solid var(--glass-border);">
                            ${html}
                        </div>
                    </div>
                </div>`;
            openBootstrapModal('categoryModal');
        })
        .catch(err => console.error("Erro Fetch:", err));
}

function carregarModalEditar(id) {
    limparModais();
    console.log("Abrindo modal Editar...", id);

    fetch(`/Categories/Edit/${id}`)
        .then(res => res.text())
        .then(html => {
            modalContainer.innerHTML = `
                <div class="modal fade" id="categoryModal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content glass-panel" style="border: 1px solid var(--glass-border);">
                            ${html}
                        </div>
                    </div>
                </div>`;
            openBootstrapModal('categoryModal');
        })
        .catch(err => console.error(err));
}

let idParaExcluir = 0;

function confirmarExclusao(id) {
    idParaExcluir = id;
    limparModais();
    openBootstrapModal('deleteModal');
}

function executarExclusao() {
    if (idParaExcluir > 0) {
        fetch(`/Categories/Delete/${idParaExcluir}`, {
            method: 'POST'
        })
            .then(res => {
                if (res.ok) {
                    // Sucesso: Recarrega a página para atualizar a lista
                    window.location.reload();
                } else {
                    // Erro (ex: Categoria em uso)
                    return res.text().then(text => alert("Erro: " + text));
                }
            })
            .catch(err => alert("Erro de conexão."));
    }
}