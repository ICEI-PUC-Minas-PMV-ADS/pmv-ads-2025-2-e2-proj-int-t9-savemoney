/* ============================================================================
   CALCULADORA DE METAS FINANCEIRAS
   ============================================================================ */

(() => {
    'use strict';

    // Elementos principais
    const calculateBtn = document.getElementById('calculateBtn');
    const resultsDiv = document.getElementById('resultsContent');
    const copyBtn = document.getElementById('copyLinkBtn');
    const copyFeedback = document.getElementById('copyFeedback');
    const goalValueInput = document.getElementById('goalValue');
    const initialInvestmentInput = document.getElementById('initialInvestment');
    const timeForGrowthInput = document.getElementById('timeForGrowth');
    const annualReturnRateInput = document.getElementById('annualReturnRate');
    const yearsRadio = document.getElementById('years');
    const monthsRadio = document.getElementById('months');

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeCalculadora);

    function initializeCalculadora() {
        console.log('Calculadora de Metas loaded');
        setupEventListeners();
        updateTimeUnitStyles();
        fillFormFromUrlParams();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        if (calculateBtn) {
            calculateBtn.addEventListener('click', handleCalculate);
        }

        if (copyBtn) {
            copyBtn.addEventListener('click', handleCopyLink);
        }

        const timeUnitRadios = document.querySelectorAll('input[name="timeUnit"]');
        timeUnitRadios.forEach(radio => {
            radio.addEventListener('change', updateTimeUnitStyles);
        });

        const accordionHeaders = document.querySelectorAll('.accordion-header');
        accordionHeaders.forEach(header => {
            header.addEventListener('click', () => toggleAccordion(header));
        });
    }

    /* Cálculos Financeiros
       ======================================================================== */
    function parseCurrency(value) {
        if (!value) return 0;
        const stringValue = String(value);
        const cleanedValue = stringValue
            .replace(/R\$\s?/g, '')
            .replace(/\./g, '')
            .replace(',', '.');
        return parseFloat(cleanedValue) || 0;
    }

    function calculateMonthlyContribution(goalValue, initialInvestment, months, annualRate) {
        if (months <= 0) return NaN;

        const monthlyRate = annualRate > 0.0001
            ? Math.pow(1 + (annualRate / 100), 1 / 12) - 1
            : 0;

        // Calcula valor futuro do investimento inicial
        let futureValueOfPV = initialInvestment;
        if (monthlyRate !== 0 && months > 0) {
            futureValueOfPV = initialInvestment * Math.pow(1 + monthlyRate, months);
        }

        const futureValueOfContributionsNeeded = goalValue - futureValueOfPV;

        if (futureValueOfContributionsNeeded <= 0) {
            return 0;
        }

        // Calcula contribuição mensal necessária
        let monthlyContribution = 0;
        if (monthlyRate === 0) {
            monthlyContribution = futureValueOfContributionsNeeded / months;
        } else {
            const factor = Math.pow(1 + monthlyRate, months) - 1;
            if (factor === 0) return NaN;
            monthlyContribution = (futureValueOfContributionsNeeded * monthlyRate) / factor;
        }

        return isNaN(monthlyContribution) ? NaN : monthlyContribution;
    }

    /* Handlers
       ======================================================================== */
    function handleCalculate() {
        const goalValue = parseCurrency(goalValueInput.value);
        const initialInvestment = parseCurrency(initialInvestmentInput.value);
        const timeForGrowth = parseInt(timeForGrowthInput.value) || 0;
        const isYears = yearsRadio ? yearsRadio.checked : true;
        const annualReturnRate = parseFloat(annualReturnRateInput.value.replace(',', '.'));

        // Validações
        if (goalValue <= 0) {
            displayError('O valor do objetivo deve ser maior que zero.');
            return;
        }
        if (initialInvestment < 0) {
            displayError('O investimento inicial não pode ser negativo.');
            return;
        }
        if (timeForGrowth <= 0) {
            displayError('O tempo para atingir a meta deve ser maior que zero.');
            return;
        }
        if (annualReturnRate < 0) {
            displayError('A taxa de retorno não pode ser negativa.');
            return;
        }

        const months = isYears ? timeForGrowth * 12 : timeForGrowth;
        const monthlyContribution = calculateMonthlyContribution(
            goalValue,
            initialInvestment,
            months,
            annualReturnRate
        );

        if (isNaN(monthlyContribution)) {
            displayError('Não foi possível calcular. Verifique se os valores inseridos são realistas.');
            return;
        }

        displayResult(
            monthlyContribution,
            goalValue,
            initialInvestment,
            timeForGrowth,
            isYears,
            annualReturnRate
        );

        updateCopyLink(
            goalValueInput.value,
            initialInvestmentInput.value,
            timeForGrowthInput.value,
            isYears ? 'y' : 'm',
            annualReturnRateInput.value
        );
    }

    function handleCopyLink() {
        const urlToCopy = copyBtn.dataset.shareUrl;
        if (!urlToCopy) return;

        navigator.clipboard.writeText(urlToCopy)
            .then(() => {
                if (copyFeedback) {
                    copyFeedback.style.display = 'block';
                    setTimeout(() => {
                        copyFeedback.style.display = 'none';
                    }, 2000);
                }
            })
            .catch(err => {
                console.error('Erro ao copiar com Clipboard API:', err);
                copyUsingExecCommand(urlToCopy);
            });
    }

    /* Display de Resultados
       ======================================================================== */
    function displayResult(monthlyContribution, goalValue, initialInvestment, timeValue, isYears, rate) {
        if (!resultsDiv) return;

        resultsDiv.classList.remove('results-placeholder');
        resultsDiv.classList.add('calculated-result');
        resultsDiv.style.textAlign = 'left';

        const formatter = new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        });

        const timeUnit = isYears
            ? (timeValue === 1 ? 'Ano' : 'Anos')
            : (timeValue === 1 ? 'Mês' : 'Meses');

        let resultHTML = `
            <h4>Resultado da Simulação</h4>
            <p><small>Para atingir <strong>${formatter.format(goalValue)}</strong></small></p>
            <p><small>Começando com ${formatter.format(initialInvestment)}</small></p>
            <p><small>Em ${timeValue} ${timeUnit}</small></p>
            <p><small>Com taxa de ${rate.toFixed(2).replace('.', ',')}% ao ano</small></p>
            <hr style="border-color: var(--border-color);">
            <h5>Você precisará poupar mensalmente:</h5>
        `;

        if (goalValue <= initialInvestment) {
            resultHTML += `<h3 style="color: var(--accent-primary);">${formatter.format(0)}</h3>`;
            resultHTML += `<p class="small">Seu investimento inicial já atinge ou supera a meta!</p>`;
        } else if (monthlyContribution <= 0) {
            resultHTML += `<h3 style="color: var(--accent-primary);">${formatter.format(0)}</h3>`;
            resultHTML += `<p class="small">Seu investimento inicial renderá o suficiente para atingir a meta, sem aportes!</p>`;
        } else {
            resultHTML += `<h3 style="color: var(--accent-primary);">${formatter.format(monthlyContribution)}</h3>`;
        }

        resultHTML += `<p class="small" style="color: var(--text-secondary);">Nota: Simulação simplificada sem taxas, impostos ou inflação.</p>`;

        resultsDiv.innerHTML = resultHTML;
    }

    function displayError(message) {
        if (!resultsDiv) return;

        resultsDiv.classList.remove('calculated-result');
        resultsDiv.classList.add('results-placeholder');
        resultsDiv.style.textAlign = 'center';

        resultsDiv.innerHTML = `
            <i class="bi bi-exclamation-triangle results-icon" style="color: #F87171;"></i>
            <h4 style="color: #F87171;">Erro na Simulação</h4>
            <p>${message}</p>
        `;

        if (copyBtn) {
            copyBtn.style.display = 'none';
        }
    }

    /* Compartilhamento
       ======================================================================== */
    function updateCopyLink(goal, initial, time, unit, rate) {
        if (!copyBtn) return;

        const params = new URLSearchParams({
            goal: goal,
            initial: initial,
            time: time,
            unit: unit,
            rate: rate
        });

        const shareUrl = window.location.origin + window.location.pathname + '?' + params.toString();
        copyBtn.dataset.shareUrl = shareUrl;
        copyBtn.style.display = 'block';

        if (copyFeedback) {
            copyFeedback.style.display = 'none';
        }
    }

    function copyUsingExecCommand(urlToCopy) {
        const tempInput = document.createElement('textarea');
        tempInput.value = urlToCopy;
        document.body.appendChild(tempInput);
        tempInput.select();

        try {
            const successful = document.execCommand('copy');
            if (successful && copyFeedback) {
                copyFeedback.style.display = 'block';
                setTimeout(() => {
                    copyFeedback.style.display = 'none';
                }, 2000);
            } else if (!successful) {
                console.error('Fallback execCommand falhou ao copiar.');
            }
        } catch (err) {
            console.error('Erro no fallback execCommand:', err);
        } finally {
            document.body.removeChild(tempInput);
        }
    }

    /* URL Parameters
       ======================================================================== */
    function fillFormFromUrlParams() {
        const urlParams = new URLSearchParams(window.location.search);

        if (urlParams.has('goal') && goalValueInput) {
            goalValueInput.value = decodeURIComponent(urlParams.get('goal'));
        }
        if (urlParams.has('initial') && initialInvestmentInput) {
            initialInvestmentInput.value = decodeURIComponent(urlParams.get('initial'));
        }
        if (urlParams.has('time') && timeForGrowthInput) {
            timeForGrowthInput.value = decodeURIComponent(urlParams.get('time'));
        }
        if (urlParams.has('unit')) {
            const unit = decodeURIComponent(urlParams.get('unit'));
            if (unit === 'm' && monthsRadio) {
                monthsRadio.checked = true;
            } else if (yearsRadio) {
                yearsRadio.checked = true;
            }
            updateTimeUnitStyles();
        }
        if (urlParams.has('rate') && annualReturnRateInput) {
            annualReturnRateInput.value = decodeURIComponent(urlParams.get('rate'));
        }

        // Auto-calcula se todos os parâmetros estão presentes
        const hasAllParams = urlParams.has('goal') &&
            urlParams.has('initial') &&
            urlParams.has('time') &&
            urlParams.has('unit') &&
            urlParams.has('rate');

        if (hasAllParams && calculateBtn) {
            setTimeout(() => calculateBtn.click(), 100);
        }
    }

    /* UI Helpers
       ======================================================================== */
    function updateTimeUnitStyles() {
        const monthsLabel = document.querySelector('label[for="months"]');
        const yearsLabel = document.querySelector('label[for="years"]');

        if (!monthsLabel || !yearsLabel || !monthsRadio) return;

        if (monthsRadio.checked) {
            monthsLabel.classList.add('btn-toggle-active');
            yearsLabel.classList.remove('btn-toggle-active');
        } else {
            yearsLabel.classList.add('btn-toggle-active');
            monthsLabel.classList.remove('btn-toggle-active');
        }
    }

    function toggleAccordion(header) {
        const content = header.nextElementSibling;
        const isExpanded = header.getAttribute('aria-expanded') === 'true';

        if (isExpanded) {
            header.setAttribute('aria-expanded', 'false');
            content.setAttribute('aria-hidden', 'true');
            content.style.maxHeight = null;
            content.style.paddingBottom = null;
        } else {
            header.setAttribute('aria-expanded', 'true');
            content.setAttribute('aria-hidden', 'false');
            content.style.maxHeight = content.scrollHeight + 'px';
            content.style.paddingBottom = '1.5rem';
        }
    }

})();