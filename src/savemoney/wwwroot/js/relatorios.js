document.addEventListener('DOMContentLoaded', function () {
    // Verifica se os dados foram injetados corretamente na View
    const data = window.relatoriosData;
    if (!data) {
        console.warn('Dados de relatório não encontrados.');
        return;
    }

    // ==========================================
    // 1. Configurações Globais e Utilitários
    // ==========================================
    const locale = 'pt-BR';
    const currencyFormatter = new Intl.NumberFormat(locale, { style: 'currency', currency: 'BRL' });

    // Configuração Padrão do Chart.js para Dark Mode
    Chart.defaults.font.family = "'Inter', sans-serif";
    Chart.defaults.color = '#9ca3af'; // text-secondary
    Chart.defaults.borderColor = 'rgba(255, 255, 255, 0.05)';

    // Cores do Tema
    const colors = {
        success: '#10b981',
        danger: '#ef4444',
        info: '#3b82f6',
        warning: '#f59e0b',
        palette: ['#3b82f6', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#14b8a6', '#6366f1']
    };

    // ==========================================
    // 2. Animação de KPIs (Counter Up)
    // ==========================================
    const animateCounters = () => {
        const counters = document.querySelectorAll('.kpi-counter');
        const countersPercent = document.querySelectorAll('.kpi-counter-percent');

        // Função genérica de animação
        const runAnimation = (el, isPercent) => {
            const rawValue = el.getAttribute('data-target').replace(',', '.');
            const target = parseFloat(rawValue);

            if (isNaN(target)) return;

            const duration = 1500; // ms
            const startTimestamp = performance.now();

            const step = (timestamp) => {
                const progress = Math.min((timestamp - startTimestamp) / duration, 1);
                // Easing function: easeOutExpo
                const ease = progress === 1 ? 1 : 1 - Math.pow(2, -10 * progress);

                const currentVal = target * ease;

                if (isPercent) {
                    el.innerText = currentVal.toFixed(1).replace('.', ',') + '%';
                } else {
                    el.innerText = currencyFormatter.format(currentVal);
                }

                if (progress < 1) {
                    window.requestAnimationFrame(step);
                } else {
                    // Garante valor final exato
                    if (isPercent) el.innerText = target.toFixed(1).replace('.', ',') + '%';
                    else el.innerText = currencyFormatter.format(target);
                }
            };

            window.requestAnimationFrame(step);
        };

        counters.forEach(c => runAnimation(c, false));
        countersPercent.forEach(c => runAnimation(c, true));
    };

    animateCounters();

    // ==========================================
    // 3. Gráficos (Chart.js)
    // ==========================================

    // --- Gráfico de Fluxo (Barras) ---
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
                        backgroundColor: colors.success,
                        borderRadius: 4,
                        barPercentage: 0.6,
                    },
                    {
                        label: 'Despesas',
                        data: data.timeline.despesas,
                        backgroundColor: colors.danger,
                        borderRadius: 4,
                        barPercentage: 0.6,
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: { display: false }, // Legenda customizada no HTML
                    tooltip: {
                        backgroundColor: 'rgba(20, 22, 30, 0.9)',
                        titleColor: '#fff',
                        bodyColor: '#ccc',
                        borderColor: 'rgba(255,255,255,0.1)',
                        borderWidth: 1,
                        callbacks: {
                            label: function (context) {
                                return context.dataset.label + ': ' + currencyFormatter.format(context.raw);
                            }
                        }
                    }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: { ticks: { callback: (value) => 'R$ ' + value } }
                },
                // Drill-down: Filtra a tabela ao clicar na barra
                onClick: (e, elements) => {
                    if (elements.length > 0) {
                        const index = elements[0].index;
                        const label = data.timeline.labels[index];
                        filtrarTabelaPorData(label);
                    } else {
                        limparFiltroTabela();
                    }
                }
            }
        });
    }

    // --- Gráfico de Categorias (Doughnut) ---
    const ctxCat = document.getElementById('chartCategorias');
    if (ctxCat && data.categorias) {
        new Chart(ctxCat, {
            type: 'doughnut',
            data: {
                labels: data.categorias.labels,
                datasets: [{
                    data: data.categorias.valores,
                    backgroundColor: colors.palette,
                    borderWidth: 0,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: { position: 'right', labels: { boxWidth: 10, usePointStyle: true } },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return context.label + ': ' + currencyFormatter.format(context.raw);
                            }
                        }
                    }
                }
            }
        });
    }

    // --- Gráfico Dia da Semana (Barra Horizontal ou Radar) ---
    const ctxDia = document.getElementById('chartDiaSemana');
    if (ctxDia && data.diasSemana) {
        new Chart(ctxDia, {
            type: 'bar', // ou 'radar'
            data: {
                labels: data.diasSemana.labels,
                datasets: [{
                    label: 'Gasto Médio',
                    data: data.diasSemana.valores,
                    backgroundColor: 'rgba(59, 130, 246, 0.5)',
                    borderColor: colors.info,
                    borderWidth: 1,
                    borderRadius: 4
                }]
            },
            options: {
                indexAxis: 'y', // Barra horizontal
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: { x: { grid: { color: 'rgba(255,255,255,0.02)' } } }
            }
        });
    }

    // --- Gráfico de Projeção (Line Area) ---
    const ctxProjecao = document.getElementById('chartProjecao');
    if (ctxProjecao && data.projecao) {
        // Criar gradiente
        const gradient = ctxProjecao.getContext('2d').createLinearGradient(0, 0, 0, 400);
        gradient.addColorStop(0, 'rgba(16, 185, 129, 0.5)'); // Verde transparente
        gradient.addColorStop(1, 'rgba(16, 185, 129, 0)');

        new Chart(ctxProjecao, {
            type: 'line',
            data: {
                labels: data.projecao.meses,
                datasets: [{
                    label: 'Saldo Projetado',
                    data: data.projecao.saldos,
                    borderColor: colors.success,
                    backgroundColor: gradient,
                    fill: true,
                    tension: 0.4,
                    pointBackgroundColor: colors.success
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: { grid: { color: 'rgba(255,255,255,0.05)' } }
                }
            }
        });
    }

    // ==========================================
    // 4. Lógica de Filtros e Chips de Data
    // ==========================================
    const chips = document.querySelectorAll('.date-chip');
    const inputInicio = document.querySelector('input[name="dataInicio"]');
    const inputFim = document.querySelector('input[name="dataFim"]');
    const form = document.getElementById('filterForm');

    // Mapeamento de períodos
    chips.forEach(chip => {
        // Marca o chip ativo baseado nos inputs atuais (ao carregar)
        // ...logica simplificada para MVP...

        chip.addEventListener('click', function () {
            // Remove active de todos
            chips.forEach(c => c.classList.remove('active'));
            this.classList.add('active');

            const period = this.getAttribute('data-period');
            const today = new Date();
            let start = new Date();
            let end = new Date(); // Fim é hoje por padrão

            switch (period) {
                case 'today':
                    // Start = End = Hoje
                    break;
                case 'week':
                    // Últimos 7 dias ou Início da semana (domingo)
                    start.setDate(today.getDate() - 7);
                    break;
                case 'month':
                    // Dia 1 do mês atual
                    start.setDate(1);
                    break;
                case 'quarter':
                    // 3 meses atrás
                    start.setMonth(today.getMonth() - 3);
                    break;
                case 'year':
                    // 1 de Janeiro
                    start.setMonth(0);
                    start.setDate(1);
                    break;
            }

            // Formata para YYYY-MM-DD
            inputInicio.value = start.toISOString().split('T')[0];
            inputFim.value = end.toISOString().split('T')[0];

            // Submete o form automaticamente (opcional)
            // form.submit(); 
        });
    });

    // ==========================================
    // 5. Drill-down e Busca na Tabela
    // ==========================================
    const searchInput = document.getElementById('searchTransactions');
    const tableRows = document.querySelectorAll('#transactionsTable tbody tr');

    // Filtro por Data (vindo do clique no gráfico)
    function filtrarTabelaPorData(dataLabel) { // dataLabel ex: "01/12"
        // Formato simples DD/MM. O ideal é ter um atributo data-dateISO na TR
        tableRows.forEach(row => {
            const dateCell = row.querySelector('.date-badge').innerText; // Pega "01/12"
            // Comparação simples de string (cuidado com ano)
            if (dateCell.includes(dataLabel)) {
                row.classList.remove('hidden-row');
                row.style.animation = 'fadeInUp 0.3s forwards';
            } else {
                row.classList.add('hidden-row');
            }
        });
    }

    function limparFiltroTabela() {
        tableRows.forEach(row => row.classList.remove('hidden-row'));
    }

    // Busca por Texto (Input)
    if (searchInput) {
        searchInput.addEventListener('keyup', function (e) {
            const term = e.target.value.toLowerCase();
            tableRows.forEach(row => {
                const text = row.innerText.toLowerCase();
                if (text.includes(term)) {
                    row.classList.remove('hidden-row');
                } else {
                    row.classList.add('hidden-row');
                }
            });
        });
    }
});