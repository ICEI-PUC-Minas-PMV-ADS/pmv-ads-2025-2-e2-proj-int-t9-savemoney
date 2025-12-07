/* ============================================================================
   PROJEÇÃO FINANCEIRA
   100% Vanilla JavaScript - Zero Dependências
   Features: Tooltip, Toggle View, Export CSV, Smooth Animations
   ============================================================================ */

(() => {
    'use strict';

    // ============================================================================
    // ESTADO DA APLICAÇÃO
    // ============================================================================

    const state = {
        chartInstance: null,
        viewMode: 'area', // 'line' ou 'area'
        hoveredPoint: null,
        isAnimating: false
    };

    // ============================================================================
    // ELEMENTOS DO DOM (Cached)
    // ============================================================================

    const elements = {
        btnToggleInfo: null,
        infoSection: null,
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
        console.log('💰 Projeção Financeira - Módulo Carregado');

        // Cache de elementos
        cacheElements();

        // Configurar event listeners
        setupEventListeners();

        // Inicializar componentes
        initChart();
        initAccordion();
        animateKpiCards();
        initViewToggle();
        initExport();

        console.log('✅ Inicialização Completa');
    }

    // ============================================================================
    // CACHE DE ELEMENTOS
    // ============================================================================

    function cacheElements() {
        elements.btnToggleInfo = document.getElementById('btnToggleInfo');
        elements.infoSection = document.getElementById('infoSection');
        elements.canvas = document.getElementById('meuGraficoCanvas');
        elements.tooltip = document.getElementById('chartTooltip');
        elements.btnExportar = document.getElementById('btnExportar');
        elements.toggleBtns = document.querySelectorAll('.toggle-btn');
    }

    // ============================================================================
    // EVENT LISTENERS
    // ============================================================================

    function setupEventListeners() {
        // Toggle da seção educativa
        if (elements.btnToggleInfo && elements.infoSection) {
            elements.btnToggleInfo.addEventListener('click', toggleInfoSection);
        }

        // Redimensionamento do gráfico (debounced)
        const debouncedResize = debounce(() => {
            if (state.chartInstance) {
                state.chartInstance.draw();
            }
        }, 250);

        window.addEventListener('resize', debouncedResize);

        // Mouse tracking no canvas (para tooltip)
        if (elements.canvas) {
            elements.canvas.addEventListener('mousemove', handleCanvasMouseMove);
            elements.canvas.addEventListener('mouseleave', handleCanvasMouseLeave);
        }
    }

    // ============================================================================
    // TOGGLE INFO SECTION
    // ============================================================================

    function toggleInfoSection() {
        if (!elements.infoSection || !elements.btnToggleInfo) return;

        const btnText = document.getElementById('btnInfoText');
        const isHidden = elements.infoSection.style.display === 'none' ||
            !elements.infoSection.style.display;

        if (isHidden) {
            // Mostra seção
            elements.infoSection.style.display = 'block';
            elements.btnToggleInfo.setAttribute('aria-expanded', 'true');

            if (btnText) {
                btnText.textContent = 'Ocultar Explicação';
            }

            // Scroll suave após renderização
            requestAnimationFrame(() => {
                setTimeout(() => {
                    scrollToElement(elements.infoSection, 100);
                }, 100);
            });
        } else {
            // Oculta seção
            elements.infoSection.style.display = 'none';
            elements.btnToggleInfo.setAttribute('aria-expanded', 'false');

            if (btnText) {
                btnText.textContent = 'Como Funciona';
            }
        }
    }

    // ============================================================================
    // ACCORDION (FAQ)
    // ============================================================================

    function initAccordion() {
        const accordionButtons = document.querySelectorAll('.accordion-button');

        accordionButtons.forEach(button => {
            button.addEventListener('click', function () {
                const isExpanded = this.getAttribute('aria-expanded') === 'true';

                // Fecha todos os outros itens
                accordionButtons.forEach(btn => {
                    if (btn !== this) {
                        btn.setAttribute('aria-expanded', 'false');
                        const content = btn.nextElementSibling;
                        if (content && content.classList.contains('accordion-content')) {
                            content.style.maxHeight = '0';
                        }
                    }
                });

                // Toggle do item atual
                const content = this.nextElementSibling;
                if (content && content.classList.contains('accordion-content')) {
                    if (isExpanded) {
                        this.setAttribute('aria-expanded', 'false');
                        content.style.maxHeight = '0';
                    } else {
                        this.setAttribute('aria-expanded', 'true');
                        content.style.maxHeight = content.scrollHeight + 'px';

                        // Reajusta se necessário
                        setTimeout(() => {
                            content.style.maxHeight = content.scrollHeight + 'px';
                        }, 50);
                    }
                }
            });
        });
    }

    // ============================================================================
    // GRÁFICO CANVAS
    // ============================================================================

    function initChart() {
        if (!elements.canvas) {
            console.warn('⚠️ Canvas não encontrado');
            return;
        }

        if (!window.saveMoneyDados) {
            console.warn('⚠️ Dados do gráfico não disponíveis');
            return;
        }

        const ctx = elements.canvas.getContext('2d');
        if (!ctx) {
            console.error('❌ Erro ao obter contexto do canvas');
            return;
        }

        const { labels, values } = window.saveMoneyDados;

        if (!Array.isArray(labels) || !Array.isArray(values)) {
            console.error('❌ Formato de dados inválido');
            return;
        }

        if (labels.length === 0 || values.length === 0) {
            console.warn('⚠️ Dados vazios');
            return;
        }

        // Configurações do gráfico
        const config = {
            colors: {
                primary: '#0dcaf0',      // Cyan accent
                secondary: '#3b82f6',    // Blue
                text: '#aaaaaa',
                grid: '#2a2c3c',
                bgStart: 'rgba(13, 202, 240, 0.3)',
                bgEnd: 'rgba(13, 202, 240, 0.0)',
                pointFill: '#1b1d29',
                hover: '#0aa2c0'
            },
            padding: {
                top: 40,
                right: 30,
                bottom: 50,
                left: 70
            },
            gridLines: 5,
            pointRadius: 6,
            lineWidth: 3,
            hoverPointRadius: 8
        };

        // Cria instância do gráfico
        state.chartInstance = {
            canvas: elements.canvas,
            ctx,
            labels,
            values,
            config,
            points: [], // Armazena coordenadas dos pontos
            draw: function () {
                drawChart(this);
            }
        };

        // Desenha o gráfico inicial
        state.chartInstance.draw();

        console.log('📊 Gráfico inicializado');
    }

    function drawChart(chart) {
        const { canvas, ctx, labels, values, config } = chart;
        const container = canvas.parentElement;

        // Ajusta tamanho do canvas
        canvas.width = container.clientWidth;
        canvas.height = container.clientHeight;

        const w = canvas.width;
        const h = canvas.height;
        const { padding } = config;

        // Limpa canvas
        ctx.clearRect(0, 0, w, h);

        const chartW = w - padding.left - padding.right;
        const chartH = h - padding.top - padding.bottom;

        if (values.length === 0) return;

        // Calcula limites
        const maxVal = Math.max(...values);
        const minVal = Math.min(...values);
        let range = maxVal - minVal;

        if (range === 0) range = Math.abs(maxVal) * 0.2 || 100;

        const offset = range * 0.15;
        const upperLimit = maxVal + offset;
        const lowerLimit = minVal - offset;
        const adjustedRange = upperLimit - lowerLimit;

        // Funções de posicionamento
        const getY = (val) => {
            return padding.top + chartH - ((val - lowerLimit) / adjustedRange) * chartH;
        };

        // CORREÇÃO 1: Evitar divisão por zero se tiver apenas 1 ponto
        const getX = (i) => {
            if (labels.length <= 1) return padding.left + (chartW / 2); // Centraliza se for só 1 ponto
            return padding.left + (i / (labels.length - 1)) * chartW;
        };

        // Armazena coordenadas
        chart.points = values.map((val, i) => ({
            x: getX(i),
            y: getY(val),
            value: val,
            label: labels[i]
        }));

        drawGrid(ctx, w, h, padding, chartH, upperLimit, adjustedRange, config);

        if (state.viewMode === 'area' && values.length > 1) { // Só desenha área se > 1 ponto
            drawArea(ctx, values, getX, getY, h, padding, config);
        }

        if (values.length > 1) { // Só desenha linha se > 1 ponto
            drawLine(ctx, values, getX, getY, config);
        }

        // Desenha pontos (funciona mesmo com 1 ponto)
        drawPoints(ctx, chart.points, state.hoveredPoint, config, h, padding); // Passando h e padding

        if (state.hoveredPoint !== null) {
            drawHoverLine(ctx, chart.points[state.hoveredPoint], h, padding, config);
        }
    }

    // ... drawGrid, drawArea e drawLine continuam iguais ...

    // CORREÇÃO 2: Passar h e padding para fixar labels no rodapé
    function drawPoints(ctx, points, hoveredIndex, config, h, padding) {
        points.forEach((point, i) => {
            const isHovered = i === hoveredIndex;
            const radius = isHovered ? config.hoverPointRadius : config.pointRadius;

            // Círculo interno
            ctx.fillStyle = config.colors.pointFill;
            ctx.beginPath();
            ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
            ctx.fill();

            // Borda
            ctx.strokeStyle = isHovered ? config.colors.hover : config.colors.primary;
            ctx.lineWidth = isHovered ? 3 : 2;
            ctx.stroke();

            // Glow effect no hover
            if (isHovered) {
                ctx.shadowColor = config.colors.primary;
                ctx.shadowBlur = 15;
                ctx.beginPath();
                ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
                ctx.stroke();
                ctx.shadowBlur = 0;
            }

            // CORREÇÃO: Label fixo na parte inferior (usando altura do canvas)
            // Antes estava: point[0].y + 80 (o que fazia o texto pular com o gráfico)
            ctx.fillStyle = config.colors.text;
            ctx.font = isHovered ? 'bold 13px Inter' : '12px Inter';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'top';
            // Fixa o texto 15px abaixo da área do gráfico
            ctx.fillText(point.label, point.x, h - padding.bottom + 15);
        });
    }

    function drawGrid(ctx, w, h, padding, chartH, upperLimit, adjustedRange, config) {
        ctx.strokeStyle = config.colors.grid;
        ctx.lineWidth = 1;
        ctx.setLineDash([5, 5]); // Linha tracejada
        ctx.beginPath();

        for (let i = 0; i <= config.gridLines; i++) {
            const y = padding.top + (chartH / config.gridLines) * i;

            // Linha horizontal
            ctx.moveTo(padding.left, y);
            ctx.lineTo(w - padding.right, y);

            // Label do valor (à esquerda)
            const val = upperLimit - (adjustedRange / config.gridLines) * i;
            ctx.fillStyle = config.colors.text;
            ctx.font = '12px Inter, sans-serif';
            ctx.textAlign = 'right';
            ctx.textBaseline = 'middle';
            ctx.fillText(
                formatCurrency(val),
                padding.left - 10,
                y
            );
        }

        ctx.stroke();
        ctx.setLineDash([]); // Remove tracejado
    }

    function drawArea(ctx, values, getX, getY, h, padding, config) {
        const gradient = ctx.createLinearGradient(0, padding.top, 0, h - padding.bottom);
        gradient.addColorStop(0, config.colors.bgStart);
        gradient.addColorStop(1, config.colors.bgEnd);

        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.moveTo(getX(0), getY(values[0]));

        for (let i = 1; i < values.length; i++) {
            ctx.lineTo(getX(i), getY(values[i]));
        }

        ctx.lineTo(getX(values.length - 1), h - padding.bottom);
        ctx.lineTo(getX(0), h - padding.bottom);
        ctx.closePath();
        ctx.fill();
    }

    function drawLine(ctx, values, getX, getY, config) {
        ctx.strokeStyle = config.colors.primary;
        ctx.lineWidth = config.lineWidth;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.shadowColor = 'rgba(13, 202, 240, 0.5)';
        ctx.shadowBlur = 10;

        ctx.beginPath();
        ctx.moveTo(getX(0), getY(values[0]));

        for (let i = 1; i < values.length; i++) {
            ctx.lineTo(getX(i), getY(values[i]));
        }

        ctx.stroke();
        ctx.shadowBlur = 0; // Remove sombra
    }

    function drawPoints(ctx, points, hoveredIndex, config) {
        points.forEach((point, i) => {
            const isHovered = i === hoveredIndex;
            const radius = isHovered ? config.hoverPointRadius : config.pointRadius;

            // Círculo interno
            ctx.fillStyle = config.colors.pointFill;
            ctx.beginPath();
            ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
            ctx.fill();

            // Borda
            ctx.strokeStyle = isHovered ? config.colors.hover : config.colors.primary;
            ctx.lineWidth = isHovered ? 3 : 2;
            ctx.stroke();

            // Glow effect no hover
            if (isHovered) {
                ctx.shadowColor = config.colors.primary;
                ctx.shadowBlur = 15;
                ctx.beginPath();
                ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
                ctx.stroke();
                ctx.shadowBlur = 0;
            }

            // Label do mês (embaixo)
            ctx.fillStyle = config.colors.text;
            ctx.font = isHovered ? 'bold 13px Inter' : '12px Inter';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'top';
            ctx.fillText(point.label, point.x, points[0].y + 80);
        });
    }

    function drawHoverLine(ctx, point, h, padding, config) {
        ctx.strokeStyle = 'rgba(13, 202, 240, 0.3)';
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

        // Encontra ponto mais próximo
        let closestPoint = null;
        let closestIndex = null;
        let minDistance = Infinity;

        state.chartInstance.points.forEach((point, i) => {
            const distance = Math.sqrt(
                Math.pow(mouseX - point.x, 2) +
                Math.pow(mouseY - point.y, 2)
            );

            if (distance < minDistance && distance < 30) { // Raio de 30px
                minDistance = distance;
                closestPoint = point;
                closestIndex = i;
            }
        });

        if (closestPoint) {
            // Atualiza estado
            state.hoveredPoint = closestIndex;

            // Mostra tooltip
            showTooltip(closestPoint, mouseX, mouseY);

            // Redesenha gráfico
            state.chartInstance.draw();

            // Cursor pointer
            elements.canvas.style.cursor = 'pointer';
        } else {
            hideTooltip();
            state.hoveredPoint = null;
            state.chartInstance.draw();
            elements.canvas.style.cursor = 'default';
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

    function showTooltip(point, mouseX, mouseY) {
        if (!elements.tooltip) return;

        // Atualiza conteúdo
        const monthElement = elements.tooltip.querySelector('.tooltip-month');
        const valueElement = elements.tooltip.querySelector('.tooltip-value');

        if (monthElement) monthElement.textContent = point.label;
        if (valueElement) valueElement.textContent = formatCurrency(point.value);

        // Posiciona tooltip
        const rect = elements.canvas.getBoundingClientRect();
        const tooltipWidth = 120;
        const tooltipHeight = 60;

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
                    // Atualiza estado
                    state.viewMode = view;

                    // Atualiza UI dos botões
                    elements.toggleBtns.forEach(b => {
                        b.classList.remove('active');
                        b.setAttribute('aria-pressed', 'false');
                    });

                    this.classList.add('active');
                    this.setAttribute('aria-pressed', 'true');

                    // Redesenha gráfico com animação
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

        // Fade out
        elements.canvas.style.opacity = '0.5';
        elements.canvas.style.transition = 'opacity 0.2s ease';

        setTimeout(() => {
            // Redesenha
            state.chartInstance.draw();

            // Fade in
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
        if (!window.saveMoneyDados) {
            console.warn('⚠️ Sem dados para exportar');
            return;
        }

        const { labels, values } = window.saveMoneyDados;

        // Cria conteúdo CSV
        let csvContent = 'Mês,Saldo Projetado\n';

        labels.forEach((label, i) => {
            const value = values[i];
            csvContent += `${label},"${formatCurrency(value)}"\n`;
        });

        // Cria Blob
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const url = URL.createObjectURL(blob);

        // Cria link temporário
        const link = document.createElement('a');
        link.setAttribute('href', url);
        link.setAttribute('download', `projecao-financeira-${getDateString()}.csv`);
        link.style.display = 'none';

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        // Libera memória
        URL.revokeObjectURL(url);

        console.log('💾 Dados exportados com sucesso');
    }

    // ============================================================================
    // ANIMAÇÕES KPI CARDS
    // ============================================================================

    function animateKpiCards() {
        const kpiCards = document.querySelectorAll('.kpi-card, .summary-section');

        if ('IntersectionObserver' in window && kpiCards.length > 0) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((entry, index) => {
                    if (entry.isIntersecting) {
                        setTimeout(() => {
                            entry.target.style.opacity = '1';
                            entry.target.style.transform = 'translateY(0)';
                        }, index * 80); // Stagger animation

                        observer.unobserve(entry.target);
                    }
                });
            }, {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            });

            kpiCards.forEach(card => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.6s ease, transform 0.6s cubic-bezier(0.16, 1, 0.3, 1)';
                observer.observe(card);
            });
        }
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

    function formatCurrency(value) {
        return value.toLocaleString('pt-BR', {
            style: 'currency',
            currency: 'BRL',
            maximumFractionDigits: 0
        });
    }

    function getDateString() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    function scrollToElement(element, offset = 100) {
        const elementPosition = element.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.scrollY - offset;

        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });
    }

    // ============================================================================
    // SMOOTH SCROLL PARA LINKS INTERNOS
    // ============================================================================

    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

            if (href === '#' || href === '#!') return;

            e.preventDefault();

            const target = document.querySelector(href);
            if (target) {
                scrollToElement(target);
            }
        });
    });

    // ============================================================================
    // EXPOR FUNÇÕES GLOBAIS (Compatibilidade)
    // ============================================================================

    window.toggleInfoSection = toggleInfoSection;

    console.log('🚀 Projeção Financeira - Pronto para uso');

})();