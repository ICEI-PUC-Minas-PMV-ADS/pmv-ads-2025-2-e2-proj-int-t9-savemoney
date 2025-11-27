/* ============================================================================
   BUDGETS - GERENCIAMENTO DE ORÇAMENTOS
   ============================================================================ */

(() => {
    'use strict';

    // Elementos principais
    const modalContainer = document.getElementById('modal-container');
    let deleteTargetId = 0;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeBudgets);

    function initializeBudgets() {
        console.log('✓ Budgets module loaded');
        setupEventDelegation();
    }

    /* Delegação de Eventos
       ======================================================================== */
    function setupEventDelegation() {
        // Delegação para botões dinâmicos
        document.body.addEventListener('click', handleBodyClick);

        // Delegação para inputs de máscara
        document.body.addEventListener('input', handleBodyInput);
    }

    function handleBodyClick(e) {
        const target = e.target;

        // Adicionar categoria
        if (target.id === 'addCategoryBtn' || target.closest('#addCategoryBtn')) {
            e.preventDefault();
            addCategoryRow();
        }

        // Remover linha (detecta vários seletores possíveis)
        const removeBtn = target.closest('.btn-icon.danger');
        if (removeBtn && removeBtn.onclick?.toString().includes('removerLinha')) {
            // Deixa o onclick inline funcionar
            return;
        }
    }

    function handleBodyInput(e) {
        if (e.target.classList.contains('money-mask')) {
            applyMoneyMask(e.target);
        }
    }

    /* Gerenciamento de Modais
       ======================================================================== */

    // Limpa qualquer modal preso e reseta o body
    function cleanupModals() {
        // Fecha modais Bootstrap ativos
        document.querySelectorAll('.modal.show').forEach(modal => {
            const instance = bootstrap.Modal.getInstance(modal);
            if (instance) {
                instance.hide();
            }
        });

        // Remove backdrops órfãos
        document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
            backdrop.remove();
        });

        // Reseta estado do body
        document.body.classList.remove('modal-open');
        document.body.style.overflow = '';
        document.body.style.paddingRight = '';
    }

    // Abre modal Bootstrap
    function openModal(modalId) {
        const modalElement = document.getElementById(modalId);
        if (!modalElement) {
            console.error(`Modal ${modalId} not found`);
            return;
        }

        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    }

    // Carrega modal de detalhes
    window.carregarModalDetalhes = function (id) {
        cleanupModals();

        fetch(`/Budgets/Details/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="detailModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered modal-lg">
                            <div class="modal-content glass-panel premium-modal">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                const modalElement = document.getElementById('detailModal');

                // Renderiza gráfico quando modal estiver visível
                modalElement.addEventListener('shown.bs.modal', () => {
                    initDonutChart();
                }, { once: true });

                openModal('detailModal');
            })
            .catch(error => {
                console.error('Error loading details:', error);
                alert('Erro ao carregar detalhes do orçamento.');
            });
    };

    // Carrega modal de criação
    window.carregarModalCriar = function () {
        cleanupModals();

        fetch('/Budgets/Create')
            .then(response => response.text())
            .then(html => {
                modalContainer.innerHTML = html;
                openModal('budgetModal');
            })
            .catch(error => {
                console.error('Error loading create form:', error);
                alert('Erro ao carregar formulário.');
            });
    };

    // Carrega modal de edição
    window.carregarModalEditar = function (id) {
        cleanupModals();

        fetch(`/Budgets/Edit/${id}`)
            .then(response => response.text())
            .then(html => {
                modalContainer.innerHTML = html;
                openModal('budgetModal');
            })
            .catch(error => {
                console.error('Error loading edit form:', error);
                alert('Erro ao carregar formulário de edição.');
            });
    };

    /* Gráfico Donut
       ======================================================================== */
    function initDonutChart() {
        const canvas = document.getElementById('budgetDonutChart');
        if (!canvas) return;

        const chartData = canvas.dataset.chart;
        const limitData = canvas.dataset.limit;

        if (!chartData) return;

        let categories = [];
        try {
            categories = JSON.parse(chartData);
        } catch (error) {
            console.error('Error parsing chart data:', error);
            return;
        }

        // Configurações do gráfico
        const ctx = canvas.getContext('2d');
        const width = canvas.width;
        const height = canvas.height;
        const centerX = width / 2;
        const centerY = height / 2;
        const radius = 75;
        const lineWidth = 14;
        const colors = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

        // Processa dados
        let totalSpent = 0;
        categories.forEach((category, index) => {
            category.value = parseFloat(category.value) || 0;
            if (category.value < 0) category.value = 0;
            totalSpent += category.value;
            category.color = colors[index % colors.length];
        });

        // Obtém o limite total
        const totalLimit = limitData ? parseFloat(limitData) : totalSpent;
        const denominator = totalLimit > 0 ? totalLimit : totalSpent;

        if (denominator === 0) return;

        // Limpa canvas
        ctx.clearRect(0, 0, width, height);

        // Desenha trilha de fundo
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI);
        ctx.lineWidth = lineWidth;
        ctx.strokeStyle = 'rgba(255, 255, 255, 0.05)';
        ctx.stroke();

        // Desenha arcos das categorias
        let startAngle = -0.5 * Math.PI;

        categories.forEach(category => {
            if (category.value <= 0) return;

            const percentage = Math.min(category.value / denominator, 1);
            const sliceAngle = percentage * 2 * Math.PI;
            const endAngle = startAngle + sliceAngle;

            ctx.beginPath();
            ctx.arc(centerX, centerY, radius, startAngle, endAngle);
            ctx.lineWidth = lineWidth;
            ctx.strokeStyle = category.color;
            ctx.lineCap = 'round';
            ctx.stroke();

            startAngle = endAngle;
        });
    }

    /* Gerenciamento de Categorias
       ======================================================================== */
    function addCategoryRow() {
        const container = document.getElementById('categoriesContainer');
        const emptyMsg = document.getElementById('emptyTableMsg');
        const template = document.getElementById('categoryRowTemplate');

        if (!container || !template) return;

        const currentIndex = container.querySelectorAll('.category-row').length;
        const newRow = template.innerHTML.replace(/{INDEX}/g, currentIndex);

        container.insertAdjacentHTML('beforeend', newRow);

        if (emptyMsg) {
            emptyMsg.classList.add('d-none');
        }
    }

    window.removerLinha = function (button) {
        const row = button.closest('.category-row');
        if (!row) return;

        row.remove();

        const container = document.getElementById('categoriesContainer');
        const emptyMsg = document.getElementById('emptyTableMsg');

        // Mostra mensagem vazia se não houver mais linhas
        if (container && container.children.length === 0 && emptyMsg) {
            emptyMsg.classList.remove('d-none');
        }

        // Reindexa as categorias restantes
        reindexCategories();
    };

    function reindexCategories() {
        const container = document.getElementById('categoriesContainer');
        if (!container) return;

        container.querySelectorAll('.category-row').forEach((row, index) => {
            const select = row.querySelector('select');
            const input = row.querySelector('input[type="text"]');

            if (select) {
                select.name = `Categories[${index}].CategoryId`;
            }
            if (input) {
                input.name = `Categories[${index}].Limit`;
            }
        });
    }

    /* Máscara de Moeda
       ======================================================================== */
    function applyMoneyMask(input) {
        let value = input.value;

        // Remove tudo exceto números e vírgula
        value = value.replace(/[^0-9,]/g, '');

        // Garante apenas uma vírgula
        const parts = value.split(',');
        if (parts.length > 2) {
            value = parts[0] + ',' + parts.slice(1).join('');
        }

        // Limita casas decimais a 2
        if (parts[1] && parts[1].length > 2) {
            value = parts[0] + ',' + parts[1].substring(0, 2);
        }

        input.value = value;
    }

    /* Exclusão de Orçamento
       ======================================================================== */
    window.confirmarExclusao = function (id) {
        deleteTargetId = id;
        cleanupModals();
        openModal('deleteModal');
    };

    window.executarExclusao = function () {
        if (deleteTargetId <= 0) return;

        fetch(`/Budgets/Delete/${deleteTargetId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    window.location.reload();
                } else {
                    throw new Error('Delete failed');
                }
            })
            .catch(error => {
                console.error('Error deleting budget:', error);
                alert('Erro ao excluir orçamento. Tente novamente.');
            });
    };

})();