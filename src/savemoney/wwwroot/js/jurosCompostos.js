document.addEventListener('DOMContentLoaded', function () {
    const btnCalcular = document.getElementById('btnCalcular');
    const periodoSlider = document.getElementById('periodoAnos');
    const periodoOutput = document.getElementById('periodoOutput');
    let chartInstance = null;

    // Atualiza o display do slider em tempo real
    periodoSlider.addEventListener('input', function () {
        periodoOutput.innerText = this.value;
    });

    const formatCurrency = (value) => {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
    };

    const calcular = () => {
        // 1. Pegar valores
        const inicial = parseFloat(document.getElementById('valorInicial').value) || 0;
        const aporte = parseFloat(document.getElementById('aporteMensal').value) || 0;
        const taxaAnual = parseFloat(document.getElementById('taxaJuros').value) || 0;
        const anos = parseInt(periodoSlider.value) || 1;

        // 2. Conversão de Taxa (Anual para Mensal)
        const taxaMensal = Math.pow(1 + (taxaAnual / 100), 1 / 12) - 1;
        const totalMeses = anos * 12;

        // 3. Arrays para o Gráfico
        const labels = [];
        const dadosInvestido = [];
        const dadosTotal = [];

        let saldo = inicial;
        let investido = inicial;

        // Ponto zero
        labels.push('Início');
        dadosInvestido.push(inicial);
        dadosTotal.push(inicial);

        // 4. Loop de Cálculo
        for (let i = 1; i <= totalMeses; i++) {
            // Aplica Juros
            saldo = saldo * (1 + taxaMensal);

            // Adiciona Aporte
            saldo += aporte;
            investido += aporte;

            // Grava pontos anuais para o gráfico não ficar pesado
            if (i % 12 === 0) {
                labels.push(`Ano ${i / 12}`);
                dadosInvestido.push(investido);
                dadosTotal.push(saldo);
            }
        }

        // 5. Atualizar UI
        const totalJuros = saldo - investido;

        document.getElementById('resTotalInvestido').innerText = formatCurrency(investido);
        document.getElementById('resTotalJuros').innerText = "+ " + formatCurrency(totalJuros);
        document.getElementById('resMontanteFinal').innerText = formatCurrency(saldo);
        document.getElementById('badgePeriodo').innerText = `Em ${anos} anos`;

        atualizarGrafico(labels, dadosInvestido, dadosTotal);
    };

    const atualizarGrafico = (labels, investido, total) => {
        const ctx = document.getElementById('chartJuros').getContext('2d');

        if (chartInstance) {
            chartInstance.destroy();
        }

        // Cria gradiente para o gráfico
        const gradient = ctx.createLinearGradient(0, 0, 0, 400);
        gradient.addColorStop(0, 'rgba(59, 130, 246, 0.5)');
        gradient.addColorStop(1, 'rgba(59, 130, 246, 0)');

        chartInstance = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Patrimônio Total',
                        data: total,
                        borderColor: '#3b82f6', // Azul Accent
                        backgroundColor: gradient,
                        borderWidth: 3,
                        pointBackgroundColor: '#3b82f6',
                        fill: true,
                        tension: 0.4
                    },
                    {
                        label: 'Dinheiro Investido',
                        data: investido,
                        borderColor: '#9ca3af', // Cinza
                        borderWidth: 2,
                        borderDash: [5, 5],
                        pointRadius: 0, // Esconde pontos
                        tension: 0.4
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: { color: '#e5e7eb' }
                    },
                    tooltip: {
                        mode: 'index',
                        intersect: false,
                        callbacks: {
                            label: function (context) {
                                return context.dataset.label + ': ' + formatCurrency(context.raw);
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        grid: { color: 'rgba(255,255,255,0.05)' },
                        ticks: {
                            color: '#9ca3af',
                            callback: (value) => 'R$ ' + (value >= 1000 ? value / 1000 + 'k' : value)
                        }
                    },
                    x: {
                        grid: { display: false },
                        ticks: { color: '#9ca3af' }
                    }
                },
                interaction: {
                    mode: 'nearest',
                    axis: 'x',
                    intersect: false
                }
            }
        });
    };

    // Listeners
    btnCalcular.addEventListener('click', calcular);

    // Cálculo inicial
    calcular();
});