/* ============================================================================
   TENDÊNCIA FINANCEIRA - INDEX
   100% Vanilla JavaScript
   Features: Validação, Feedback Visual, Preview Dinâmico
   ============================================================================ */

(() => {
    'use strict';

    // ============================================================================
    // ESTADO DA APLICAÇÃO
    // ============================================================================

    const state = {
        isSubmitting: false,
        selectedPeriod: null
    };

    // ============================================================================
    // ELEMENTOS DO DOM (Cached)
    // ============================================================================

    const elements = {
        form: null,
        selectMeses: null,
        btnSubmit: null
    };

    // ============================================================================
    // INICIALIZAÇÃO
    // ============================================================================

    document.addEventListener('DOMContentLoaded', initialize);

    function initialize() {
        console.log('📊 Tendência Financeira - Index Carregado');

        // Cache de elementos
        cacheElements();

        if (!elements.form || !elements.selectMeses) {
            console.error('❌ Elementos do formulário não encontrados');
            return;
        }

        // Configurar event listeners
        setupEventListeners();

        // Adicionar estilos de animação
        addAnimationStyles();

        // Animar cards de entrada
        animatePageElements();

        console.log('✅ Inicialização Completa');
    }

    // ============================================================================
    // CACHE DE ELEMENTOS
    // ============================================================================

    function cacheElements() {
        elements.form = document.getElementById('formAnalise');
        elements.selectMeses = document.getElementById('meses');
        elements.btnSubmit = elements.form?.querySelector('button[type="submit"]');
    }

    // ============================================================================
    // EVENT LISTENERS
    // ============================================================================

    function setupEventListeners() {
        // Change no select
        elements.selectMeses.addEventListener('change', handleSelectChange);

        // Submit do formulário
        elements.form.addEventListener('submit', handleFormSubmit);

        // Input no select (para acessibilidade)
        elements.selectMeses.addEventListener('input', handleSelectChange);

        console.log('🎧 Event listeners configurados');
    }

    // ============================================================================
    // SELECT CHANGE HANDLER
    // ============================================================================

    function handleSelectChange(e) {
        const value = e.target.value;

        // Remove classes de validação
        elements.selectMeses.classList.remove('is-invalid', 'is-valid');

        // Remove mensagens anteriores
        clearMessages();

        if (!value || value === '' || value === '0') {
            state.selectedPeriod = null;
            return;
        }

        // Adiciona classe válida
        elements.selectMeses.classList.add('is-valid');

        // Salva período selecionado
        const meses = parseInt(value);
        if (!isNaN(meses)) {
            state.selectedPeriod = meses;
            showPeriodInfo(meses);
            updatePreviewHighlight(meses);
        }
    }

    // ============================================================================
    // FORM SUBMIT HANDLER
    // ============================================================================

    function handleFormSubmit(e) {
        // Previne submit múltiplo
        if (state.isSubmitting) {
            e.preventDefault();
            return false;
        }

        // Valida formulário
        if (!validateForm()) {
            e.preventDefault();
            return false;
        }

        // Marca como submetendo
        state.isSubmitting = true;

        // Mostra loading no botão
        showLoadingState();

        // O formulário vai submeter normalmente (ASP.NET MVC)
        console.log('📤 Enviando formulário - Período:', state.selectedPeriod, 'meses');
    }

    // ============================================================================
    // VALIDAÇÃO DO FORMULÁRIO
    // ============================================================================

    function validateForm() {
        // Remove validações anteriores
        elements.selectMeses.classList.remove('is-invalid', 'is-valid');
        clearMessages();

        const value = elements.selectMeses.value;

        // Validar se um período foi selecionado
        if (!value || value === '' || value === '0') {
            elements.selectMeses.classList.add('is-invalid');
            showError('Por favor, selecione um período de análise');
            focusElement(elements.selectMeses);
            return false;
        }

        const meses = parseInt(value);

        // Validar se é número válido
        if (isNaN(meses)) {
            elements.selectMeses.classList.add('is-invalid');
            showError('Período inválido');
            return false;
        }

        // Validar valor mínimo e máximo
        if (meses < 1 || meses > 12) {
            elements.selectMeses.classList.add('is-invalid');
            showError('O período deve estar entre 1 e 12 meses');
            return false;
        }

        // Validação bem-sucedida
        elements.selectMeses.classList.add('is-valid');
        console.log('✅ Formulário válido - Período:', meses, 'meses');
        return true;
    }

    // ============================================================================
    // MOSTRAR INFORMAÇÃO DO PERÍODO
    // ============================================================================

    function showPeriodInfo(meses) {
        clearMessages();

        const infoDiv = document.createElement('div');
        infoDiv.className = 'alert alert-info period-info-alert';
        infoDiv.style.animation = 'fadeInUp 0.3s ease-out';

        const infoData = getPeriodInfoData(meses);

        infoDiv.innerHTML = `
            <span class="material-symbols-outlined alert-icon" aria-hidden="true">${infoData.icon}</span>
            <div class="alert-content">
                <strong>${infoData.title}</strong>
                <p style="margin: 0.25rem 0 0 0; font-size: 0.875rem;">${infoData.description}</p>
            </div>
        `;

        elements.form.appendChild(infoDiv);
    }

    function getPeriodInfoData(meses) {
        if (meses === 1) {
            return {
                icon: 'calendar_today',
                title: 'Análise de 1 mês',
                description: 'Ideal para verificar mudanças muito recentes nas suas finanças'
            };
        } else if (meses <= 3) {
            return {
                icon: 'calendar_month',
                title: `Análise de ${meses} meses (Recomendado)`,
                description: 'Período ideal para identificar tendências iniciais com boa precisão'
            };
        } else if (meses <= 6) {
            return {
                icon: 'date_range',
                title: `Análise de ${meses} meses`,
                description: 'Ótimo para análises de médio prazo e padrões sazonais'
            };
        } else {
            return {
                icon: 'event_note',
                title: `Análise de ${meses} meses`,
                description: 'Ideal para identificar padrões anuais e tendências de longo prazo'
            };
        }
    }

    // ============================================================================
    // ATUALIZAR PREVIEW HIGHLIGHT (NOVO)
    // ============================================================================

    function updatePreviewHighlight(meses) {
        // Remove highlight anterior
        const previewItems = document.querySelectorAll('.preview-item');
        previewItems.forEach(item => {
            item.classList.remove('preview-item-highlight');
        });

        // Adiciona highlight baseado no período
        let targetIndex = -1;

        if (meses >= 1 && meses <= 3) {
            targetIndex = 0; // Gráfico Interativo
        } else if (meses >= 4 && meses <= 6) {
            targetIndex = 1; // Identificação de Tendências
        } else if (meses >= 7 && meses <= 9) {
            targetIndex = 2; // Estatísticas Detalhadas
        } else if (meses >= 10) {
            targetIndex = 3; // Alertas Inteligentes
        }

        if (targetIndex >= 0 && previewItems[targetIndex]) {
            previewItems[targetIndex].classList.add('preview-item-highlight');
        }
    }

    // ============================================================================
    // MOSTRAR ERRO
    // ============================================================================

    function showError(mensagem) {
        clearMessages();

        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger error-alert';
        alertDiv.innerHTML = `
            <span class="material-symbols-outlined alert-icon" aria-hidden="true">error</span>
            <div class="alert-content">${mensagem}</div>
        `;

        elements.form.appendChild(alertDiv);

        // Animação shake
        alertDiv.classList.add('animate-shake');
        setTimeout(() => {
            alertDiv.classList.remove('animate-shake');
        }, 500);

        // Remove após 5 segundos
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.style.animation = 'fadeOut 0.3s ease-out';
                setTimeout(() => alertDiv.remove(), 300);
            }
        }, 5000);
    }

    // ============================================================================
    // LIMPAR MENSAGENS
    // ============================================================================

    function clearMessages() {
        const existingInfo = elements.form?.querySelector('.period-info-alert');
        const existingError = elements.form?.querySelector('.error-alert');

        if (existingInfo) {
            existingInfo.remove();
        }

        if (existingError) {
            existingError.remove();
        }
    }

    // ============================================================================
    // LOADING STATE
    // ============================================================================

    function showLoadingState() {
        if (!elements.btnSubmit) return;

        elements.btnSubmit.disabled = true;
        const originalText = elements.btnSubmit.innerHTML;
        elements.btnSubmit.dataset.originalText = originalText;

        elements.btnSubmit.innerHTML = `
            <span class="material-symbols-outlined spinning" aria-hidden="true">sync</span>
            <span class="btn-text">Analisando dados...</span>
        `;

        console.log('⏳ Loading state ativado');
    }

    // ============================================================================
    // ANIMAÇÕES DE PÁGINA
    // ============================================================================

    function animatePageElements() {
        const cards = document.querySelectorAll('.info-card, .form-card, .preview-card, .tips-card');

        cards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';

            setTimeout(() => {
                card.style.transition = 'opacity 0.6s cubic-bezier(0.16, 1, 0.3, 1), transform 0.6s cubic-bezier(0.16, 1, 0.3, 1)';
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, index * 100);
        });
    }

    // ============================================================================
    // ADICIONAR ESTILOS DE ANIMAÇÃO
    // ============================================================================

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

            @keyframes fadeOut {
                from {
                    opacity: 1;
                    transform: translateY(0);
                }
                to {
                    opacity: 0;
                    transform: translateY(-0.625rem);
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
                box-shadow: 0 0 0 0.25rem rgba(16, 185, 129, 0.25) !important;
                background: rgba(16, 185, 129, 0.05) !important;
            }

            .form-input.is-invalid:focus {
                box-shadow: 0 0 0 0.25rem rgba(239, 68, 68, 0.25) !important;
                background: rgba(239, 68, 68, 0.05) !important;
            }

            .preview-item-highlight {
                background: rgba(99, 102, 241, 0.1) !important;
                border-color: rgba(99, 102, 241, 0.3) !important;
                transform: translateY(-0.25rem) !important;
            }

            @media (prefers-reduced-motion: reduce) {
                .animate-shake,
                .spinning {
                    animation: none !important;
                }
            }
        `;
        document.head.appendChild(style);

        console.log('🎨 Animation styles adicionados');
    }

    // ============================================================================
    // UTILITÁRIOS
    // ============================================================================

    function focusElement(element) {
        if (!element) return;

        setTimeout(() => {
            element.focus();

            // Scroll suave até o elemento
            element.scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });
        }, 100);
    }

    // ============================================================================
    // EXPORT PARA DEBUGGING
    // ============================================================================

    window.TendenciaFinanceiraIndex = {
        state: () => state,
        validateForm,
        showError,
        clearMessages
    };

    console.log('🚀 Tendência Financeira Index - Pronto para uso');

})();