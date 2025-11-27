/* ============================================================================
   CATEGORIES - GERENCIAMENTO DE CATEGORIAS
   ============================================================================ */

(() => {
    'use strict';

    // Elementos principais
    const modalContainer = document.getElementById('modal-container');
    let deleteTargetId = 0;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeCategories);

    function initializeCategories() {
        console.log('✓ Categories module loaded');
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

        fetch('/Categories/Create')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="categoryModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('categoryModal');
            })
            .catch(error => {
                console.error('Error loading create form:', error);
                alert('Erro ao carregar formulário.');
            });
    };

    // Carrega modal de edição
    window.carregarModalEditar = function (id) {
        cleanupModals();

        fetch(`/Categories/Edit/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                modalContainer.innerHTML = `
                    <div class="modal fade" id="categoryModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content glass-panel">
                                ${html}
                            </div>
                        </div>
                    </div>
                `;

                openModal('categoryModal');
            })
            .catch(error => {
                console.error('Error loading edit form:', error);
                alert('Erro ao carregar formulário de edição.');
            });
    };

    /* Exclusão de Categoria
       ======================================================================== */
    window.confirmarExclusao = function (id) {
        deleteTargetId = id;
        cleanupModals();
        openModal('deleteModal');
    };

    window.executarExclusao = function () {
        if (deleteTargetId <= 0) return;

        fetch(`/Categories/Delete/${deleteTargetId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    window.location.reload();
                } else {
                    return response.text().then(text => {
                        throw new Error(text || 'Delete failed');
                    });
                }
            })
            .catch(error => {
                console.error('Error deleting category:', error);
                alert('Erro ao excluir categoria. Pode estar sendo usada em transações.');
            });
    };

})();