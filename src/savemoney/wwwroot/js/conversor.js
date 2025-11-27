/* ============================================================================
   CONVERSOR ENERGIAS - CALCULADORA DE CONSUMO E CUSTO
   ============================================================================ */

(() => {
    'use strict';

    // Elementos principais
    const modalContainer = document.getElementById('modal-container');

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeConversor);

    function initializeConversor() {
        console.log('✓ Conversor de Energias module loaded');
        setupEventDelegation();
    }

    /* Delegação de Eventos
       ======================================================================== */
    function setupEventDelegation() {
        // Delegação para submissão de formulários
        document.body.addEventListener('submit', handleFormSubmit);
    }

    function handleFormSubmit(e) {
        const formId = e.target.id;

        if (formId === 'formConversor' || formId === 'formDelete') {
            e.preventDefault();
            submitFormAjax(e.target);
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

    // Fecha modal ativo específico
    function closeActiveModal() {
        const activeModal = document.querySelector('.modal.show');
        if (activeModal) {
            const instance = bootstrap.Modal.getInstance(activeModal);
            if (instance) {
                instance.hide();
            }
        }
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

        fetch('/ConversorEnergias/Create')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="conversorModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered modal-lg">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('conversorModal');
            })
            .catch(error => {
                console.error('Error loading create form:', error);
                alert('Erro ao carregar formulário.');
            });
    };

    // Carrega modal de edição
    window.carregarModalEditar = function (id) {
        cleanupModals();

        fetch(`/ConversorEnergias/Edit/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="conversorModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered modal-lg">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('conversorModal');
            })
            .catch(error => {
                console.error('Error loading edit form:', error);
                alert('Erro ao carregar formulário de edição.');
            });
    };

    // Carrega modal de exclusão
    window.carregarModalExcluir = function (id) {
        cleanupModals();

        fetch(`/ConversorEnergias/Delete/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered modal-sm">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('deleteModal');
            })
            .catch(error => {
                console.error('Error loading delete modal:', error);
                alert('Erro ao carregar modal de exclusão.');
            });
    };

    /* Submissão de Formulários via AJAX
       ======================================================================== */
    function submitFormAjax(form) {
        const formData = new FormData(form);

        fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': getAntiForgeryToken()
            }
        })
            .then(response => {
                // Se redirecionou, fecha o modal e redireciona
                if (response.redirected) {
                    closeActiveModal();
                    window.location.href = '/ConversorEnergias/Index';
                    return null;
                }
                // Se não redirecionou, retorna o HTML com erros de validação
                return response.text();
            })
            .then(html => {
                if (html) {
                    // Falha de validação: atualiza o conteúdo do modal com os erros
                    const modalContent = document.querySelector('.modal-content');
                    if (modalContent) {
                        modalContent.innerHTML = html;
                    }
                }
            })
            .catch(error => {
                console.error('Error submitting form:', error);
                alert('Erro ao processar formulário. Tente novamente.');
            });
    }

    /* Utilitários
       ======================================================================== */
    function getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

})();