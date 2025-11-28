/* ============================================================================
   PROJEÇÃO FINANCEIRA
   ============================================================================ */

(() => {
    'use strict';

    // Estado da aplicação
    let chartInstance = null;

    // Elementos do DOM
    let btnToggleInfo = null;
    let infoSection = null;
    let canvas = null;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeProjecao);

    function initializeProjecao() {
        console.log('Projecao module loaded');

        // Cache de elementos
        btnToggleInfo = document.getElementById('btnToggleInfo');
        infoSection = document.getElementById('infoSection');
        canvas = document.getElementById('meuGraficoCanvas');

        // Configurar event listeners
        setupEventListeners();

        // Inicializar componentes
        initChart();
        initAccordion();
        animateKpiCards();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Toggle da seção educativa
        if (btnToggleInfo && infoSection) {
            btnToggleInfo.addEventListener('click', toggleInfoSection);
        }

        // Redimensionamento do gráfico
        let resizeTimer;
        window.addEventListener('resize', () => {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(() => {
                if (chartInstance) {
                    chartInstance.draw();
                }
            }, 250);
        });
    }

    /* Toggle Info Section
       ======================================================================== */
    function toggleInfoSection() {
        if (!infoSection || !btnToggleInfo) return;

        const btnText = document.getElementById('btnInfoText');
        const isHidden = infoSection.style.display === 'none' ||
            !infoSection.style.display;

        if (isHidden) {
            // Mostra seção
            infoSection.style.display = 'block';
            btnToggleInfo.setAttribute('aria-expanded', 'true');

            if (btnText) {
                btnText.textContent = 'Ocultar Explicação';
            }

            // Scroll suave após renderização
            requestAnimationFrame(() => {
                setTimeout(() => {
                    const headerOffset = 100;
                    const elementPosition = infoSection.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.scrollY - headerOffset;

                    window.scrollTo({
                        top: offsetPosition,
                        behavior: 'smooth'
                    });
                }, 100);
            });
        } else {
            // Oculta seção
            infoSection.style.display = 'none';
            btnToggleInfo.setAttribute('aria-expanded', 'false');

            if (btnText) {
                btnText.textContent = 'Entenda o Cálculo';
            }
        }
    }

    /* Accordion (FAQ)
       ======================================================================== */
    function initAccordion() {
        const accordionButtons = document.querySelectorAll('.accordion-button');

        accordionButtons.forEach(button => {
            button.addEventListener('click', function () {
                const contentId = this.getAttribute('aria-controls');
                const content = document.getElementById(contentId);
                const isExpanded = this.getAttribute('aria-expanded') === 'true';

                // Fecha todos os outros (accordion single)
                accordionButtons.forEach(btn => {
                    if (btn !== this) {
                        btn.setAttribute('aria-expanded', 'false');
                        const otherContent = document.getElementById(
                            btn.getAttribute('aria-controls')
                        );
                        if (otherContent) {
                            closeAccordionItem(otherContent);
                        }
                    }
                });

                // Toggle do item atual
                if (isExpanded) {
                    this.setAttribute('aria-expanded', 'false');
                    closeAccordionItem(content);
                } else {
                    this.setAttribute('aria-expanded', 'true');
                    openAccordionItem(content);
                }
            });
        });
    }

    function openAccordionItem(content) {
        if (!content) return;

        content.style.maxHeight = content.scrollHeight + 'px';

        // Ajusta maxHeight se o conteúdo mudar
        setTimeout(() => {
            content.style.maxHeight = content.scrollHeight + 'px';
        }, 50);
    }

    function closeAccordionItem(content) {
        if (!content) return;
        content.style.maxHeight = '0';
    }

    /* Gráfico Canvas
       ======================================================================== */
    function initChart() {
        if (!canvas) {
            console.warn('Canvas element not found');
            return;
        }

        if (!window.saveMoneyDados) {
            console.warn('Chart data not available');
            return;
        }

        const ctx = canvas.getContext('2d');
        if (!ctx) {
            console.error('Could not get canvas context');
            return;
        }

        const labels = window.saveMoneyDados.labels;
        const values = window.saveMoneyDados.values;

        if (!Array.isArray(labels) || !Array.isArray(values)) {
            console.error('Invalid chart data format');
            return;
        }

        if (labels.length === 0 || values.length === 0) {
            console.warn('Empty chart data');
            return;
        }

        // Configurações do gráfico
        const config = {
            colors: {
                primary: '#3b82f6',
                accent: '#0dcaf0',
                text: '#aaaaaa',
                grid: '#2a2c3c',
                bgStart: 'rgba(59, 130, 246, 0.4)',
                bgEnd: 'rgba(59, 130, 246, 0.0)',
                pointFill: '#1b1d29'
            },
            padding: {
                top: 40,
                right: 30,
                bottom: 30,
                left: 60
            },
            gridLines: 4,
            pointRadius: 5,
            lineWidth: 3
        };

        // Cria instância do gráfico
        chartInstance = {
            canvas,
            ctx,
            labels,
            values,
            config,
            draw: function () {
                drawChart(this);
            }
        };

        // Desenha o gráfico inicial
        chartInstance.draw();
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

        // Calcula dimensões do gráfico
        const chartW = w - padding.left - padding.right;
        const chartH = h - padding.top - padding.bottom;

        if (values.length === 0) return;

        // Calcula limites do gráfico
        const maxVal = Math.max(...values);
        const minVal = Math.min(...values);
        let range = maxVal - minVal;

        // Previne divisão por zero
        if (range === 0) range = 100;

        const offset = range * 0.2;
        const upperLimit = maxVal + offset;
        const lowerLimit = minVal - offset;
        const adjustedRange = upperLimit - lowerLimit;

        // Funções de posicionamento
        const getY = (val) => {
            return padding.top + chartH - ((val - lowerLimit) / adjustedRange) * chartH;
        };

        const getX = (i) => {
            return padding.left + (i / (labels.length - 1)) * chartW;
        };

        // 1. Desenha grade
        drawGrid(ctx, w, h, padding, chartH, upperLimit, adjustedRange, config);

        // 2. Desenha área com gradiente
        drawArea(ctx, values, getX, getY, h, padding, config);

        // 3. Desenha linha principal
        drawLine(ctx, values, getX, getY, config);

        // 4. Desenha pontos
        drawPoints(ctx, values, labels, getX, getY, h, config);
    }

    function drawGrid(ctx, w, h, padding, chartH, upperLimit, adjustedRange, config) {
        ctx.strokeStyle = config.colors.grid;
        ctx.lineWidth = 1;
        ctx.beginPath();

        for (let i = 0; i <= config.gridLines; i++) {
            const y = padding.top + (chartH / config.gridLines) * i;

            // Linha horizontal
            ctx.moveTo(padding.left, y);
            ctx.lineTo(w - padding.right, y);

            // Label do valor
            const val = upperLimit - (adjustedRange / config.gridLines) * i;
            ctx.fillStyle = config.colors.text;
            ctx.font = '11px Inter, sans-serif';
            ctx.textAlign = 'right';
            ctx.textBaseline = 'middle';
            ctx.fillText(
                val.toLocaleString('pt-BR', {
                    style: 'currency',
                    currency: 'BRL',
                    maximumFractionDigits: 0
                }),
                padding.left - 10,
                y
            );
        }

        ctx.stroke();
    }

    function drawArea(ctx, values, getX, getY, h, padding, config) {
        const gradient = ctx.createLinearGradient(0, padding.top, 0, h);
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
        ctx.beginPath();
        ctx.moveTo(getX(0), getY(values[0]));

        for (let i = 1; i < values.length; i++) {
            ctx.lineTo(getX(i), getY(values[i]));
        }

        ctx.stroke();
    }

    function drawPoints(ctx, values, labels, getX, getY, h, config) {
        for (let i = 0; i < values.length; i++) {
            const x = getX(i);
            const y = getY(values[i]);

            // Ponto (círculo com borda)
            ctx.fillStyle = config.colors.pointFill;
            ctx.beginPath();
            ctx.arc(x, y, config.pointRadius, 0, Math.PI * 2);
            ctx.fill();

            ctx.strokeStyle = config.colors.primary;
            ctx.lineWidth = 2;
            ctx.stroke();

            // Label do mês
            ctx.fillStyle = config.colors.text;
            ctx.font = '12px Inter, sans-serif';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'top';
            ctx.fillText(labels[i], x, h - 20);
        }
    }

    /* Animações dos KPI Cards
       ======================================================================== */
    function animateKpiCards() {
        const kpiCards = document.querySelectorAll('.kpi-card');

        if ('IntersectionObserver' in window && kpiCards.length > 0) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((entry, index) => {
                    if (entry.isIntersecting) {
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

            kpiCards.forEach(card => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
                observer.observe(card);
            });
        }
    }

    /* Utilitários
       ======================================================================== */
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    /* Smooth Scroll para Links Internos
       ======================================================================== */
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

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

    /* Expor função global se necessário (para compatibilidade)
       ======================================================================== */
    window.toggleInfoSection = toggleInfoSection;

})();