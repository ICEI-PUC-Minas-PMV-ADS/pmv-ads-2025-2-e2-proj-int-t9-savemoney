/* ============================================================================
   TENDÊNCIA FINANCEIRA - RESULTADO
   100% Vanilla JavaScript - Zero Dependências
   Features: Gráfico Canvas, Tooltip, Toggle View, Export CSV
   ============================================================================ */

(() => {
    'use strict';

    // ============================================================================
    // ESTADO DA APLICAÇÃO
    // ============================================================================

    const state = {
        chartInstance: null,
        viewMode: 'line', // 'line' ou 'area'
        hoveredPoint: null,
        isAnimating: false
    };

    // ============================================================================
    // ELEMENTOS DO DOM (Cached)
    // ============================================================================

    const elements = {
        canvas: null,
        tooltip: null,
        btnExportar: null,
        toggleBtns: null
    };

    // ============================================================================
    // INICIALIZAÇÃO
    // ============================================================================

    document.addEventListener('DOMContentLoaded', initialize);

    function initialize() {
        console.log('📈 Tendência Financeira - Resultado Carregado');

        // Cache de elementos
        cacheElements();

        // Inicializar componentes
        initChart();
        initViewToggle();
        initExport();
        initAnimations();
        setupTableInteractions();

        console.log('✅ Inicialização Completa');
    }

    // ============================================================================
    // CACHE DE ELEMENTOS
    // ============================================================================

    function cacheElements() {
        elements.canvas = document.getElementById('graficoTendencia');
        elements.tooltip = document.getElementById('chartTooltip');
        elements.btnExportar = document.getElementById('btnExportar');
        elements.toggleBtns = document.querySelectorAll('.toggle-btn');
    }

    // ============================================================================
    // GRÁFICO CANVAS NATIVO
    // ============================================================================

    function initChart() {
        if (!elements.canvas) {
            console.warn('⚠️ Canvas não encontrado');
            return;
        }

        // Buscar dados dos data attributes
        const labelsJson = elements.canvas.getAttribute('data-labels');
        const receitasJson = elements.canvas.getAttribute('data-receitas');
        const despesasJson = elements.canvas.getAttribute('data-despesas');
        const saldosJson = elements.canvas.getAttribute('data-saldos');

        if (!labelsJson || !receitasJson || !despesasJson || !saldosJson) {
            console.error('❌ Dados do gráfico não encontrados');
            return;
        }

        try {
            const dados = {
                labels: JSON.parse(labelsJson),
                receitas: JSON.parse(receitasJson),
                despesas: JSON.parse(despesasJson),
                saldos: JSON.parse(saldosJson)
            };

            // Validar dados
            if (!dados.labels.length) {
                console.error('❌ Dados do gráfico estão vazios');
                return;
            }

            if (dados.labels.length !== dados.receitas.length ||
                dados.labels.length !== dados.despesas.length ||
                dados.labels.length !== dados.saldos.length) {
                console.error('❌ Dados do gráfico com tamanhos incompatíveis');
                return;
            }

            console.log('📊 Dados do gráfico carregados:', dados);
            createChart(dados);

        } catch (error) {
            console.error('❌ Erro ao parsear dados do gráfico:', error);
        }
    }

    function createChart(dados) {
        const ctx = elements.canvas.getContext('2d');
        const container = elements.canvas.parentElement;

        // Configurações do gráfico
        const config = {
            colors: {
                receitas: '#10b981',
                despesas: '#ef4444',
                saldo: '#3b82f6',
                grid: 'rgba(255, 255, 255, 0.05)',
                text: '#aaaaaa',
                hover: '#6366f1'
            },
            padding: {
                top: 60,
                right: 40,
                bottom: 70,
                left: 80
            },
            lineWidth: 3,
            pointRadius: 6,
            hoverPointRadius: 8
        };

        // Cria instância do gráfico
        state.chartInstance = {
            canvas: elements.canvas,
            ctx,
            container,
            dados,
            config,
            points: [],
            draw: function () {
                drawChart(this);
            }
        };

        // Desenha o gráfico inicial
        state.chartInstance.draw();

        // Event listeners
        elements.canvas.addEventListener('mousemove', handleCanvasMouseMove);
        elements.canvas.addEventListener('mouseleave', handleCanvasMouseLeave);

        // Redimensionar com debounce
        const debouncedResize = debounce(() => {
            if (state.chartInstance) {
                state.chartInstance.draw();
            }
        }, 250);

        window.addEventListener('resize', debouncedResize);

        console.log('📊 Gráfico Canvas criado com sucesso');
    }

    function drawChart(chart) {
        const { canvas, ctx, container, dados, config } = chart;

        // Ajusta tamanho do canvas
        const dpr = window.devicePixelRatio || 1;
        const rect = container.getBoundingClientRect();

        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;
        ctx.scale(dpr, dpr);

        const w = rect.width;
        const h = rect.height;
        const { padding } = config;

        // Limpa canvas
        ctx.clearRect(0, 0, w, h);

        // Calcula dimensões do gráfico
        const chartW = w - padding.left - padding.right;
        const chartH = h - padding.top - padding.bottom;

        if (dados.labels.length === 0) return;

        // Calcula limites do gráfico
        const allValues = [...dados.receitas, ...dados.despesas, ...dados.saldos];
        const maxVal = Math.max(...allValues);
        const minVal = Math.min(...allValues, 0);
        let range = maxVal - minVal;

        // Previne divisão por zero
        if (range === 0) range = Math.abs(maxVal) * 0.2 || 100;

        const offset = range * 0.15;
        const upperLimit = maxVal + offset;
        const lowerLimit = minVal - offset;
        const adjustedRange = upperLimit - lowerLimit;

        // Funções de posicionamento
        const getY = (val) => {
            return padding.top + chartH - ((val - lowerLimit) / adjustedRange) * chartH;
        };

        const getX = (i) => {
            return padding.left + (i / (dados.labels.length - 1)) * chartW;
        };

        // Armazena coordenadas dos pontos
        chart.points = {
            receitas: dados.receitas.map((val, i) => ({ x: getX(i), y: getY(val), value: val, label: dados.labels[i] })),
            despesas: dados.despesas.map((val, i) => ({ x: getX(i), y: getY(val), value: val, label: dados.labels[i] })),
            saldos: dados.saldos.map((val, i) => ({ x: getX(i), y: getY(val), value: val, label: dados.labels[i] }))
        };

        // 1. Desenha grade
        drawGrid(ctx, w, h, padding, chartH, upperLimit, adjustedRange, config);

        // 2. Desenha labels dos eixos
        drawAxes(ctx, dados.labels, padding, chartW, chartH, upperLimit, adjustedRange, config);

        // 3. Desenha linhas e áreas
        if (state.viewMode === 'area') {
            drawLineWithArea(ctx, chart.points.receitas, config.colors.receitas, 'rgba(16, 185, 129, 0.1)', padding, chartH, config);
            drawLineWithArea(ctx, chart.points.despesas, config.colors.despesas, 'rgba(239, 68, 68, 0.1)', padding, chartH, config);
            drawLineWithArea(ctx, chart.points.saldos, config.colors.saldo, 'rgba(59, 130, 246, 0.1)', padding, chartH, config);
        } else {
            drawLine(ctx, chart.points.receitas, config.colors.receitas, config);
            drawLine(ctx, chart.points.despesas, config.colors.despesas, config);
            drawLine(ctx, chart.points.saldos, config.colors.saldo, config);
        }

        // 4. Desenha pontos
        drawPoints(ctx, chart.points.receitas, config.colors.receitas, state.hoveredPoint, config);
        drawPoints(ctx, chart.points.despesas, config.colors.despesas, state.hoveredPoint, config);
        drawPoints(ctx, chart.points.saldos, config.colors.saldo, state.hoveredPoint, config);

        // 5. Linha vertical de hover
        if (state.hoveredPoint !== null) {
            drawHoverLine(ctx, state.hoveredPoint, h, padding, config);
        }
    }

    function drawGrid(ctx, w, h, padding, chartH, upperLimit, adjustedRange, config) {
        ctx.strokeStyle = config.colors.grid;
        ctx.lineWidth = 1;
        ctx.setLineDash([5, 5]);
        ctx.beginPath();

        for (let i = 0; i <= 5; i++) {
            const y = padding.top + (chartH / 5) * i;
            ctx.moveTo(padding.left, y);
            ctx.lineTo(w - padding.right, y);
        }

        ctx.stroke();
        ctx.setLineDash([]);
    }

    function drawAxes(ctx, labels, padding, chartW, chartH, upperLimit, adjustedRange, config) {
        // Labels Y
        ctx.fillStyle = config.colors.text;
        ctx.font = '12px Inter, sans-serif';
        ctx.textAlign = 'right';
        ctx.textBaseline = 'middle';

        for (let i = 0; i <= 5; i++) {
            const val = upperLimit - (adjustedRange / 5) * i;
            const y = padding.top + (chartH / 5) * i;
            ctx.fillText(formatCurrencyShort(val), padding.left - 10, y);
        }

        // Labels X
        ctx.textAlign = 'center';
        ctx.textBaseline = 'top';

        labels.forEach((label, i) => {
            const x = padding.left + (i / (labels.length - 1)) * chartW;
            ctx.fillText(label, x, padding.top + chartH + 15);
        });
    }

    function drawLine(ctx, points, color, config) {
        if (points.length === 0) return;

        ctx.strokeStyle = color;
        ctx.lineWidth = config.lineWidth;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.shadowColor = color;
        ctx.shadowBlur = 8;

        ctx.beginPath();
        ctx.moveTo(points[0].x, points[0].y);

        for (let i = 1; i < points.length; i++) {
            ctx.lineTo(points[i].x, points[i].y);
        }

        ctx.stroke();
        ctx.shadowBlur = 0;
    }

    function drawLineWithArea(ctx, points, color, fillColor, padding, chartH, config) {
        if (points.length === 0) return;

        // Área
        const gradient = ctx.createLinearGradient(0, padding.top, 0, padding.top + chartH);
        gradient.addColorStop(0, fillColor);
        gradient.addColorStop(1, 'rgba(0, 0, 0, 0)');

        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.moveTo(points[0].x, points[0].y);

        for (let i = 1; i < points.length; i++) {
            ctx.lineTo(points[i].x, points[i].y);
        }

        ctx.lineTo(points[points.length - 1].x, padding.top + chartH);
        ctx.lineTo(points[0].x, padding.top + chartH);
        ctx.closePath();
        ctx.fill();

        // Linha
        drawLine(ctx, points, color, config);
    }

    function drawPoints(ctx, points, color, hoveredIndex, config) {
        points.forEach((point, i) => {
            const isHovered = i === hoveredIndex;
            const radius = isHovered ? config.hoverPointRadius : config.pointRadius;

            // Círculo interno
            ctx.fillStyle = '#1b1d29';
            ctx.beginPath();
            ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
            ctx.fill();

            // Borda
            ctx.strokeStyle = isHovered ? config.colors.hover : color;
            ctx.lineWidth = isHovered ? 3 : 2;
            ctx.stroke();

            // Glow effect no hover
            if (isHovered) {
                ctx.shadowColor = color;
                ctx.shadowBlur = 15;
                ctx.beginPath();
                ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
                ctx.stroke();
                ctx.shadowBlur = 0;
            }
        });
    }

    function drawHoverLine(ctx, index, h, padding, config) {
        const point = state.chartInstance.points.saldos[index];
        if (!point) return;

        ctx.strokeStyle = 'rgba(99, 102, 241, 0.3)';
        ctx.lineWidth = 1;
        ctx.setLineDash([3, 3]);
        ctx.beginPath();
        ctx.moveTo(point.x, padding.top);
        ctx.lineTo(point.x, h - padding.bottom);
        ctx.stroke();
        ctx.setLineDash([]);
    }

    // ============================================================================
    // MOUSE TRACKING (TOOLTIP)
    // ============================================================================

    function handleCanvasMouseMove(e) {
        if (!state.chartInstance || !elements.tooltip) return;

        const rect = elements.canvas.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;

        const { padding, config } = state.chartInstance;
        const chartW = rect.width - padding.left - padding.right;
        const chartH = rect.height - padding.top - padding.bottom;

        if (mouseX < padding.left || mouseX > padding.left + chartW ||
            mouseY < padding.top || mouseY > padding.top + chartH) {
            hideTooltip();
            state.hoveredPoint = null;
            state.chartInstance.draw();
            elements.canvas.style.cursor = 'default';
            return;
        }

        // Encontra índice mais próximo
        const relativeX = mouseX - padding.left;
        const numPoints = state.chartInstance.dados.labels.length;
        const step = numPoints > 1 ? chartW / (numPoints - 1) : 0;
        const index = Math.round(relativeX / step);

        if (index >= 0 && index < numPoints) {
            state.hoveredPoint = index;
            showTooltip(index, mouseX, mouseY);
            state.chartInstance.draw();
            elements.canvas.style.cursor = 'pointer';
        }
    }

    function handleCanvasMouseLeave() {
        hideTooltip();
        state.hoveredPoint = null;
        if (state.chartInstance) {
            state.chartInstance.draw();
        }
        elements.canvas.style.cursor = 'default';
    }

    function showTooltip(index, mouseX, mouseY) {
        if (!elements.tooltip) return;

        const { dados } = state.chartInstance;

        // Atualiza conteúdo
        const monthElement = elements.tooltip.querySelector('.tooltip-month');
        const receitasElement = elements.tooltip.querySelector('.tooltip-value-receitas');
        const despesasElement = elements.tooltip.querySelector('.tooltip-value-despesas');
        const saldoElement = elements.tooltip.querySelector('.tooltip-value-saldo');

        if (monthElement) monthElement.textContent = dados.labels[index];
        if (receitasElement) receitasElement.textContent = formatCurrency(dados.receitas[index]);
        if (despesasElement) despesasElement.textContent = formatCurrency(dados.despesas[index]);
        if (saldoElement) saldoElement.textContent = formatCurrency(dados.saldos[index]);

        // Posiciona tooltip
        const rect = elements.canvas.getBoundingClientRect();
        const tooltipWidth = 200;
        const tooltipHeight = 140;

        let left = rect.left + mouseX + 15;
        let top = rect.top + mouseY - 30;

        // Ajusta se sair da tela
        if (left + tooltipWidth > window.innerWidth) {
            left = rect.left + mouseX - tooltipWidth - 15;
        }

        if (top < 0) {
            top = rect.top + mouseY + 15;
        }

        elements.tooltip.style.left = left + 'px';
        elements.tooltip.style.top = top + 'px';
        elements.tooltip.classList.add('active');
    }

    function hideTooltip() {
        if (elements.tooltip) {
            elements.tooltip.classList.remove('active');
        }
    }

    // ============================================================================
    // TOGGLE VIEW (LINHA/ÁREA)
    // ============================================================================

    function initViewToggle() {
        if (!elements.toggleBtns || elements.toggleBtns.length === 0) return;

        elements.toggleBtns.forEach(btn => {
            btn.addEventListener('click', function () {
                const view = this.getAttribute('data-view');

                if (view && view !== state.viewMode) {
                    state.viewMode = view;

                    // Atualiza UI
                    elements.toggleBtns.forEach(b => {
                        b.classList.remove('active');
                        b.setAttribute('aria-pressed', 'false');
                    });

                    this.classList.add('active');
                    this.setAttribute('aria-pressed', 'true');

                    // Redesenha com animação
                    if (state.chartInstance) {
                        animateChartTransition();
                    }

                    console.log(`📊 Visualização alterada: ${view}`);
                }
            });
        });
    }

    function animateChartTransition() {
        if (state.isAnimating) return;

        state.isAnimating = true;

        elements.canvas.style.opacity = '0.5';
        elements.canvas.style.transition = 'opacity 0.2s ease';

        setTimeout(() => {
            state.chartInstance.draw();
            elements.canvas.style.opacity = '1';

            setTimeout(() => {
                state.isAnimating = false;
                elements.canvas.style.transition = '';
            }, 200);
        }, 200);
    }

    // ============================================================================
    // EXPORT CSV
    // ============================================================================

    function initExport() {
        if (!elements.btnExportar) return;

        elements.btnExportar.addEventListener('click', exportToCSV);
    }

    function exportToCSV() {
        if (!state.chartInstance) {
            console.warn('⚠️ Sem dados para exportar');
            return;
        }

        const { dados } = state.chartInstance;

        // Cria conteúdo CSV
        let csvContent = 'Mês,Receitas,Despesas,Saldo\n';

        dados.labels.forEach((label, i) => {
            csvContent += `${label},"${formatCurrency(dados.receitas[i])}","${formatCurrency(dados.despesas[i])}","${formatCurrency(dados.saldos[i])}"\n`;
        });

        // Cria Blob
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const url = URL.createObjectURL(blob);

        // Cria link temporário
        const link = document.createElement('a');
        link.setAttribute('href', url);
        link.setAttribute('download', `tendencia-financeira-${getDateString()}.csv`);
        link.style.display = 'none';

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        URL.revokeObjectURL(url);

        console.log('💾 Dados exportados com sucesso');
    }

    // ============================================================================
    // ANIMAÇÕES
    // ============================================================================

    function initAnimations() {
        animateCards();
        animateCounters();
        animateBadges();
        addAnimationStyles();

        console.log('✨ Animações inicializadas');
    }

    function animateCards() {
        const cards = document.querySelectorAll('.metric-card, .summary-section, .month-card, .chart-section, .table-section');

        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((entry, index) => {
                    if (entry.isIntersecting) {
                        setTimeout(() => {
                            entry.target.style.opacity = '1';
                            entry.target.style.transform = 'translateY(0)';
                        }, index * 80);

                        observer.unobserve(entry.target);
                    }
                });
            }, {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            });

            cards.forEach(card => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.6s cubic-bezier(0.16, 1, 0.3, 1), transform 0.6s cubic-bezier(0.16, 1, 0.3, 1)';
                observer.observe(card);
            });
        }
    }

    function animateCounters() {
        const counters = document.querySelectorAll('.metric-value, .stat-value');

        counters.forEach(elemento => {
            const textoOriginal = elemento.textContent.trim();
            const numeroMatch = textoOriginal.match(/[\d,.]+/);
            if (!numeroMatch) return;

            const numeroStr = numeroMatch[0].replace(/\./g, '').replace(',', '.');
            const valorNumerico = parseFloat(numeroStr);

            if (isNaN(valorNumerico)) return;

            animateCounter(elemento, 0, valorNumerico, 1500, textoOriginal);
        });
    }

    function animateCounter(elemento, inicio, fim, duracao, formatoOriginal) {
        const passos = 60;
        const incremento = (fim - inicio) / passos;
        let atual = inicio;
        let contador = 0;

        const intervalo = duracao / passos;

        const timer = setInterval(() => {
            atual += incremento;
            contador++;

            if (contador >= passos) {
                atual = fim;
                clearInterval(timer);
            }

            if (formatoOriginal.includes('R$')) {
                elemento.textContent = formatCurrency(atual);
            } else if (formatoOriginal.includes('%')) {
                elemento.textContent = atual.toFixed(1) + '%';
            } else {
                elemento.textContent = Math.round(atual).toLocaleString('pt-BR');
            }
        }, intervalo);
    }

    function animateBadges() {
        const badges = document.querySelectorAll('.status-badge');

        badges.forEach((badge, index) => {
            setTimeout(() => {
                badge.style.animation = 'pulse 0.6s ease-out';
            }, 600 + (index * 150));
        });
    }

    // ============================================================================
    // TABLE INTERACTIONS
    // ============================================================================

    function setupTableInteractions() {
        const tableRows = document.querySelectorAll('.data-table tbody tr');

        tableRows.forEach(row => {
            row.addEventListener('click', function () {
                this.style.background = 'rgba(99, 102, 241, 0.1)';

                setTimeout(() => {
                    this.style.background = '';
                }, 300);
            });
        });
    }

    // ============================================================================
    // UTILITÁRIOS
    // ============================================================================

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func(...args), wait);
        };
    }

    function formatCurrency(valor) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
            maximumFractionDigits: 0
        }).format(valor);
    }

    function formatCurrencyShort(valor) {
        const absValor = Math.abs(valor);

        if (absValor >= 1000000) {
            return (valor >= 0 ? 'R$ ' : '-R$ ') + (absValor / 1000000).toFixed(1) + 'M';
        } else if (absValor >= 1000) {
            return (valor >= 0 ? 'R$ ' : '-R$ ') + (absValor / 1000).toFixed(1) + 'K';
        }

        return (valor >= 0 ? 'R$ ' : '-R$ ') + absValor.toFixed(0);
    }

    function getDateString() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    // ============================================================================
    // ADICIONAR ESTILOS DE ANIMAÇÃO
    // ============================================================================

    function addAnimationStyles() {
        if (document.getElementById('tendencia-resultado-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'tendencia-resultado-animations';
        style.textContent = `
            @keyframes pulse {
                0%, 100% { transform: scale(1); }
                50% { transform: scale(1.05); }
            }

            @media (prefers-reduced-motion: reduce) {
                * {
                    animation-duration: 0.01ms !important;
                    animation-iteration-count: 1 !important;
                    transition-duration: 0.01ms !important;
                }
            }
        `;
        document.head.appendChild(style);
    }

    // ============================================================================
    // EXPORT PARA DEBUGGING
    // ============================================================================

    window.TendenciaFinanceiraResultado = {
        state: () => state,
        redraw: () => state.chartInstance?.draw()
    };

    console.log('🚀 Tendência Financeira Resultado - Pronto para uso');

})();