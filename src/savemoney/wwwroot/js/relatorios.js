document.addEventListener('DOMContentLoaded', function () {
    const data = window.relatoriosData;

    // Configuração Global Chart.js (Fonte e Cores)
    Chart.defaults.font.family = "'Inter', sans-serif";
    Chart.defaults.color = '#aaaaaa';
    Chart.defaults.scale.grid.color = 'rgba(255, 255, 255, 0.05)';

    // === 1. Gráfico de Fluxo (Barra Dupla) ===
    const ctxFluxo = document.getElementById('chartFluxo');
    if (ctxFluxo && data.timeline) {
        new Chart(ctxFluxo, {
            type: 'bar',
            data: {
                labels: data.timeline.labels,
                datasets: [
                    {
                        label: 'Receitas',
                        data: data.timeline.receitas,
                        backgroundColor: '#10b981', // var(--success)
                        borderRadius: 4,
                        barPercentage: 0.6,
                        categoryPercentage: 0.8
                    },
                    {
                        label: 'Despesas',
                        data: data.timeline.despesas,
                        backgroundColor: '#ef4444', // var(--danger)
                        borderRadius: 4,
                        barPercentage: 0.6,
                        categoryPercentage: 0.8
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                        align: 'end',
                        labels: { usePointStyle: true, boxWidth: 8 }
                    },
                    tooltip: {
                        mode: 'index',
                        intersect: false,
                        backgroundColor: 'rgba(16, 17, 26, 0.95)',
                        titleColor: '#fff',
                        bodyColor: '#ccc',
                        borderColor: 'rgba(255,255,255,0.1)',
                        borderWidth: 1,
                        padding: 10,
                        callbacks: {
                            label: function (context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    label += new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(context.parsed.y);
                                }
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return 'R$ ' + value; // Simplificado para economizar espaço
                            }
                        }
                    },
                    x: {
                        grid: { display: false }
                    }
                },
                interaction: {
                    mode: 'index',
                    intersect: false,
                },
            }
        });
    }

    // === 2. Gráfico de Categorias (Doughnut) ===
    const ctxCat = document.getElementById('chartCategorias');
    if (ctxCat && data.categorias && data.categorias.valores.length > 0) {
        new Chart(ctxCat, {
            type: 'doughnut',
            data: {
                labels: data.categorias.labels,
                datasets: [{
                    data: data.categorias.valores,
                    backgroundColor: [
                        '#3b82f6', // Azul
                        '#8b5cf6', // Roxo
                        '#ec4899', // Rosa
                        '#f59e0b', // Laranja
                        '#10b981', // Verde
                        '#6366f1', // Indigo
                        '#14b8a6', // Teal
                    ],
                    borderWidth: 0,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '75%', // Rosca mais fina
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            boxWidth: 12,
                            padding: 15,
                            color: '#aaaaaa'
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(16, 17, 26, 0.95)',
                        borderColor: 'rgba(255,255,255,0.1)',
                        borderWidth: 1,
                        callbacks: {
                            label: function (context) {
                                let val = context.parsed;
                                return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(val);
                            }
                        }
                    }
                }
            }
        });
    } else if (ctxCat) {
        // Fallback visual se não houver dados
        ctxCat.parentElement.innerHTML = '<div class="d-flex align-items-center justify-content-center h-100 text-secondary small">Sem dados de despesas</div>';
    }
});