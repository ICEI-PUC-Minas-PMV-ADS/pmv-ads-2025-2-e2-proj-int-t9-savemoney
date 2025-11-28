/* ============================================================================
   TENDÊNCIA FINANCEIRA - RESULTADO
   Gráfico Canvas Nativo (SEM BIBLIOTECAS)
   ============================================================================ */

(() => {
    'use strict';

    // Estado do gráfico
    let graficoState = null;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeTendenciaResultado);

    function initializeTendenciaResultado() {
        console.log('Tendência Financeira - Resultado loaded');

        // Inicializar módulos
        initializeChart();
        initializeAnimations();
        setupTableInteractions();
    }

    /* ========================================================================
       GRÁFICO CANVAS NATIVO
       ======================================================================== */

    function initializeChart() {
        const canvas = document.getElementById('graficoTendencia');

        if (!canvas) {
            console.error('Canvas do gráfico não encontrado');
            return;
        }

        // Buscar dados dos data attributes
        const labelsJson = canvas.getAttribute('data-labels');
        const receitasJson = canvas.getAttribute('data-receitas');
        const despesasJson = canvas.getAttribute('data-despesas');
        const saldosJson = canvas.getAttribute('data-saldos');

        if (!labelsJson || !receitasJson || !despesasJson || !saldosJson) {
            console.error('Dados do gráfico não encontrados');
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
                console.error('Dados do gráfico estão vazios');
                return;
            }

            if (dados.labels.length !== dados.receitas.length ||
                dados.labels.length !== dados.despesas.length ||
                dados.labels.length !== dados.saldos.length) {
                console.error('Dados do gráfico com tamanhos incompatíveis');
                return;
            }

            console.log('Dados do gráfico carregados:', dados);
            createChart(canvas, dados);

        } catch (error) {
            console.error('Erro ao parsear dados do gráfico:', error);
        }
    }

    function createChart(canvas, dados) {
        const ctx = canvas.getContext('2d');
        const dpr = window.devicePixelRatio || 1;

        // Ajustar canvas para alta resolução
        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;
        ctx.scale(dpr, dpr);

        const width = rect.width;
        const height = rect.height;

        // Padding do gráfico
        const padding = {
            top: 60,
            right: 40,
            bottom: 60,
            left: 80
        };

        const graphWidth = width - padding.left - padding.right;
        const graphHeight = height - padding.top - padding.bottom;

        // Calcular valores min/max
        const allValues = [...dados.receitas, ...dados.despesas, ...dados.saldos];
        const maxValue = Math.max(...allValues);
        const minValue = Math.min(...allValues, 0);
        const valueRange = maxValue - minValue || 1;

        // Salvar estado
        graficoState = {
            canvas,
            ctx,
            width,
            height,
            padding,
            graphWidth,
            graphHeight,
            dados,
            maxValue,
            minValue,
            valueRange,
            hoveredPoint: null
        };

        // Desenhar gráfico
        drawChart();

        // Event listeners
        canvas.addEventListener('mousemove', handleMouseMove);
        canvas.addEventListener('mouseleave', handleMouseLeave);

        // Redimensionar com debounce
        let resizeTimeout;
        window.addEventListener('resize', () => {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                console.log('Redimensionando gráfico...');
                createChart(canvas, dados);
            }, 250);
        });

        console.log('Gráfico Canvas criado com sucesso');
    }

    function drawChart() {
        if (!graficoState) return;

        const { ctx, width, height } = graficoState;

        // Limpar canvas
        ctx.clearRect(0, 0, width, height);

        // Desenhar componentes
        drawGrid();
        drawAxes();
        drawLine(graficoState.dados.receitas, '#10b981', 'rgba(16, 185, 129, 0.1)', false);
        drawLine(graficoState.dados.despesas, '#ef4444', 'rgba(239, 68, 68, 0.1)', false);
        drawLine(graficoState.dados.saldos, '#3b82f6', 'rgba(59, 130, 246, 0.1)', true);
        drawLegend();

        // Tooltip se hover
        if (graficoState.hoveredPoint !== null) {
            drawTooltip();
        }
    }

    function drawGrid() {
        const { ctx, padding, graphWidth, graphHeight } = graficoState;

        ctx.strokeStyle = 'rgba(255, 255, 255, 0.05)';
        ctx.lineWidth = 1;

        for (let i = 0; i <= 5; i++) {
            const y = padding.top + (graphHeight / 5) * i;
            ctx.beginPath();
            ctx.moveTo(padding.left, y);
            ctx.lineTo(padding.left + graphWidth, y);
            ctx.stroke();
        }
    }

    function drawAxes() {
        const { ctx, padding, graphWidth, graphHeight, dados, maxValue, valueRange } = graficoState;

        // Labels Y
        ctx.fillStyle = '#aaaaaa';
        ctx.font = '12px Inter, sans-serif';
        ctx.textAlign = 'right';
        ctx.textBaseline = 'middle';

        for (let i = 0; i <= 5; i++) {
            const value = maxValue - (valueRange / 5) * i;
            const y = padding.top + (graphHeight / 5) * i;
            ctx.fillText(formatCurrencyShort(value), padding.left - 10, y);
        }

        // Labels X
        ctx.textAlign = 'center';
        ctx.textBaseline = 'top';

        const numLabels = dados.labels.length;
        const step = numLabels > 1 ? graphWidth / (numLabels - 1) : 0;

        dados.labels.forEach((label, i) => {
            const x = padding.left + step * i;
            ctx.fillText(label, x, padding.top + graphHeight + 10);
        });
    }

    function drawLine(values, color, fillColor, dashed) {
        const { ctx, padding, graphWidth, graphHeight, minValue, valueRange } = graficoState;

        const numPoints = values.length;
        if (numPoints === 0) return;

        const step = numPoints > 1 ? graphWidth / (numPoints - 1) : 0;

        // Área preenchida
        ctx.fillStyle = fillColor;
        ctx.beginPath();

        values.forEach((value, i) => {
            const x = padding.left + step * i;
            const y = padding.top + graphHeight - ((value - minValue) / valueRange) * graphHeight;

            if (i === 0) {
                ctx.moveTo(x, y);
            } else {
                ctx.lineTo(x, y);
            }
        });

        ctx.lineTo(padding.left + graphWidth, padding.top + graphHeight);
        ctx.lineTo(padding.left, padding.top + graphHeight);
        ctx.closePath();
        ctx.fill();

        // Linha
        ctx.strokeStyle = color;
        ctx.lineWidth = 3;

        if (dashed) {
            ctx.setLineDash([5, 5]);
        } else {
            ctx.setLineDash([]);
        }

        ctx.beginPath();
        values.forEach((value, i) => {
            const x = padding.left + step * i;
            const y = padding.top + graphHeight - ((value - minValue) / valueRange) * graphHeight;

            if (i === 0) {
                ctx.moveTo(x, y);
            } else {
                ctx.lineTo(x, y);
            }
        });
        ctx.stroke();
        ctx.setLineDash([]);

        // Pontos
        values.forEach((value, i) => {
            const x = padding.left + step * i;
            const y = padding.top + graphHeight - ((value - minValue) / valueRange) * graphHeight;

            ctx.fillStyle = '#fff';
            ctx.beginPath();
            ctx.arc(x, y, 6, 0, Math.PI * 2);
            ctx.fill();

            ctx.strokeStyle = color;
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(x, y, 6, 0, Math.PI * 2);
            ctx.stroke();
        });
    }

    function drawLegend() {
        const { ctx, width } = graficoState;

        const legendas = [
            { label: 'Receitas', color: '#10b981' },
            { label: 'Despesas', color: '#ef4444' },
            { label: 'Saldo', color: '#3b82f6' }
        ];

        const legendWidth = 300;
        const startX = (width - legendWidth) / 2;
        const y = 20;

        ctx.font = '600 13px Inter, sans-serif';
        ctx.textAlign = 'left';
        ctx.textBaseline = 'middle';

        legendas.forEach((item, i) => {
            const x = startX + i * 100;

            ctx.fillStyle = item.color;
            ctx.beginPath();
            ctx.arc(x, y, 5, 0, Math.PI * 2);
            ctx.fill();

            ctx.fillStyle = '#f5f5ff';
            ctx.fillText(item.label, x + 12, y);
        });
    }

    function drawTooltip() {
        const { ctx, padding, graphWidth, dados, hoveredPoint, width } = graficoState;

        if (hoveredPoint === null || hoveredPoint.index === undefined) return;

        const i = hoveredPoint.index;
        if (i < 0 || i >= dados.labels.length) return;

        const numPoints = dados.labels.length;
        const step = numPoints > 1 ? graphWidth / (numPoints - 1) : 0;
        const x = padding.left + step * i;

        const tooltipData = [
            { label: 'Receitas', value: dados.receitas[i], color: '#10b981' },
            { label: 'Despesas', value: dados.despesas[i], color: '#ef4444' },
            { label: 'Saldo', value: dados.saldos[i], color: '#3b82f6' }
        ];

        const tooltipWidth = 200;
        const tooltipHeight = 110;
        const tooltipX = x > width / 2 ? x - tooltipWidth - 15 : x + 15;
        const tooltipY = padding.top + 20;

        // Fundo
        ctx.fillStyle = 'rgba(27, 29, 41, 0.95)';
        ctx.strokeStyle = 'rgba(99, 102, 241, 0.3)';
        ctx.lineWidth = 1;

        ctx.beginPath();
        ctx.roundRect(tooltipX, tooltipY, tooltipWidth, tooltipHeight, 8);
        ctx.fill();
        ctx.stroke();

        // Título
        ctx.fillStyle = '#f5f5ff';
        ctx.font = 'bold 14px Inter, sans-serif';
        ctx.textAlign = 'left';
        ctx.textBaseline = 'top';
        ctx.fillText(dados.labels[i], tooltipX + 12, tooltipY + 12);

        // Valores
        ctx.font = '13px Inter, sans-serif';
        tooltipData.forEach((item, idx) => {
            const yPos = tooltipY + 40 + idx * 22;

            ctx.fillStyle = item.color;
            ctx.beginPath();
            ctx.arc(tooltipX + 15, yPos + 5, 4, 0, Math.PI * 2);
            ctx.fill();

            ctx.fillStyle = '#aaaaaa';
            ctx.fillText(`${item.label}: ${formatCurrency(item.value)}`, tooltipX + 28, yPos);
        });
    }

    function handleMouseMove(e) {
        if (!graficoState) return;

        const { canvas, padding, graphWidth, graphHeight, dados } = graficoState;
        const rect = canvas.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        if (x < padding.left || x > padding.left + graphWidth ||
            y < padding.top || y > padding.top + graphHeight) {
            graficoState.hoveredPoint = null;
            drawChart();
            return;
        }

        const numPoints = dados.labels.length;
        const step = numPoints > 1 ? graphWidth / (numPoints - 1) : 0;
        const relativeX = x - padding.left;
        const index = Math.round(relativeX / step);

        if (index >= 0 && index < dados.labels.length) {
            graficoState.hoveredPoint = { index };
            drawChart();
        }
    }

    function handleMouseLeave() {
        if (!graficoState) return;

        graficoState.hoveredPoint = null;
        drawChart();
    }

    /* ========================================================================
       ANIMAÇÕES E INTERATIVIDADE
       ======================================================================== */

    function initializeAnimations() {
        animateCards();
        animateCounters();
        addHoverEffects();
        animateBadges();
        addAnimationStyles();

        console.log('Animações inicializadas');
    }

    function animateCards() {
        const cards = document.querySelectorAll('.metric-card, .glass-panel, .summary-banner');

        cards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(1.25rem)';

            setTimeout(() => {
                card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, index * 100);
        });
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

    function addHoverEffects() {
        const cards = document.querySelectorAll('.metric-card, .month-card');

        cards.forEach(card => {
            card.style.transition = 'transform 0.3s ease, box-shadow 0.3s ease';

            card.addEventListener('mouseenter', function () {
                this.style.transform = 'translateY(-0.3125rem)';
            });

            card.addEventListener('mouseleave', function () {
                this.style.transform = 'translateY(0)';
            });
        });
    }

    function animateBadges() {
        const badges = document.querySelectorAll('.status-badge');

        badges.forEach((badge, index) => {
            setTimeout(() => {
                badge.style.animation = 'pulse 0.6s ease-out';
            }, 600 + (index * 150));
        });
    }

    /* ========================================================================
       TABLE INTERACTIONS
       ======================================================================== */

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

    /* ========================================================================
       UTILITÁRIOS - FORMATAÇÃO
       ======================================================================== */

    function formatCurrency(valor) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
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

    /* ========================================================================
       ANIMATION STYLES
       ======================================================================== */

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
        `;
        document.head.appendChild(style);
    }

    /* ========================================================================
       EXPORT PARA DEBUGGING
       ======================================================================== */

    window.TendenciaFinanceiraResultado = {
        graficoState: () => graficoState,
        redesenhar: () => drawChart()
    };

})();