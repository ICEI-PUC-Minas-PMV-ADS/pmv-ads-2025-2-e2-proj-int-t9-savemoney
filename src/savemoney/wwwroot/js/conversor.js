document.addEventListener("DOMContentLoaded", function () {
    console.log("Conversor JS: Ativo.");
});

const modalContainer = document.getElementById("modal-container");

// Função para fechar o modal visível (se houver)
function fecharModalAtivo() {
    const modalElement = document.querySelector('.modal.show');
    if (modalElement) {
        const modalInstance = bootstrap.Modal.getInstance(modalElement);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
}

// Função Faxineira para Modais
function limparModais() {
    // Apenas remove os artefatos, mas não esconde a instância principal
    document.querySelectorAll('.modal.show').forEach(el => {
        const instance = bootstrap.Modal.getInstance(el);
        if (instance) instance.hide();
    });
    document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
}

function openBootstrapModal(modalId) {
    const modalEl = document.getElementById(modalId);
    if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
}

// Helper para obter o token CSRF
function getAntiForgeryToken() {
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenElement ? tokenElement.value : '';
}

// --- CARREGAMENTO DE MODAIS (CRUD) ---
// ... (Funções carregarModalCriar, carregarModalEditar, carregarModalExcluir permanecem iguais) ...
function carregarModalCriar() {
    limparModais();
    fetch("/ConversorEnergias/Create")
        .then(res => res.text())
        .then(html => {
            modalContainer.innerHTML = `<div class="modal fade" id="conversorModal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered modal-lg">
                        <div class="modal-content glass-card">${html}</div>
                    </div>
                </div>`;
            openBootstrapModal('conversorModal');
        })
        .catch(err => console.error("Erro ao carregar modal Criar:", err));
}

function carregarModalEditar(id) {
    limparModais();
    fetch(`/ConversorEnergias/Edit/${id}`)
        .then(res => res.text())
        .then(html => {
            modalContainer.innerHTML = `<div class="modal fade" id="conversorModal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered modal-lg">
                        <div class="modal-content glass-card">${html}</div>
                    </div>
                </div>`;
            openBootstrapModal('conversorModal');
        })
        .catch(err => console.error("Erro ao carregar modal Editar:", err));
}

function carregarModalExcluir(id) {
    limparModais();
    fetch(`/ConversorEnergias/Delete/${id}`)
        .then(res => res.text())
        .then(html => {
            modalContainer.innerHTML = `<div class="modal fade" id="deleteModal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered modal-sm">
                        <div class="modal-content glass-card">${html}</div>
                    </div>
                </div>`;
            openBootstrapModal('deleteModal');
        })
        .catch(err => console.error("Erro ao carregar modal Excluir:", err));
}


// --- LÓGICA DE SUBMISSÃO (CREATE/EDIT/DELETE) ---

function submeterFormularioAjax(e) {
    e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);

    fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': getAntiForgeryToken()
        }
    })
        .then(response => {
            if (response.redirected) {
                // Ação 1: FECHA A JANELA ANTES DO REDIRECIONAMENTO
                fecharModalAtivo();
                // Ação 2: Redireciona para Index
                window.location.href = "/ConversorEnergias/Index";
                return;
            } else {
                return response.text();
            }
        })
        .then(html => {
            if (html) {
                // Falha de Validação: Injeta o HTML retornado (com erros)
                document.querySelector('.modal-content').innerHTML = html;
            }
        })
        .catch(err => console.error("Erro no submit AJAX:", err));
}

// --- DELEGAÇÃO DE EVENTOS ---
document.body.addEventListener('submit', function (e) {
    if (e.target && (e.target.id === 'formConversor' || e.target.id === 'formDelete')) {
        submeterFormularioAjax(e);
    }
});