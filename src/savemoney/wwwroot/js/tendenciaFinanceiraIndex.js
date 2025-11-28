/* ============================================================================
   TENDÊNCIA FINANCEIRA - INDEX
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let form = null;
    let selectMeses = null;
    let btnSubmit = null;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeTendenciaIndex);

    function initializeTendenciaIndex() {
        console.log('Tendência Financeira - Index loaded');

        // Cache de elementos
        form = document.getElementById('formAnalise');
        selectMeses = document.getElementById('meses');
        btnSubmit = form?.querySelector('button[type="submit"]');

        if (!form || !selectMeses) {
            console.error('Elementos do formulário não encontrados');
            return;
        }

        setupEventListeners();
        addAnimationStyles();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Change no select
        selectMeses.addEventListener('change', handleSelectChange);

        // Submit do formulário
        form.addEventListener('submit', handleFormSubmit);

        console.log('Event listeners configurados');
    }

    /* Select Change Handler
       ======================================================================== */
    function handleSelectChange(e) {
        const value = e.target.value;

        // Remove classes de validação
        selectMeses.classList.remove('is-invalid', 'is-valid');

        // Remove info/erro anterior
        clearMessages();

        if (!value || value === '' || value === '0') {
            return;
        }

        // Adiciona classe válida
        selectMeses.classList.add('is-valid');

        // Mostra informação sobre o período
        const meses = parseInt(value);
        if (!isNaN(meses)) {
            showPeriodInfo(meses);
        }
    }

    /* Form Submit Handler
       ======================================================================== */
    function handleFormSubmit(e) {
        // Valida formulário
        if (!validateForm()) {
            e.preventDefault();
            return false;
        }

        // Mostra loading no botão
        showLoadingState();
    }

    /* Form Validation
       ======================================================================== */
    function validateForm() {
        // Remove validações anteriores
        selectMeses.classList.remove('is-invalid', 'is-valid');
        clearMessages();

        const value = selectMeses.value;

        // Validar se um período foi selecionado
        if (!value || value === '' || value === '0') {
            selectMeses.classList.add('is-invalid');
            showError('Por favor, selecione um período de análise');
            selectMeses.focus();
            return false;
        }

        const meses = parseInt(value);

        // Validar se é número válido
        if (isNaN(meses)) {
            selectMeses.classList.add('is-invalid');
            showError('Período inválido');
            return false;
        }

        // Validar valor mínimo e máximo
        if (meses < 1 || meses > 12) {
            selectMeses.classList.add('is-invalid');
            showError('O período deve estar entre 1 e 12 meses');
            return false;
        }

        selectMeses.classList.add('is-valid');
        console.log('Formulário válido - Período:', meses, 'meses');
        return true;
    }

    /* Show Period Info
       ======================================================================== */
    function showPeriodInfo(meses) {
        clearMessages();

        const infoDiv = document.createElement('div');
        infoDiv.className = 'alert alert-info period-info-alert';
        infoDiv.style.animation = 'fadeInUp 0.3s ease-out';

        let texto = '';

        if (meses === 1) {
            texto = 'Análise de <strong>1 mês</strong> - Ideal para verificar mudanças recentes';
        } else if (meses <= 3) {
            texto = `Análise de <strong>${meses} meses</strong> - Período recomendado para identificar tendências`;
        } else if (meses <= 6) {
            texto = `Análise de <strong>${meses} meses</strong> - Ótimo para análises de médio prazo`;
        } else {
            texto = `Análise de <strong>${meses} meses</strong> - Ideal para identificar padrões anuais`;
        }

        infoDiv.innerHTML = `
            <span class="material-symbols-outlined alert-icon" aria-hidden="true">info</span>
            <span>${texto}</span>
        `;

        form.appendChild(infoDiv);
    }

    /* Show Error
       ======================================================================== */
    function showError(mensagem) {
        clearMessages();

        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger error-alert';
        alertDiv.innerHTML = `
            <span class="material-symbols-outlined alert-icon" aria-hidden="true">error</span>
            <span>${mensagem}</span>
        `;

        form.appendChild(alertDiv);

        // Animação shake
        alertDiv.classList.add('animate-shake');
        setTimeout(() => {
            alertDiv.classList.remove('animate-shake');
        }, 500);
    }

    /* Clear Messages
       ======================================================================== */
    function clearMessages() {
        const existingInfo = form?.querySelector('.period-info-alert');
        const existingError = form?.querySelector('.error-alert');

        if (existingInfo) {
            existingInfo.remove();
        }

        if (existingError) {
            existingError.remove();
        }
    }

    /* Loading State
       ======================================================================== */
    function showLoadingState() {
        if (!btnSubmit) return;

        btnSubmit.disabled = true;
        const originalText = btnSubmit.innerHTML;
        btnSubmit.dataset.originalText = originalText;

        btnSubmit.innerHTML = `
            <span class="material-symbols-outlined spinning" aria-hidden="true">sync</span>
            Analisando...
        `;
    }

    /* Add Animation Styles
       ======================================================================== */
    function addAnimationStyles() {
        if (document.getElementById('tendencia-index-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'tendencia-index-animations';
        style.textContent = `
            @keyframes animate-shake {
                0%, 100% { transform: translateX(0); }
                25% { transform: translateX(-0.625rem); }
                75% { transform: translateX(0.625rem); }
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(0.625rem);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }

            @keyframes spin-icon {
                from { transform: rotate(0deg); }
                to { transform: rotate(360deg); }
            }

            .animate-shake {
                animation: animate-shake 0.5s ease-in-out;
            }

            .spinning {
                display: inline-block;
                animation: spin-icon 1s linear infinite;
            }

            .form-input.is-valid {
                border-color: var(--success) !important;
            }

            .form-input.is-invalid {
                border-color: var(--danger) !important;
            }

            .form-input.is-valid:focus {
                box-shadow: 0 0 0 0.25rem rgba(16, 185, 129, 0.25);
            }

            .form-input.is-invalid:focus {
                box-shadow: 0 0 0 0.25rem rgba(239, 68, 68, 0.25);
            }
        `;
        document.head.appendChild(style);

        console.log('Animation styles adicionados');
    }

    /* Export para debugging (opcional)
       ======================================================================== */
    window.TendenciaFinanceiraIndex = {
        validateForm,
        showError,
        clearMessages
    };

})();