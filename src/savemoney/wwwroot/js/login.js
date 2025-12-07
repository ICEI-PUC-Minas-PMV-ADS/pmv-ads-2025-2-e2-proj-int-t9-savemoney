/* ============================================================================
   LOGIN - VERSÃO CORRIGIDA
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let form = null;
    let emailInput = null;
    let passwordInput = null;
    let togglePasswordBtn = null;
    let submitBtn = null;

    // Estado
    let isPasswordVisible = false;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeLogin);

    function initializeLogin() {
        console.log('Login module loaded');

        // Cache de elementos
        form = document.getElementById('loginForm');
        emailInput = document.querySelector('input[name="Email"]');
        passwordInput = document.getElementById('passwordInput');
        togglePasswordBtn = document.getElementById('togglePassword');
        submitBtn = form?.querySelector('button[type="submit"]');

        if (!form) {
            console.error('Formulário não encontrado');
            return;
        }

        setupEventListeners();
        startFloatingCardsAnimation();
        addAnimationStyles();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Toggle password visibility
        if (togglePasswordBtn) {
            togglePasswordBtn.addEventListener('click', handleTogglePassword);
        }

        // Form submit
        form.addEventListener('submit', handleFormSubmit);

        // Real-time validation
        if (emailInput) {
            emailInput.addEventListener('blur', () => validateEmail(emailInput.value));
            emailInput.addEventListener('input', () => clearFieldError(emailInput));
        }

        if (passwordInput) {
            passwordInput.addEventListener('blur', () => validatePassword(passwordInput.value));
            passwordInput.addEventListener('input', () => clearFieldError(passwordInput));
        }

        console.log('Event listeners configurados');
    }

    /* Toggle Password Visibility
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

        // Animação de transição
        passwordInput.style.transition = 'all 0.2s ease';
    }

    /* Form Validation
       ======================================================================== */
    function handleFormSubmit(e) {
        // Remove validações anteriores
        clearAllErrors();

        const email = emailInput.value.trim();
        const password = passwordInput.value;

        let isValid = true;

        // Validar email
        if (!validateEmail(email)) {
            isValid = false;
        }

        // Validar senha
        if (!validatePassword(password)) {
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
            return false;
        }

        // Mostra loading
        showLoadingState();
    }

    function validateEmail(email) {
        // Limpa erro anterior
        clearFieldError(emailInput);

        if (!email) {
            showFieldError(emailInput, 'O email é obrigatório');
            return false;
        }

        // Regex para validação de email
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showFieldError(emailInput, 'Digite um email válido');
            return false;
        }

        emailInput.classList.add('is-valid');
        return true;
    }

    function validatePassword(password) {
        // Limpa erro anterior
        clearFieldError(passwordInput);

        if (!password) {
            showFieldError(passwordInput, 'A senha é obrigatória');
            return false;
        }

        if (password.length < 6) {
            showFieldError(passwordInput, 'A senha deve ter no mínimo 6 caracteres');
            return false;
        }

        passwordInput.classList.add('is-valid');
        return true;
    }

    /* Error Handling
       ======================================================================== */
    function showFieldError(input, message) {
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');

        const errorSpan = input.parentElement.querySelector('.form-error') ||
            input.closest('.form-group').querySelector('.form-error');

        if (errorSpan) {
            errorSpan.textContent = message;
            errorSpan.style.display = 'block';
        }

        // Animação shake
        input.style.animation = 'shake 0.5s ease-in-out';
        setTimeout(() => {
            input.style.animation = '';
        }, 500);
    }

    function clearFieldError(input) {
        input.classList.remove('is-invalid', 'is-valid');

        const errorSpan = input.parentElement.querySelector('.form-error') ||
            input.closest('.form-group').querySelector('.form-error');

        if (errorSpan) {
            errorSpan.textContent = '';
            errorSpan.style.display = 'none';
        }
    }

    function clearAllErrors() {
        const inputs = form.querySelectorAll('.form-input');
        inputs.forEach(input => clearFieldError(input));
    }

    /* Loading State
       ======================================================================== */
    function showLoadingState() {
        if (!submitBtn) return;

        submitBtn.disabled = true;
        const originalText = submitBtn.innerHTML;
        submitBtn.dataset.originalText = originalText;

        submitBtn.innerHTML = `
            <span class="material-symbols-outlined spinning" aria-hidden="true">sync</span>
            Entrando...
        `;

        submitBtn.style.opacity = '0.7';
    }

    /* Floating Cards Animation
       ======================================================================== */
    function startFloatingCardsAnimation() {
        const cards = document.querySelectorAll('.floating-card');

        cards.forEach((card, index) => {
            // Adiciona delay inicial baseado no índice
            card.style.animationDelay = `${index * 0.5}s`;

            // Adiciona parallax ao mover o mouse
            document.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const cardCenterX = rect.left + rect.width / 2;
                const cardCenterY = rect.top + rect.height / 2;

                const deltaX = (e.clientX - cardCenterX) / 50;
                const deltaY = (e.clientY - cardCenterY) / 50;

                card.style.transform = `translate(${deltaX}px, ${deltaY}px)`;
            });
        });

        console.log('Floating cards animation iniciada');
    }

    /* Animation Styles
       ======================================================================== */
    function addAnimationStyles() {
        if (document.getElementById('login-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'login-animations';
        style.textContent = `
            @keyframes shake {
                0%, 100% { transform: translateX(0); }
                25% { transform: translateX(-0.625rem); }
                75% { transform: translateX(0.625rem); }
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
                border-color: var(--login-success);
            }

            .form-input.is-valid:focus {
                box-shadow: 0 0 0 0.25rem rgba(16, 185, 129, 0.25);
            }

            .form-input.is-invalid {
                border-color: var(--login-danger);
            }

            .form-input.is-invalid:focus {
                box-shadow: 0 0 0 0.25rem rgba(239, 68, 68, 0.25);
            }

            /* Smooth transitions */
            .form-input {
                transition: border-color 0.3s ease, box-shadow 0.3s ease;
            }

            .password-toggle .material-symbols-outlined {
                transition: transform 0.2s ease;
            }

            .password-toggle:active .material-symbols-outlined {
                transform: scale(0.9);
            }
        `;
        document.head.appendChild(style);

        console.log('Animation styles adicionados');
    }

    /* Keyboard Navigation Enhancement
       ======================================================================== */
    document.addEventListener('keydown', (e) => {
        // Enter no email vai para senha
        if (e.key === 'Enter' && document.activeElement === emailInput) {
            e.preventDefault();
            passwordInput.focus();
        }
    });

    /* Export para debugging (opcional)
       ======================================================================== */
    window.LoginModule = {
        validateEmail,
        validatePassword,
        clearAllErrors
    };

})();