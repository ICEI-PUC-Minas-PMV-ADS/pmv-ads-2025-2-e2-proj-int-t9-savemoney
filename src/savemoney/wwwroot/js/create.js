/* ============================================================================
   CREATE (CADASTRO) - VERSÃO CORRIGIDA
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let form = null;
    let nomeInput = null;
    let emailInput = null;
    let documentoInput = null;
    let passwordInput = null;
    let togglePasswordBtn = null;
    let submitBtn = null;
    let strengthFill = null;
    let strengthText = null;

    // Estado
    let isPasswordVisible = false;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeCreate);

    function initializeCreate() {
        console.log('Create module loaded');

        // Cache de elementos
        form = document.getElementById('createForm');
        nomeInput = document.querySelector('input[name="Nome"]');
        emailInput = document.querySelector('input[name="Email"]');
        documentoInput = document.querySelector('input[name="Documento"]');
        passwordInput = document.getElementById('passwordInput');
        togglePasswordBtn = document.getElementById('togglePassword');
        submitBtn = form?.querySelector('button[type="submit"]');
        strengthFill = document.getElementById('strengthFill');
        strengthText = document.getElementById('strengthText');

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
        // Toggle password
        if (togglePasswordBtn) {
            togglePasswordBtn.addEventListener('click', handleTogglePassword);
        }

        // Form submit
        form.addEventListener('submit', handleFormSubmit);

        // Real-time validation
        if (nomeInput) {
            nomeInput.addEventListener('blur', () => validateNome(nomeInput.value));
            nomeInput.addEventListener('input', () => clearFieldError(nomeInput));
        }

        if (emailInput) {
            emailInput.addEventListener('blur', () => validateEmail(emailInput.value));
            emailInput.addEventListener('input', () => clearFieldError(emailInput));
        }

        if (documentoInput) {
            documentoInput.addEventListener('input', handleDocumentoInput);
            documentoInput.addEventListener('blur', () => validateDocumento(documentoInput.value));
        }

        if (passwordInput) {
            passwordInput.addEventListener('input', handlePasswordInput);
            passwordInput.addEventListener('blur', () => validatePassword(passwordInput.value));
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
    }

    /* Documento Mask (CPF/CNPJ)
       ======================================================================== */
    function handleDocumentoInput(e) {
        clearFieldError(documentoInput);

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

    /* Password Strength Indicator
       ======================================================================== */
    function handlePasswordInput(e) {
        clearFieldError(passwordInput);

        const password = e.target.value;
        const strength = calculatePasswordStrength(password);

        updateStrengthIndicator(strength);
    }

    function calculatePasswordStrength(password) {
        if (!password) {
            return { level: 'none', text: 'Digite sua senha' };
        }

        let score = 0;

        // Critérios de força
        if (password.length >= 6) score++;
        if (password.length >= 10) score++;
        if (/[a-z]/.test(password) && /[A-Z]/.test(password)) score++;
        if (/\d/.test(password)) score++;
        if (/[^a-zA-Z0-9]/.test(password)) score++;

        if (score <= 2) {
            return { level: 'weak', text: 'Senha fraca' };
        } else if (score <= 3) {
            return { level: 'medium', text: 'Senha média' };
        } else {
            return { level: 'strong', text: 'Senha forte' };
        }
    }

    function updateStrengthIndicator(strength) {
        if (!strengthFill || !strengthText) return;

        // Remove classes anteriores
        strengthFill.classList.remove('weak', 'medium', 'strong');
        strengthText.classList.remove('weak', 'medium', 'strong');

        if (strength.level !== 'none') {
            strengthFill.classList.add(strength.level);
            strengthText.classList.add(strength.level);
        }

        strengthText.textContent = strength.text;
    }

    /* Form Validation
       ======================================================================== */
    function handleFormSubmit(e) {
        clearAllErrors();

        const nome = nomeInput.value.trim();
        const email = emailInput.value.trim();
        const documento = documentoInput.value;
        const password = passwordInput.value;

        let isValid = true;

        // Validar nome
        if (!validateNome(nome)) {
            isValid = false;
        }

        // Validar email
        if (!validateEmail(email)) {
            isValid = false;
        }

        // Validar documento
        if (!validateDocumento(documento)) {
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

    function validateNome(nome) {
        clearFieldError(nomeInput);

        if (!nome) {
            showFieldError(nomeInput, 'O nome é obrigatório');
            return false;
        }

        if (nome.length < 3) {
            showFieldError(nomeInput, 'O nome deve ter no mínimo 3 caracteres');
            return false;
        }

        // Verifica se tem pelo menos um espaço (nome + sobrenome)
        if (!nome.includes(' ')) {
            showFieldError(nomeInput, 'Digite seu nome completo');
            return false;
        }

        nomeInput.classList.add('is-valid');
        return true;
    }

    function validateEmail(email) {
        clearFieldError(emailInput);

        if (!email) {
            showFieldError(emailInput, 'O email é obrigatório');
            return false;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showFieldError(emailInput, 'Digite um email válido');
            return false;
        }

        emailInput.classList.add('is-valid');
        return true;
    }

    function validateDocumento(documento) {
        clearFieldError(documentoInput);

        if (!documento) {
            showFieldError(documentoInput, 'O documento é obrigatório');
            return false;
        }

        // Remove formatação
        const cleaned = documento.replace(/\D/g, '');

        if (cleaned.length !== 11 && cleaned.length !== 14) {
            showFieldError(documentoInput, 'Digite um CPF (11 dígitos) ou CNPJ (14 dígitos) válido');
            return false;
        }

        // Validação básica de CPF
        if (cleaned.length === 11) {
            if (!isValidCPF(cleaned)) {
                showFieldError(documentoInput, 'CPF inválido');
                return false;
            }
        }

        // Validação básica de CNPJ
        if (cleaned.length === 14) {
            if (!isValidCNPJ(cleaned)) {
                showFieldError(documentoInput, 'CNPJ inválido');
                return false;
            }
        }

        documentoInput.classList.add('is-valid');
        return true;
    }

    function validatePassword(password) {
        clearFieldError(passwordInput);

        if (!password) {
            showFieldError(passwordInput, 'A senha é obrigatória');
            return false;
        }

        if (password.length < 6) {
            showFieldError(passwordInput, 'A senha deve ter no mínimo 6 caracteres');
            return false;
        }

        const strength = calculatePasswordStrength(password);
        if (strength.level === 'weak') {
            showFieldError(passwordInput, 'Use uma senha mais forte (combine letras, números e símbolos)');
            return false;
        }

        passwordInput.classList.add('is-valid');
        return true;
    }

    /* CPF Validation
       ======================================================================== */
    function isValidCPF(cpf) {
        // Elimina CPFs invalidos conhecidos
        if (cpf.length !== 11 || /^(\d)\1{10}$/.test(cpf)) {
            return false;
        }

        // Valida 1o digito
        let sum = 0;
        for (let i = 0; i < 9; i++) {
            sum += parseInt(cpf.charAt(i)) * (10 - i);
        }
        let rev = 11 - (sum % 11);
        if (rev === 10 || rev === 11) rev = 0;
        if (rev !== parseInt(cpf.charAt(9))) return false;

        // Valida 2o digito
        sum = 0;
        for (let i = 0; i < 10; i++) {
            sum += parseInt(cpf.charAt(i)) * (11 - i);
        }
        rev = 11 - (sum % 11);
        if (rev === 10 || rev === 11) rev = 0;
        if (rev !== parseInt(cpf.charAt(10))) return false;

        return true;
    }

    /* CNPJ Validation
       ======================================================================== */
    function isValidCNPJ(cnpj) {
        // Elimina CNPJs invalidos conhecidos
        if (cnpj.length !== 14 || /^(\d)\1{13}$/.test(cnpj)) {
            return false;
        }

        // Valida DVs
        let tamanho = cnpj.length - 2;
        let numeros = cnpj.substring(0, tamanho);
        let digitos = cnpj.substring(tamanho);
        let soma = 0;
        let pos = tamanho - 7;

        for (let i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2) pos = 9;
        }

        let resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
        if (resultado !== parseInt(digitos.charAt(0))) return false;

        tamanho = tamanho + 1;
        numeros = cnpj.substring(0, tamanho);
        soma = 0;
        pos = tamanho - 7;

        for (let i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2) pos = 9;
        }

        resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
        if (resultado !== parseInt(digitos.charAt(1))) return false;

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
            Criando conta...
        `;

        submitBtn.style.opacity = '0.7';
    }

    /* Floating Cards Animation
       ======================================================================== */
    function startFloatingCardsAnimation() {
        const cards = document.querySelectorAll('.floating-card');

        cards.forEach((card, index) => {
            card.style.animationDelay = `${index * 0.5}s`;

            // Parallax effect
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
        if (document.getElementById('create-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'create-animations';
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
                border-color: var(--create-success);
            }

            .form-input.is-valid:focus {
                box-shadow: 0 0 0 0.25rem rgba(16, 185, 129, 0.25);
            }

            .form-input.is-invalid {
                border-color: var(--create-danger);
            }

            .form-input.is-invalid:focus {
                box-shadow: 0 0 0 0.25rem rgba(239, 68, 68, 0.25);
            }

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
        if (e.key === 'Enter') {
            const focusedElement = document.activeElement;

            if (focusedElement === nomeInput) {
                e.preventDefault();
                emailInput.focus();
            } else if (focusedElement === emailInput) {
                e.preventDefault();
                documentoInput.focus();
            } else if (focusedElement === documentoInput) {
                e.preventDefault();
                passwordInput.focus();
            }
        }
    });

    /* Export para debugging (opcional)
       ======================================================================== */
    window.CreateModule = {
        validateNome,
        validateEmail,
        validateDocumento,
        validatePassword,
        clearAllErrors
    };

})();