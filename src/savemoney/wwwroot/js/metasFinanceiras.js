/* ============================================================================
   METAS FINANCEIRAS
   ============================================================================ */

(() => {
    'use strict';

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeMetasFinanceiras);

    function initializeMetasFinanceiras() {
        console.log('Metas Financeiras module loaded');

        // Animações de entrada dos cards
        setupCardAnimations();

        // Auto-dismiss de alerts
        setupAlertAutoDismiss();

        // Formatação de inputs de valor
        setupCurrencyInputs();

        // Validação de formulários
        setupFormValidation();
    }

    /* Animações de Cards
       ======================================================================== */
    function setupCardAnimations() {
        const cards = document.querySelectorAll('.meta-card, .info-card, .aporte-card, .historico-card');

        if ('IntersectionObserver' in window && cards.length > 0) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((entry, index) => {
                    if (entry.isIntersecting) {
                        // Delay sequencial para cada card
                        setTimeout(() => {
                            entry.target.style.opacity = '1';
                            entry.target.style.transform = 'translateY(0)';
                        }, index * 100);

                        observer.unobserve(entry.target);
                    }
                });
            }, {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            });

            cards.forEach(card => {
                // Estado inicial
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';

                observer.observe(card);
            });
        }
    }

    /* Auto-Dismiss de Alerts
       ======================================================================== */
    function setupAlertAutoDismiss() {
        const alerts = document.querySelectorAll('.alert-success, .alert-info');

        alerts.forEach(alert => {
            // Auto-dismiss após 5 segundos
            setTimeout(() => {
                fadeOutAndRemove(alert);
            }, 5000);
        });
    }

    function fadeOutAndRemove(element) {
        if (!element) return;

        element.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
        element.style.opacity = '0';
        element.style.transform = 'translateY(-10px)';

        setTimeout(() => {
            element.remove();
        }, 300);
    }

    /* Formatação de Inputs de Moeda
       ======================================================================== */
    function setupCurrencyInputs() {
        const currencyInputs = document.querySelectorAll('input[type="number"][step="0.01"]');

        currencyInputs.forEach(input => {
            // Formata ao perder o foco
            input.addEventListener('blur', function () {
                if (this.value) {
                    const value = parseFloat(this.value);
                    if (!isNaN(value)) {
                        this.value = value.toFixed(2);
                    }
                }
            });

            // Previne valores negativos em campos de valor
            if (input.min === '0.01' || input.name === 'ValorAporte') {
                input.addEventListener('input', function () {
                    if (parseFloat(this.value) < 0) {
                        this.value = '';
                    }
                });
            }
        });
    }

    /* Validação de Formulários
       ======================================================================== */
    function setupFormValidation() {
        const forms = document.querySelectorAll('form');

        forms.forEach(form => {
            form.addEventListener('submit', function (e) {
                // Remove mensagens de erro anteriores
                clearFormErrors(form);

                // Valida campos obrigatórios
                const isValid = validateForm(form);

                if (!isValid) {
                    e.preventDefault();
                }
            });
        });
    }

    function validateForm(form) {
        let isValid = true;
        const requiredInputs = form.querySelectorAll('[required]');

        requiredInputs.forEach(input => {
            if (!input.value.trim()) {
                showFieldError(input, 'Este campo é obrigatório');
                isValid = false;
            }
        });

        // Validação específica para valores monetários
        const valorInputs = form.querySelectorAll('input[name="ValorObjetivo"], input[name="ValorAporte"]');
        valorInputs.forEach(input => {
            if (input.value) {
                const value = parseFloat(input.value);
                if (isNaN(value) || value <= 0) {
                    showFieldError(input, 'O valor deve ser maior que zero');
                    isValid = false;
                }
            }
        });

        // Validação de data limite (não pode ser no passado)
        const dataLimiteInput = form.querySelector('input[name="DataLimite"]');
        if (dataLimiteInput && dataLimiteInput.value) {
            const dataLimite = new Date(dataLimiteInput.value);
            const hoje = new Date();
            hoje.setHours(0, 0, 0, 0);

            if (dataLimite < hoje) {
                showFieldError(dataLimiteInput, 'A data limite não pode ser no passado');
                isValid = false;
            }
        }

        return isValid;
    }

    function showFieldError(input, message) {
        // Adiciona classe de erro no input
        input.classList.add('is-invalid');
        input.style.borderColor = 'var(--danger)';

        // Cria elemento de mensagem de erro se não existir
        let errorElement = input.parentElement.querySelector('.form-error');
        if (!errorElement) {
            errorElement = document.createElement('span');
            errorElement.className = 'form-error';
            input.parentElement.appendChild(errorElement);
        }
        errorElement.textContent = message;

        // Remove erro ao digitar
        input.addEventListener('input', function () {
            clearFieldError(this);
        }, { once: true });
    }

    function clearFieldError(input) {
        input.classList.remove('is-invalid');
        input.style.borderColor = '';

        const errorElement = input.parentElement.querySelector('.form-error');
        if (errorElement && !errorElement.hasAttribute('data-valmsg-for')) {
            errorElement.remove();
        }
    }

    function clearFormErrors(form) {
        const errorElements = form.querySelectorAll('.form-error:not([data-valmsg-for])');
        errorElements.forEach(el => el.remove());

        const invalidInputs = form.querySelectorAll('.is-invalid');
        invalidInputs.forEach(input => {
            input.classList.remove('is-invalid');
            input.style.borderColor = '';
        });
    }

    /* Utilitários - Formatação de Moeda
       ======================================================================== */
    function formatCurrency(value) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    }

    function parseCurrency(value) {
        if (!value) return 0;
        return parseFloat(
            value.replace(/[^0-9,-]+/g, '')
                .replace(/\./g, '')
                .replace(',', '.')
        ) || 0;
    }

    /* Utilitários - Smooth Scroll
       ======================================================================== */
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

            // Ignora links vazios
            if (href === '#' || href === '#!') return;

            e.preventDefault();

            const target = document.querySelector(href);
            if (target) {
                const headerOffset = 100;
                const elementPosition = target.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.scrollY - headerOffset;

                window.scrollTo({
                    top: offsetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });

    /* Feedback Visual - Loading em Botões de Submit
       ======================================================================== */
    const submitButtons = document.querySelectorAll('form button[type="submit"]');
    submitButtons.forEach(button => {
        const form = button.closest('form');
        if (!form) return;

        form.addEventListener('submit', function (e) {
            // Se a validação passar, mostra loading
            if (this.checkValidity()) {
                const originalText = button.innerHTML;
                button.disabled = true;
                button.innerHTML = '<span class="material-symbols-outlined spinning">sync</span> Processando...';

                // Adiciona animação de rotação
                const style = document.createElement('style');
                style.textContent = `
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

                // Se houver erro de servidor, restaura o botão
                setTimeout(() => {
                    if (button.disabled) {
                        button.disabled = false;
                        button.innerHTML = originalText;
                    }
                }, 10000);
            }
        });
    });

    /* Animação de Progresso - Atualização Suave
       ======================================================================== */
    function animateProgressBars() {
        const progressBars = document.querySelectorAll('.progress-bar-fill');

        progressBars.forEach(bar => {
            const targetWidth = bar.style.width;
            bar.style.width = '0%';

            // Anima após um pequeno delay
            setTimeout(() => {
                bar.style.width = targetWidth;
            }, 300);
        });
    }

    // Executa animação de progresso ao carregar
    if (document.querySelector('.progress-bar-fill')) {
        setTimeout(animateProgressBars, 100);
    }

    /* Confirmação de Exclusão
       ======================================================================== */
    const deleteForm = document.querySelector('form[action*="Delete"]');
    if (deleteForm) {
        deleteForm.addEventListener('submit', function (e) {
            const metaTitle = document.querySelector('.page-subtitle')?.textContent || 'esta meta';

            const confirmed = confirm(
                `Tem certeza que deseja remover "${metaTitle}"?\n\n` +
                'Todos os aportes também serão excluídos permanentemente.\n' +
                'Esta ação não pode ser desfeita.'
            );

            if (!confirmed) {
                e.preventDefault();
            }
        });
    }

    /* Acessibilidade - Anúncios de Progresso
       ======================================================================== */
    function announceProgress() {
        const progressPercentages = document.querySelectorAll('.progress-percentage');

        progressPercentages.forEach(element => {
            const percentage = element.textContent;
            element.setAttribute('aria-live', 'polite');
            element.setAttribute('aria-atomic', 'true');

            if (parseInt(percentage) === 100) {
                element.setAttribute('role', 'status');
            }
        });
    }

    announceProgress();

})();