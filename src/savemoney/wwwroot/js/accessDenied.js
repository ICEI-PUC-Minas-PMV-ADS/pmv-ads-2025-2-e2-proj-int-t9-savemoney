/* ============================================================================
   ACCESS DENIED
   ============================================================================ */

(() => {
    'use strict';

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeAccessDenied);

    function initializeAccessDenied() {
        console.log('Access Denied module loaded');

        // Adiciona shake animation no ícone
        addShakeAnimation();
    }

    /* Shake Animation no Ícone
       ======================================================================== */
    function addShakeAnimation() {
        const iconWrapper = document.querySelector('.denied-icon-wrapper');

        if (!iconWrapper) return;

        // Shake a cada 5 segundos
        setInterval(() => {
            iconWrapper.style.animation = 'none';
            setTimeout(() => {
                iconWrapper.style.animation = 'pulse 2s ease-in-out infinite, shake 0.5s ease-in-out';
            }, 10);
        }, 5000);

        // Adiciona CSS da shake animation
        addShakeStyles();
    }

    /* Shake Styles
       ======================================================================== */
    function addShakeStyles() {
        if (document.getElementById('denied-shake-animation')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'denied-shake-animation';
        style.textContent = `
            @keyframes shake {
                0%, 100% { transform: translateX(0) scale(1); }
                10%, 30%, 50%, 70%, 90% { transform: translateX(-0.5rem) scale(1.05); }
                20%, 40%, 60%, 80% { transform: translateX(0.5rem) scale(1.05); }
            }
        `;
        document.head.appendChild(style);

        console.log('Shake animation adicionada');
    }

})();