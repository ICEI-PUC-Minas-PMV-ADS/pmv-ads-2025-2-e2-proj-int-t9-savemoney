/* ============================================================================
   RECEITAS
   ============================================================================ */

(() => {
    'use strict';

    // Estado da aplicação
    let deleteId = 0;
    let deleteTitulo = '';

    // Elementos do DOM
    const modalContainer = document.getElementById('modal-container');
    const deleteModal = document.getElementById('deleteModal');
    const confirmDeleteBtn = document.getElementById('confirm-delete-btn');

    // Símbolos de moeda
    const CURRENCY_SYMBOLS = {
        BRL: 'R$',
        USD: '$',
        EUR: '€',
        GBP: '£',
        JPY: '¥'
    };

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeReceitas);

    function initializeReceitas() {
        console.log('Receitas module loaded');

        setupEventListeners();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Botões de nova receita
        const btnNovaReceita = document.getElementById('btnNovaReceita');
        const btnNovaReceitaEmpty = document.getElementById('btnNovaReceitaEmpty');

        if (btnNovaReceita) {
            btnNovaReceita.addEventListener('click', carregarModalCriar);
        }

        if (btnNovaReceitaEmpty) {
            btnNovaReceitaEmpty.addEventListener('click', carregarModalCriar);
        }

        // Event delegation para botões dos cards
        document.addEventListener('click', handleCardActions);

        // Confirm delete button
        if (confirmDeleteBtn) {
            confirmDeleteBtn.addEventListener('click', executarExclusao);
        }

        // Event delegation para elementos do modal (dinâmicos)
        document.body.addEventListener('change', handleModalChanges);
        document.body.addEventListener('input', handleModalInputs);
    }

    /* Card Actions (Edit/Delete)
       ======================================================================== */
    function handleCardActions(e) {
        const btn = e.target.closest('[data-action]');
        if (!btn) return;

        const action = btn.dataset.action;
        const id = parseInt(btn.dataset.id);

        if (action === 'edit') {
            carregarModalEditar(id);
        } else if (action === 'delete') {
            const titulo = btn.dataset.titulo;
            confirmarExclusao(id, titulo);
        }
    }

    /* Modal - Criar
       ======================================================================== */
    function carregarModalCriar() {
        if (!modalContainer) {
            console.error('Modal container not found');
            return;
        }

        showLoadingState();

        fetch('/Receitas/Create')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = html;
                openModal('receitaModal');
                initializeModalComponents();
            })
            .catch(error => {
                console.error('Erro ao carregar modal:', error);
                showErrorMessage('Não foi possível carregar o formulário');
            })
            .finally(() => {
                hideLoadingState();
            });
    }

    /* Modal - Editar
       ======================================================================== */
    function carregarModalEditar(id) {
        if (!modalContainer) {
            console.error('Modal container not found');
            return;
        }

        if (!id || id <= 0) {
            console.error('ID inválido:', id);
            return;
        }

        showLoadingState();

        fetch(`/Receitas/Edit/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = html;
                openModal('receitaModal');
                initializeModalComponents();
            })
            .catch(error => {
                console.error('Erro ao carregar modal:', error);
                showErrorMessage('Não foi possível carregar os dados');
            })
            .finally(() => {
                hideLoadingState();
            });
    }

    /* Modal Components Initialization
       ======================================================================== */
    function initializeModalComponents() {
        // Inicializa estado do toggle de recorrência
        const recurrenceSwitch = document.getElementById('isRecurringSwitch');
        if (recurrenceSwitch) {
            updateRecurrencePanel(recurrenceSwitch.checked);
        }

        // Inicializa símbolo de moeda
        const currencySelect = document.getElementById('currencyType');
        if (currencySelect) {
            updateCurrencySymbol(currencySelect.value);
        }

        // Aplica máscara de moeda no campo valor
        const valorInput = document.querySelector('input[name="Valor"]');
        if (valorInput) {
            applyCurrencyMask(valorInput);
        }
    }

    /* Modal Changes Handler
       ======================================================================== */
    function handleModalChanges(e) {
        // Currency selector
        if (e.target && e.target.id === 'currencyType') {
            updateCurrencySymbol(e.target.value);
        }

        // Recurrence toggle
        if (e.target && e.target.id === 'isRecurringSwitch') {
            updateRecurrencePanel(e.target.checked);
        }
    }

    /* Modal Inputs Handler (Máscaras)
       ======================================================================== */
    function handleModalInputs(e) {
        // Máscara de moeda no campo Valor
        if (e.target && e.target.name === 'Valor') {
            applyCurrencyMask(e.target);
        }
    }

    /* Currency Symbol Update
       ======================================================================== */
    function updateCurrencySymbol(currency) {
        const symbolElement = document.getElementById('currencySymbol');
        if (symbolElement) {
            symbolElement.textContent = CURRENCY_SYMBOLS[currency] || '$';
        }
    }

    /* Recurrence Panel Toggle
       ======================================================================== */
    function updateRecurrencePanel(isVisible) {
        const panel = document.getElementById('recurrenceOptions');
        if (panel) {
            panel.style.display = isVisible ? 'grid' : 'none';
        }
    }

    /* Currency Mask
       ======================================================================== */
    function applyCurrencyMask(input) {
        if (!input) return;

        let value = input.value;

        // Remove tudo que não é dígito ou vírgula
        value = value.replace(/[^0-9,]/g, '');

        // Garante apenas uma vírgula
        const parts = value.split(',');
        if (parts.length > 2) {
            value = parts[0] + ',' + parts.slice(1).join('');
        }

        // Limita casas decimais a 2
        if (parts.length === 2 && parts[1].length > 2) {
            value = parts[0] + ',' + parts[1].substring(0, 2);
        }

        if (value !== input.value) {
            input.value = value;
        }
    }

    /* Delete - Confirmação
       ======================================================================== */
    function confirmarExclusao(id, titulo) {
        if (!id || id <= 0) {
            console.error('ID inválido para exclusão:', id);
            return;
        }

        deleteId = id;
        deleteTitulo = titulo || 'esta receita';

        // Atualiza texto do modal
        const tituloElement = document.getElementById('delete-titulo');
        if (tituloElement) {
            tituloElement.textContent = deleteTitulo;
        }

        // Abre modal de confirmação
        openModal('deleteModal');
    }

    /* Delete - Execução
       ======================================================================== */
    function executarExclusao() {
        if (!deleteId || deleteId <= 0) {
            console.error('ID inválido para exclusão');
            return;
        }

        // Desabilita botão
        if (confirmDeleteBtn) {
            confirmDeleteBtn.disabled = true;
            confirmDeleteBtn.innerHTML = `
                <span class="material-symbols-outlined spinning">sync</span>
                Excluindo...
            `;
        }

        fetch(`/Receitas/Delete/${deleteId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    // Fecha modal e recarrega página
                    closeModal('deleteModal');
                    window.location.reload();
                } else {
                    throw new Error('Erro ao excluir receita');
                }
            })
            .catch(error => {
                console.error('Erro ao excluir:', error);
                showErrorMessage('Não foi possível excluir a receita');

                // Restaura botão
                if (confirmDeleteBtn) {
                    confirmDeleteBtn.disabled = false;
                    confirmDeleteBtn.innerHTML = `
                        <span class="material-symbols-outlined">delete_forever</span>
                        Confirmar Exclusão
                    `;
                }
            });
    }

    /* Modal Helpers
       ======================================================================== */
    function openModal(modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement && typeof bootstrap !== 'undefined') {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        }
    }

    function closeModal(modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement && typeof bootstrap !== 'undefined') {
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) {
                modal.hide();
            }
        }
    }

    /* Loading & Error States
       ======================================================================== */
    function showLoadingState() {
        // Implementar se necessário
    }

    function hideLoadingState() {
        // Implementar se necessário
    }

    function showErrorMessage(message) {
        // Simples alert por agora
        alert(message);

        // TODO: Implementar toast notification
    }

    /* Animação de Spinner
       ======================================================================== */
    if (!document.querySelector('.spinning-animation')) {
        const style = document.createElement('style');
        style.className = 'spinning-animation';
        style.textContent = `
            @keyframes spin-icon {
                from { transform: rotate(0deg); }
                to { transform: rotate(360deg); }
            }
            .spinning {
                display: inline-block;
                animation: spin-icon 1s linear infinite;
            }
        `;
        document.head.appendChild(style);
    }

    /* Smooth Scroll
       ======================================================================== */
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

            if (href === '#' || href === '#!') return;

            e.preventDefault();

            const target = document.querySelector(href);
            if (target) {
                const headerOffset = 100;
                const elementPosition = target.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.scrollY - headerOffset;

                window.scrollTo({
                    top: offsetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });

})();