document.addEventListener('DOMContentLoaded', () => {
    const calculateBtn = document.getElementById('calculateBtn');
    const resultsDiv = document.getElementById('resultsContent');

    if (calculateBtn) {
        calculateBtn.addEventListener('click', function() {
            // Obter valores dos inputs
            const initialInvestment = parseFloat(document.getElementById('initialInvestment').value.replace(',', '.')) || 0; // Trata vírgula
            let timeForGrowth = parseInt(document.getElementById('timeForGrowth').value) || 0;
            const isYears = document.getElementById('years').checked;
            const annualReturnRate = parseFloat(document.getElementById('annualReturnRate').value.replace(',', '.')) || 0; // Trata vírgula
            const financialGoal = document.getElementById('financialGoal').value || "Sua Meta";

            // --- LÓGICA DE CÁLCULO (Exemplo Simples - Juros Compostos Anuais) ---
            const timeInYears = isYears ? timeForGrowth : timeForGrowth / 12;

            const rateDecimal = annualReturnRate / 100;

            const finalAmount = initialInvestment * Math.pow((1 + rateDecimal), timeInYears);

            if (resultsDiv) {
                resultsDiv.classList.remove('results-placeholder'); 
                resultsDiv.style.textAlign = 'left'; 

               
                const formatter = new Intl.NumberFormat('pt-BR', {
                    style: 'currency',
                    currency: 'BRL',
                });

                resultsDiv.innerHTML = `
                    <h4 class="mb-3">Projeção para "${financialGoal}"</h4>
                    <p><strong>Investimento Inicial:</strong> ${formatter.format(initialInvestment)}</p>
                    <p><strong>Tempo:</strong> ${timeForGrowth} ${isYears ? (timeForGrowth === 1 ? 'Ano' : 'Anos') : (timeForGrowth === 1 ? 'Mês' : 'Meses')}</p>
                    <p><strong>Taxa de Retorno Anual:</strong> ${annualReturnRate.toFixed(1).replace('.', ',')}%</p>
                    <hr>
                    <h5 class="mt-3">Montante Final Estimado:</h5>
                    <h3 class="text-success">${formatter.format(finalAmount)}</h3>
                    <p class="text-muted small mt-3">Nota: Esta é uma simulação simplificada e não considera taxas, impostos ou aportes adicionais.</p>
                `;
            }
        });
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
            const yearsLabel = document.querySelector('label[for="years"]');
             if (checkedRadio.id !== 'years' && yearsLabel) {
                 yearsLabel.classList.remove('btn-primary', 'active');
                 yearsLabel.classList.add('btn-outline-primary'); 
            } else if (checkedRadio.id === 'years' && yearsLabel) {
                 yearsLabel.classList.remove('btn-outline-secondary'); 
                 yearsLabel.classList.add('btn-primary', 'active');
            }
             const monthsLabel = document.querySelector('label[for="months"]');
             if(checkedRadio.id !== 'months' && monthsLabel){
                  monthsLabel.classList.remove('btn-primary', 'active');
                  monthsLabel.classList.add('btn-outline-secondary');
             }
        }
    }

    
    updateTimeUnitStyles();
});
