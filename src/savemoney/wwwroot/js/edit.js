/* ============================================================================
   EDIT (MODAL)
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let modalOverlay = null;
    let editForm = null;
    let documentoInput = null;
    let passwordInput = null;
    let togglePasswordBtn = null;

    // Estado
    let isPasswordVisible = false;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeEdit);

    function initializeEdit() {
        console.log('Edit module loaded');

        modalOverlay = document.getElementById('editModal');
        editForm = document.getElementById('editForm');
        documentoInput = document.querySelector('input[name="Documento"]');
        passwordInput = document.getElementById('passwordInput');
        togglePasswordBtn = document.getElementById('togglePassword');

        if (!modalOverlay || !editForm) {
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

        // Toggle password
        if (togglePasswordBtn) {
            togglePasswordBtn.addEventListener('click', handleTogglePassword);
        }

        // Documento mask
        if (documentoInput) {
            documentoInput.addEventListener('input', handleDocumentoInput);
        }

        // Form submit
        editForm.addEventListener('submit', handleFormSubmit);

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

    /* Toggle Password
       ======================================================================== */
    function handleTogglePassword(e) {
        e.preventDefault();

        isPasswordVisible = !isPasswordVisible;

        if (isPasswordVisible) {
            passwordInput.type = 'text';
            togglePasswordBtn.querySelector('.material-symbols-outlined').textContent = 'visibility_off';
            togglePasswordBtn.setAttribute('aria-label', 'Ocultar senha');
        } else {
            passwordInput.type = 'password';
            togglePasswordBtn.querySelector('.material-symbols-outlined').textContent = 'visibility';
            togglePasswordBtn.setAttribute('aria-label', 'Mostrar senha');
        }
    }

    /* Documento Mask
       ======================================================================== */
    function handleDocumentoInput(e) {
        let value = e.target.value.replace(/\D/g, '');

        if (value.length <= 11) {
            // CPF: 000.000.000-00
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        } else {
            // CNPJ: 00.000.000/0000-00
            value = value.replace(/^(\d{2})(\d)/, '$1.$2');
            value = value.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
            value = value.replace(/\.(\d{3})(\d)/, '.$1/$2');
            value = value.replace(/(\d{4})(\d)/, '$1-$2');
        }

        e.target.value = value;
    }

    /* Handle Form Submit
       ======================================================================== */
    function handleFormSubmit(e) {
        // Deixa a validação do ASP.NET Core trabalhar
        // Mas mostra loading
        showLoadingState();
    }

    /* Close Modal
       ======================================================================== */
    function closeModal() {
        modalOverlay.style.animation = 'fadeOut 0.3s ease';

        setTimeout(() => {
            window.location.href = '/Usuarios';
        }, 300);
    }

    /* Loading State
       ======================================================================== */
    function showLoadingState() {
        const submitBtn = editForm.querySelector('button[type="submit"]');

        if (!submitBtn) return;

        submitBtn.disabled = true;
        const originalText = submitBtn.innerHTML;
        submitBtn.dataset.originalText = originalText;

        submitBtn.innerHTML = `
            <span class="material-symbols-outlined spinning" aria-hidden="true">sync</span>
            Salvando...
        `;

        submitBtn.style.opacity = '0.7';
    }

    /* Animation Styles
       ======================================================================== */
    function addAnimationStyles() {
        if (document.getElementById('edit-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'edit-animations';
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

            .form-input.is-valid {
                border-color: var(--success);
            }

            .form-input.is-invalid {
                border-color: var(--danger);
            }
        `;
        document.head.appendChild(style);

        console.log('Animation styles adicionados');
    }

    /* Export para debugging
       ======================================================================== */
    window.EditModule = {
        closeModal
    };

})();