document.addEventListener("DOMContentLoaded", function () {
    console.log("Despesas JS (Vanilla) carregado!");

    // --- DELEGAÇÃO DE EVENTOS (Vanilla Style) ---
    // Observa o 'body' para pegar eventos de elementos criados dinamicamente pelo Modal

    document.body.addEventListener('change', function (e) {

        // 1. Troca de Moeda
        if (e.target && e.target.id === 'currencyType') {
            const symbolMap = { "BRL": "R$", "USD": "$", "EUR": "€", "GBP": "£", "JPY": "¥" };
            const symbolSpan = document.getElementById('currencySymbol');
            if (symbolSpan) symbolSpan.textContent = symbolMap[e.target.value] || "$";
        }

        // 2. Switch Recorrência
        if (e.target && e.target.id === 'isRecurringSwitch') {
            const optionsDiv = document.getElementById('recurrenceOptions');
            if (optionsDiv) {
                // Usa grid para manter o layout de 2 colunas definido no CSS
                optionsDiv.style.display = e.target.checked ? 'grid' : 'none';
            }
        }
    });

    // 3. Máscara de Moeda Simples (Permite apenas números e uma vírgula)
    document.body.addEventListener('input', function (e) {
        if (e.target && e.target.name === 'Valor') {
            let value = e.target.value;

            // Remove tudo que não é dígito ou vírgula
            value = value.replace(/[^0-9,]/g, '');

            // Garante apenas uma vírgula
            const parts = value.split(',');
            if (parts.length > 2) {
                value = parts[0] + ',' + parts.slice(1).join('');
            }

            if (value !== e.target.value) {
                e.target.value = value;
            }
        }
    });
});

// --- FUNÇÕES GLOBAIS CHAMADAS PELO HTML ---

// Função auxiliar para abrir Modal Bootstrap via JS Puro
function openBootstrapModal(modalId) {
    const modalEl = document.getElementById(modalId);
    if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
}

// 1. Carregar Criar
function carregarModalCriar() {
    const container = document.getElementById("modal-container");
    container.innerHTML = ""; // Limpa modal anterior

    fetch("/Despesas/Create")
        .then(response => response.text())
        .then(html => {
            container.innerHTML = html;
            openBootstrapModal('despesaModal'); // ID do modal no _CreateOrEditModal

            // Inicializa estado visual (dispara evento change manualmente)
            const chk = document.getElementById('isRecurringSwitch');
            if (chk) chk.dispatchEvent(new Event('change', { bubbles: true }));
        })
        .catch(err => console.error("Erro ao carregar modal:", err));
}

// 2. Carregar Editar
function carregarModalEditar(id) {
    const container = document.getElementById("modal-container");
    container.innerHTML = "";

    fetch("/Despesas/Edit/" + id)
        .then(response => response.text())
        .then(html => {
            container.innerHTML = html;
            openBootstrapModal('despesaModal');

            // Atualiza visuais com base nos dados que vieram do banco
            const chk = document.getElementById('isRecurringSwitch');
            const currency = document.getElementById('currencyType');

            if (chk) chk.dispatchEvent(new Event('change', { bubbles: true }));
            if (currency) currency.dispatchEvent(new Event('change', { bubbles: true }));
        })
        .catch(err => console.error("Erro ao carregar modal:", err));
}

// 3. Exclusão
let idParaExcluir = 0;

function confirmarExclusao(id) {
    idParaExcluir = id;
    openBootstrapModal('deleteModal');
}

function executarExclusao() {
    if (idParaExcluir > 0) {
        fetch("/Despesas/Delete/" + idParaExcluir, {
            method: 'POST' // Importante: POST para segurança
        })
            .then(response => {
                if (response.ok) {
                    window.location.reload();
                } else {
                    alert("Erro ao excluir.");
                }
            })
            .catch(err => alert("Erro de rede ao excluir."));
    }
}