/* ============================================================================
   TOAST MESSAGES (Sistema de Notificações)
   ============================================================================ */

(() => {
    'use strict';

    // Configuração
    const AUTO_DISMISS_TIME = 5000; // 5 segundos

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeToasts);

    function initializeToasts() {
        console.log('Toast messages loaded');

        const toasts = document.querySelectorAll('.toast-message');

        toasts.forEach((toast) => {
            setupToastEvents(toast);
            setupAutoDismiss(toast);
        });
    }

    /* Setup Toast Events
       ======================================================================== */
    function setupToastEvents(toast) {
        // Botão de fechar
        const closeBtn = toast.querySelector('.toast-close');

        if (closeBtn) {
            closeBtn.addEventListener('click', () => {
                dismissToast(toast);
            });
        }

        // Click no toast inteiro também fecha (opcional)
        toast.addEventListener('click', (e) => {
            if (e.target === toast || e.target.classList.contains('toast-content')) {
                dismissToast(toast);
            }
        });
    }

    /* Setup Auto Dismiss
       ======================================================================== */
    function setupAutoDismiss(toast) {
        setTimeout(() => {
            dismissToast(toast);
        }, AUTO_DISMISS_TIME);
    }

    /* Dismiss Toast
       ======================================================================== */
    function dismissToast(toast) {
        toast.classList.add('hiding');

        setTimeout(() => {
            toast.remove();

            // Se não há mais toasts, remove o container
            const container = document.getElementById('toastContainer');
            if (container && container.children.length === 0) {
                container.style.display = 'none';
            }
        }, 300);
    }

    /* Create Toast Programmatically (opcional - uso via JS)
       ======================================================================== */
    window.showToast = function (message, type = 'success') {
        const container = document.getElementById('toastContainer');

        if (!container) {
            console.error('Toast container not found');
            return;
        }

        const icons = {
            success: 'check_circle',
            error: 'error',
            info: 'info',
            warning: 'warning'
        };

        const titles = {
            success: 'Sucesso!',
            error: 'Erro!',
            info: 'Informação',
            warning: 'Atenção!'
        };

        const toast = document.createElement('div');
        toast.className = `toast-message toast-${type}`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');

        toast.innerHTML = `
            <div class="toast-icon">
                <span class="material-symbols-outlined" aria-hidden="true">${icons[type]}</span>
            </div>
            <div class="toast-content">
                <strong class="toast-title">${titles[type]}</strong>
                <p class="toast-text">${message}</p>
            </div>
            <button type="button" class="toast-close" aria-label="Fechar">
                <span class="material-symbols-outlined" aria-hidden="true">close</span>
            </button>
        `;

        container.appendChild(toast);
        container.style.display = 'flex';

        setupToastEvents(toast);
        setupAutoDismiss(toast);
    };

})();