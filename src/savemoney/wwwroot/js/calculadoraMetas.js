document.addEventListener('DOMContentLoaded', () => {
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

    function parseCurrency(value) {
        if (typeof value !== 'string' || !value) return 0;
        const cleanedValue = value.replace(/R\$\s?/g, '').replace(/\./g, '').replace(',', '.');
        return parseFloat(cleanedValue) || 0;
    }

    function calculateMonthlyContribution(goalValue, initialInvestment, months, annualRate) {
        if (months <= 0) return NaN;

        const monthlyRate = annualRate > 0.0001 ? Math.pow(1 + (annualRate / 100), 1 / 12) - 1 : 0;

        let futureValueOfPV = initialInvestment;
        if (monthlyRate !== 0 && months > 0) {
            futureValueOfPV = initialInvestment * Math.pow(1 + monthlyRate, months);
        } else if (months === 0) {
            futureValueOfPV = initialInvestment;
        }

        const futureValueOfContributionsNeeded = goalValue - futureValueOfPV;

        if (futureValueOfContributionsNeeded <= 0) {
            return 0;
        }

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

    function displayResult(monthlyContribution, goalValue, initialInvestment, timeValue, isYears, rate) {
        if (!resultsDiv) return;
        resultsDiv.classList.remove('results-placeholder');
        resultsDiv.classList.add('calculated-result');
        resultsDiv.style.textAlign = 'left';

        const formatter = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

        let resultHTML = `
             <h4 class="mb-3">Resultado da Simulação</h4>
             <p><small>Para atingir <strong>${formatter.format(goalValue)}</strong></small></p>
             <p><small>Começando com ${formatter.format(initialInvestment)}</small></p>
             <p><small>Em ${timeValue} ${isYears ? (timeValue === 1 ? 'Ano' : 'Anos') : (timeValue === 1 ? 'Mês' : 'Meses')}</small></p>
             <p><small>Com taxa de ${rate.toFixed(1).replace('.', ',')}% ao ano</small></p>
             <hr>
             <h5 class="mt-3">Você precisará poupar mensalmente:</h5>
        `;

        if (goalValue <= initialInvestment) {
            resultHTML += `<h3 class="text-success">${formatter.format(0)}</h3>`;
            resultHTML += `<p class="text-info small mt-2">Seu investimento inicial já atinge ou supera a meta!</p>`;
        } else if (monthlyContribution <= 0) {
            resultHTML += `<h3 class="text-success">${formatter.format(0)}</h3>`;
            resultHTML += `<p class="text-info small mt-2">Seu investimento inicial renderá o suficiente para atingir a meta, sem aportes!</p>`;
        } else {
            resultHTML += `<h3 class="text-primary">${formatter.format(monthlyContribution)}</h3>`;
        }

        resultHTML += `<p class="text-muted small mt-3">Nota: Simulação simplificada sem taxas, impostos ou inflação.</p>`;

        resultsDiv.innerHTML = resultHTML;
    }

    function displayError(message) {
        if (!resultsDiv) return;
        resultsDiv.classList.remove('calculated-result');
        resultsDiv.classList.add('results-placeholder');
        resultsDiv.style.textAlign = 'center';
        resultsDiv.innerHTML = `
             <i class="bi bi-exclamation-triangle display-4 text-danger mb-3"></i>
             <h5 class="text-danger">Erro na Simulação</h5>
             <p class="mb-0">${message}</p>
         `;
        if(copyBtn) copyBtn.classList.add('d-none');
    }

    function updateCopyLink(goal, initial, time, unit, rate) {
        if (!copyBtn) return;
        const goalRaw = goalValueInput.value;
        const initialRaw = initialInvestmentInput.value;
        const timeRaw = timeForGrowthInput.value;
        const rateRaw = annualReturnRateInput.value;

        const params = new URLSearchParams({
            goal: goalRaw,
            initial: initialRaw,
            time: timeRaw,
            unit: unit,
            rate: rateRaw
         });
        const shareUrl = `${window.location.origin}${window.location.pathname}?${params.toString()}`;
        copyBtn.dataset.shareUrl = shareUrl;
        copyBtn.classList.remove('d-none');
        if(copyFeedback) copyFeedback.classList.add('d-none');
    }

    if (calculateBtn) {
        calculateBtn.addEventListener('click', function() {
            const goalValue = parseCurrency(goalValueInput.value);
            const initialInvestment = parseCurrency(initialInvestmentInput.value);
            const timeForGrowth = parseInt(timeForGrowthInput.value) || 0;
            const isYears = yearsRadio ? yearsRadio.checked : true;
            const annualReturnRate = parseCurrency(annualReturnRateInput.value);

            if (goalValue <= 0) {
                displayError("O valor do objetivo deve ser maior que zero.");
                return;
            }
            if (initialInvestment < 0) {
                displayError("O investimento inicial não pode ser negativo.");
                return;
            }
            if (timeForGrowth <= 0) {
                displayError("O tempo para atingir a meta deve ser maior que zero.");
                return;
            }
            if (annualReturnRate < 0) {
                displayError("A taxa de retorno não pode ser negativa para calcular a contribuição necessária.");
                return;
            }

            const months = isYears ? timeForGrowth * 12 : timeForGrowth;
            const monthlyContribution = calculateMonthlyContribution(goalValue, initialInvestment, months, annualReturnRate);

            if (isNaN(monthlyContribution)) {
                displayError("Não foi possível calcular. Verifique se os valores inseridos (especialmente a taxa) são realistas.");
                return;
            }

            displayResult(monthlyContribution, goalValue, initialInvestment, timeForGrowth, isYears, annualReturnRate);
            updateCopyLink(goalValueInput.value, initialInvestmentInput.value, timeForGrowthInput.value, isYears ? 'y' : 'm', annualReturnRateInput.value);
        });
    }

    if (copyBtn) {
        copyBtn.addEventListener('click', () => {
            const urlToCopy = copyBtn.dataset.shareUrl;
            if (urlToCopy) {
                const tempInput = document.createElement('textarea');
                tempInput.value = urlToCopy;
                document.body.appendChild(tempInput);
                tempInput.select();
                try {
                    if (navigator.clipboard && navigator.clipboard.writeText) {
                        navigator.clipboard.writeText(urlToCopy).then(() => {
                            if(copyFeedback) {
                                copyFeedback.classList.remove('d-none');
                                setTimeout(() => copyFeedback.classList.add('d-none'), 2000);
                            }
                        }).catch(err => {
                            console.error('Erro ao copiar com Clipboard API:', err);
                            copyUsingExecCommand(urlToCopy);
                        });
                    } else {
                        copyUsingExecCommand(urlToCopy);
                    }
                } catch (err) {
                    console.error('Erro ao copiar link (execCommand):', err);
                } finally {
                    document.body.removeChild(tempInput);
                }
            }
        });
    }

    function copyUsingExecCommand(urlToCopy) {
        try {
            const successful = document.execCommand('copy');
            if (successful && copyFeedback) {
                copyFeedback.classList.remove('d-none');
                setTimeout(() => copyFeedback.classList.add('d-none'), 2000);
            } else if (!successful) {
                console.error('Fallback execCommand falhou ao copiar.');
            }
        } catch(err) {
            console.error('Erro no fallback execCommand:', err);
        }
    }

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
            } else if (yearsRadio){
                 yearsRadio.checked = true;
            }
            updateTimeUnitStyles();
       }
       if (urlParams.has('rate') && annualReturnRateInput) {
            annualReturnRateInput.value = decodeURIComponent(urlParams.get('rate'));
       }

       if (urlParams.has('goal') && urlParams.has('initial') && urlParams.has('time') && urlParams.has('unit') && urlParams.has('rate')) {
            if(calculateBtn) {
                 setTimeout(() => calculateBtn.click(), 100);
            }
       }
    }

    const timeUnitRadios = document.querySelectorAll('input[name="timeUnit"]');
    const timeUnitLabels = document.querySelectorAll('label[for^="months"], label[for^="years"]');

    timeUnitRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            updateTimeUnitStyles();
        });
    });

    function updateTimeUnitStyles() {
        const checkedRadio = document.querySelector('input[name="timeUnit"]:checked');
        if (!checkedRadio) return;

        timeUnitLabels.forEach(label => {
            label.classList.remove('btn-primary', 'active', 'btn-outline-primary');
            label.classList.add('btn-outline-secondary');
        });

        const activeLabel = document.querySelector(`label[for="${checkedRadio.id}"]`);
        if (activeLabel) {
            activeLabel.classList.remove('btn-outline-secondary');
            activeLabel.classList.add('btn-primary', 'active');
        }
    }

    updateTimeUnitStyles();
    fillFormFromUrlParams();
});

