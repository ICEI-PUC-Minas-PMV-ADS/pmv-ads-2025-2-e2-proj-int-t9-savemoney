/* ============================================================================
   DESPESAS - CONTROLE DE GASTOS
   ============================================================================ */

(() => {
    'use strict';

    // Elementos principais
    const modalContainer = document.getElementById('modal-container');
    let deleteTargetId = 0;

    // Mapa de símbolos de moeda
    const currencySymbols = {
        'BRL': 'R$',
        'USD': '$',
        'EUR': '€',
        'GBP': '£',
        'JPY': '¥'
    };

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeDespesas);

    function initializeDespesas() {
        console.log('✓ Despesas module loaded');
        setupEventDelegation();
    }

    /* Delegação de Eventos
       ======================================================================== */
    function setupEventDelegation() {
        // Delegação para mudanças em campos
        document.body.addEventListener('change', handleBodyChange);

        // Delegação para inputs (máscara de moeda)
        document.body.addEventListener('input', handleBodyInput);
    }

    function handleBodyChange(e) {
        const target = e.target;

        // Troca de moeda
        if (target.id === 'currencyType') {
            updateCurrencySymbol(target.value);
        }

        // Toggle de recorrência
        if (target.id === 'isRecurringSwitch') {
            toggleRecurrencePanel(target.checked);
        }
    }

    function handleBodyInput(e) {
        const target = e.target;

        // Máscara de moeda no campo Valor
        if (target.name === 'Valor') {
            applyMoneyMask(target);
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

    // Carrega modal de criação
    window.carregarModalCriar = function () {
        cleanupModals();

        fetch('/Despesas/Create')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="despesaModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('despesaModal');

                // Inicializa estado visual
                initializeModalState();
            })
            .catch(error => {
                console.error('Error loading create form:', error);
                alert('Erro ao carregar formulário.');
            });
    };

    // Carrega modal de edição
    window.carregarModalEditar = function (id) {
        cleanupModals();

        fetch(`/Despesas/Edit/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="despesaModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('despesaModal');

                // Inicializa estado visual com dados existentes
                initializeModalState();
            })
            .catch(error => {
                console.error('Error loading edit form:', error);
                alert('Erro ao carregar formulário de edição.');
            });
    };

    /* Inicialização do Estado do Modal
       ======================================================================== */
    function initializeModalState() {
        // Atualiza símbolo de moeda
        const currencySelect = document.getElementById('currencyType');
        if (currencySelect) {
            updateCurrencySymbol(currencySelect.value);
        }

        // Atualiza painel de recorrência
        const recurrenceSwitch = document.getElementById('isRecurringSwitch');
        if (recurrenceSwitch) {
            toggleRecurrencePanel(recurrenceSwitch.checked);
        }
    }

    /* Funções de UI
       ======================================================================== */
    function updateCurrencySymbol(currencyType) {
        const symbolElement = document.getElementById('currencySymbol');
        if (symbolElement) {
            symbolElement.textContent = currencySymbols[currencyType] || '$';
        }
    }

    function toggleRecurrencePanel(isVisible) {
        const recurrencePanel = document.getElementById('recurrenceOptions');
        if (recurrencePanel) {
            recurrencePanel.style.display = isVisible ? 'grid' : 'none';
        }
    }

    /* Máscara de Moeda
       ======================================================================== */
    function applyMoneyMask(input) {
        let value = input.value;

        // Remove tudo que não é dígito ou vírgula
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

    /* Exclusão de Despesa
       ======================================================================== */
    window.confirmarExclusao = function (id) {
        deleteTargetId = id;
        cleanupModals();
        openModal('deleteModal');
    };

    window.executarExclusao = function () {
        if (deleteTargetId <= 0) return;

        fetch(`/Despesas/Delete/${deleteTargetId}`, {
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
                console.error('Error deleting despesa:', error);
                alert('Erro ao excluir despesa. Tente novamente.');
            });
    };

})();