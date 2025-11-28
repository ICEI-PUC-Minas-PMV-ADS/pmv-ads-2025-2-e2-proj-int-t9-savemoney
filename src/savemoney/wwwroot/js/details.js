/* ============================================================================
   DETAILS (MODAL)
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let modalOverlay = null;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeDetails);

    function initializeDetails() {
        console.log('Details module loaded');

        modalOverlay = document.getElementById('detailsModal');

        if (!modalOverlay) {
            console.error('Modal não encontrado');
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

        // Prevenir scroll do body
        document.body.style.overflow = 'hidden';

        console.log('Event listeners configurados');
    }

    /* Handle Overlay Click
       ======================================================================== */
    function handleOverlayClick(e) {
        // Fecha apenas se clicar fora do card
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

    /* Close Modal
       ======================================================================== */
    function closeModal() {
        modalOverlay.style.animation = 'fadeOut 0.3s ease';

        setTimeout(() => {
            // Redireciona para Index
            window.location.href = '/Usuarios';
        }, 300);
    }

    /* Animation Styles
       ======================================================================== */
    function addAnimationStyles() {
        if (document.getElementById('details-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'details-animations';
        style.textContent = `
            @keyframes fadeOut {
                from {
                    opacity: 1;
                }
                to {
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);

        console.log('Animation styles adicionados');
    }

    /* Export para debugging
       ======================================================================== */
    window.DetailsModule = {
        closeModal
    };

})();