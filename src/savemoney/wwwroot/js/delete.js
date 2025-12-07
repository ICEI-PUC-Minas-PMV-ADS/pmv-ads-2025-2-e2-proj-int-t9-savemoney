/* ============================================================================
   DELETE (MODAL)
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let modalOverlay = null;
    let deleteForm = null;
    let btnDelete = null;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeDelete);

    function initializeDelete() {
        console.log('Delete module loaded');

        modalOverlay = document.getElementById('deleteModal');
        deleteForm = document.getElementById('deleteForm');
        btnDelete = document.getElementById('btnDelete');

        if (!modalOverlay || !deleteForm) {
            console.error('Elementos não encontrados');
            return;
        }

        setupEventListeners();
        addAnimationStyles();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Fechar modal ao clicar no overlay
        modalOverlay.addEventListener('click', handleOverlayClick);

        // Fechar modal com ESC
        document.addEventListener('keydown', handleKeyDown);

        // Confirmar exclusão
        deleteForm.addEventListener('submit', handleDeleteSubmit);

        // Prevenir scroll do body
        document.body.style.overflow = 'hidden';

        console.log('Event listeners configurados');
    }

    /* Handle Overlay Click
       ======================================================================== */
    function handleOverlayClick(e) {
        if (e.target === modalOverlay) {
            closeModal();
        }
    }

    /* Handle Keyboard
       ======================================================================== */
    function handleKeyDown(e) {
        if (e.key === 'Escape') {
            closeModal();
        }
    }

    /* Handle Delete Submit
       ======================================================================== */
    function handleDeleteSubmit(e) {
        e.preventDefault();

        // Confirmação dupla
        const confirmed = confirm('Tem certeza absoluta que deseja excluir este usuário?\n\nEsta ação é IRREVERSÍVEL!');

        if (!confirmed) {
            return;
        }

        // Mostra loading
        showLoadingState();

        // Submete o form
        deleteForm.submit();
    }

    /* Close Modal
       ======================================================================== */
    function closeModal() {
        modalOverlay.style.animation = 'fadeOut 0.3s ease';

        setTimeout(() => {
            // Redireciona para Index
            window.location.href = '/Usuarios';
        }, 300);
    }

    /* Loading State
       ======================================================================== */
    function showLoadingState() {
        if (!btnDelete) return;

        btnDelete.disabled = true;
        const originalText = btnDelete.innerHTML;
        btnDelete.dataset.originalText = originalText;

        btnDelete.innerHTML = `
            <span class="material-symbols-outlined spinning" aria-hidden="true">sync</span>
            Excluindo...
        `;

        btnDelete.style.opacity = '0.7';
    }

    /* Animation Styles
       ======================================================================== */
    function addAnimationStyles() {
        if (document.getElementById('delete-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'delete-animations';
        style.textContent = `
            @keyframes fadeOut {
                from {
                    opacity: 1;
                }
                to {
                    opacity: 0;
                }
            }

            @keyframes spin {
                from { transform: rotate(0deg); }
                to { transform: rotate(360deg); }
            }

            .spinning {
                display: inline-block;
                animation: spin 1s linear infinite;
            }
        `;
        document.head.appendChild(style);

        console.log('Animation styles adicionados');
    }

    /* Export para debugging
       ======================================================================== */
    window.DeleteModule = {
        closeModal
    };

})();