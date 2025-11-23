document.addEventListener("DOMContentLoaded", function () {
    console.log("Budgets JS carregado vPremium Final!");

    // --- DELEGAÇÃO DE EVENTOS ---
    document.body.addEventListener('click', function (e) {
        if (e.target && e.target.id === 'addCategoryBtn') addCategoryRow();
        if (e.target && e.target.closest('.remove-cat-btn')) removerLinha(e.target.closest('.remove-cat-btn'));
    });

    document.body.addEventListener('input', function (e) {
        if (e.target && e.target.classList.contains('money-mask')) aplicarMascaraMoeda(e.target);
    });
});

// ==============================================================
// 1. GERENCIAMENTO DE MODAIS (CORREÇÃO DA TELA CINZA)
// ==============================================================

const modalContainer = document.getElementById("modal-container");

// Função Faxineira: Remove backdrops presos e reseta o body
function limparModais() {
    // 1. Esconde qualquer modal Bootstrap ativo
    document.querySelectorAll('.modal.show').forEach(el => {
        const instance = bootstrap.Modal.getInstance(el);
        if (instance) instance.hide();
    });

    // 2. Remove forçadamente os backdrops órfãos
    document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());

    // 3. Destrava o scroll do body
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

function carregarModalDetalhes(id) {
    limparModais(); // <--- LIMPEZA ANTES DE ABRIR

    fetch(`/Budgets/Details/${id}`)
        .then(res => {
            if (!res.ok) throw new Error("Erro");
            return res.text();
        })
        .then(html => {
            modalContainer.innerHTML = `
                <div class="modal fade" id="detailModal" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered modal-lg">
                        <div class="modal-content glass-panel" style="border: 1px solid var(--pm-glass-border);">
                            ${html}
                        </div>
                    </div>
                </div>`;

            const modalElement = document.getElementById('detailModal');

            // Só desenha quando o modal estiver visível (evita bugs visuais)
            modalElement.addEventListener('shown.bs.modal', function () {
                initDonutChart();
            });

            openBootstrapModal('detailModal');
        })
        .catch(console.error);
}

function carregarModalCriar() {
    limparModais(); // <--- LIMPEZA
    fetch("/Budgets/Create")
        .then(res => res.text())
        .then(html => {
            modalContainer.innerHTML = html;
            openBootstrapModal('budgetModal');
        });
}

function carregarModalEditar(id) {
    limparModais(); // <--- LIMPEZA CRÍTICA AQUI (Pois vem do modal de detalhes)

    fetch(`/Budgets/Edit/${id}`)
        .then(res => res.text())
        .then(html => {
            modalContainer.innerHTML = html;
            openBootstrapModal('budgetModal');
        });
}

// ==============================================================
// 2. GRÁFICO DONUT (CORREÇÃO DA MATEMÁTICA 50%)
// ==============================================================
function initDonutChart() {
    const canvas = document.getElementById("budgetDonutChart");
    if (!canvas || !canvas.dataset.chart) return;

    let categories = [];
    try { categories = JSON.parse(canvas.dataset.chart); } catch (e) { return; }

    // LER O LIMITE DO HTML (Se não existir, usa a soma como fallback)
    const limitAttr = canvas.dataset.limit;
    let limitTotal = limitAttr ? parseFloat(limitAttr) : 0;

    const ctx = canvas.getContext("2d");
    const width = canvas.width;
    const height = canvas.height;
    const centerX = width / 2;
    const centerY = height / 2;
    const radius = 75;
    const lineWidth = 14;
    const colors = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

    // Prepara dados
    let totalSpent = 0;
    categories.forEach((cat, i) => {
        cat.value = parseFloat(cat.value);
        if (cat.value < 0) cat.value = 0;
        totalSpent += cat.value;
        cat.color = colors[i % colors.length];
    });

    // Se não tiver limite definido (0), usa o total gasto para evitar divisão por zero
    // Isso mantém o gráfico cheio se não houver orçamento definido
    const denominator = limitTotal > 0 ? limitTotal : totalSpent;

    ctx.clearRect(0, 0, width, height);

    // 1. Fundo do Donut (Trilha completa)
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI);
    ctx.lineWidth = lineWidth;
    ctx.strokeStyle = "rgba(255, 255, 255, 0.05)";
    ctx.stroke();

    if (denominator === 0) return;

    // 2. Desenha os Arcos (Proporcionais ao LIMITE)
    let startAngle = -0.5 * Math.PI; // 12 horas

    categories.forEach(cat => {
        if (cat.value <= 0) return;

        // AQUI ESTÁ A CORREÇÃO MÁGICA: Divide pelo LIMITE, não pelo Gasto Total
        // Math.min garante que não desenhe mais que 360 graus se estourar
        const share = Math.min(cat.value / denominator, 1);
        const sliceAngle = share * 2 * Math.PI;
        const endAngle = startAngle + sliceAngle;

        ctx.beginPath();
        ctx.arc(centerX, centerY, radius, startAngle, endAngle);
        ctx.lineWidth = lineWidth;
        ctx.strokeStyle = cat.color;
        ctx.lineCap = 'round';
        ctx.stroke();

        startAngle = endAngle;
    });
}

// ==============================================================
// 3. UTILITÁRIOS E TABELA
// ==============================================================

function addCategoryRow() {
    const container = document.getElementById('categoriesContainer');
    const emptyMsg = document.getElementById('emptyTableMsg');
    const template = document.getElementById('categoryRowTemplate').innerHTML;
    const currentIndex = container.querySelectorAll('.category-row').length;
    container.insertAdjacentHTML('beforeend', template.replace(/{INDEX}/g, currentIndex));
    if (emptyMsg) emptyMsg.classList.add('d-none');
}

function removerLinha(btn) {
    btn.closest('.category-row').remove();
    const container = document.getElementById('categoriesContainer');
    if (container.children.length === 0) document.getElementById('emptyTableMsg').classList.remove('d-none');

    // Reindexar
    container.querySelectorAll('.category-row').forEach((row, index) => {
        const select = row.querySelector('select');
        const input = row.querySelector('input');
        if (select) select.name = `Categories[${index}].CategoryId`;
        if (input) input.name = `Categories[${index}].Limit`;
    });
}

function aplicarMascaraMoeda(input) {
    let value = input.value.replace(/[^0-9,]/g, '');
    const parts = value.split(',');
    if (parts.length > 2) value = parts[0] + ',' + parts.slice(1).join('');
    if (value !== input.value) input.value = value;
}

let idParaExcluir = 0;
function confirmarExclusao(id) {
    idParaExcluir = id;
    limparModais(); // Limpa antes de abrir confirmação
    openBootstrapModal('deleteModal');
}

function executarExclusao() {
    if (idParaExcluir > 0) {
        fetch("/Budgets/Delete/" + idParaExcluir, { method: 'POST' })
            .then(r => {
                if (r.ok) window.location.reload();
                else alert("Erro ao excluir.");
            })
            .catch(() => alert("Erro de conexão."));
    }
}