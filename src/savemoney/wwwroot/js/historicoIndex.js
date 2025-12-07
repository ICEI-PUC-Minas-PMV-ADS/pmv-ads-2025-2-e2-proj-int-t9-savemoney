/* ============================================================================
   HISTÓRICO FINANCEIRO - INDEX.JS
   Funcionalidades: Gráfico Canvas Nativo, Toggle View, Tooltip, Animações
   Autor: SaveMoney Team
   ============================================================================ */

// =============================================================================
// ESTADO GLOBAL DA APLICAÇÃO
// =============================================================================
const state = {
    chart: {
        canvas: null,
        ctx: null,
        data: {
            labels: [],
            receitas: [],
            despesas: [],
            saldos: []
        },
        dimensions: {
            width: 0,
            height: 0,
            padding: { top: 40, right: 40, bottom: 60, left: 80 }
        },
        scales: {
            min: 0,
            max: 0,
            stepY: 0
        },
        currentView: 'saldo', // 'saldo' ou 'comparativo'
        hoveredPoint: null,
        dpr: window.devicePixelRatio || 1
    },
    tooltip: {
        element: null,
        isVisible: false
    },
    filters: {
        autoSubmitTimer: null
    },
    animations: {
        observers: []
    }
};

// =============================================================================
// INICIALIZAÇÃO
// =============================================================================
document.addEventListener('DOMContentLoaded', () => {
    console.log('🚀 Histórico Financeiro - Inicializando...');

    initializeChart();
    initializeFilters();
    initializeAnimations();
    initializeTooltip();

    console.log('✅ Histórico Financeiro - Carregado com sucesso!');
});

// =============================================================================
// GRÁFICO CANVAS - CONFIGURAÇÃO E RENDERIZAÇÃO
// =============================================================================

/**
 * Inicializa o gráfico canvas
 */
function initializeChart() {
    const canvas = document.getElementById('historicoCanvas');
    if (!canvas) {
        console.warn('Canvas não encontrado - pulando inicialização do gráfico');
        return;
    }

    state.chart.canvas = canvas;
    state.chart.ctx = canvas.getContext('2d');

    // Carrega dados dos data attributes
    try {
        state.chart.data.labels = JSON.parse(canvas.dataset.labels || '[]');
        state.chart.data.receitas = JSON.parse(canvas.dataset.receitas || '[]');
        state.chart.data.despesas = JSON.parse(canvas.dataset.despesas || '[]');
        state.chart.data.saldos = JSON.parse(canvas.dataset.saldos || '[]');
    } catch (error) {
        console.error('Erro ao carregar dados do gráfico:', error);
        return;
    }

    // Se não há dados, não renderiza
    if (state.chart.data.labels.length === 0) {
        console.warn('Sem dados para renderizar gráfico');
        return;
    }

    // Configura event listeners
    setupChartEventListeners();

    // Renderiza pela primeira vez
    resizeCanvas();
    drawChart();

    console.log('📊 Gráfico inicializado com sucesso');
}

/**
 * Configura os event listeners do gráfico
 */
function setupChartEventListeners() {
    const canvas = state.chart.canvas;

    // Toggle de visualização
    const toggleButtons = document.querySelectorAll('.toggle-btn');
    toggleButtons.forEach(btn => {
        btn.addEventListener('click', handleViewToggle);
    });

    // Mouse tracking para tooltip
    canvas.addEventListener('mousemove', handleCanvasMouseMove);
    canvas.addEventListener('mouseleave', handleCanvasMouseLeave);

    // Resize com debounce
    let resizeTimer;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            resizeCanvas();
            drawChart();
        }, 250);
    });
}

/**
 * Redimensiona o canvas considerando DPR para displays high-DPI
 */
function resizeCanvas() {
    const canvas = state.chart.canvas;
    const container = canvas.parentElement;
    const dpr = state.chart.dpr;

    // Dimensões CSS
    const rect = container.getBoundingClientRect();
    const width = rect.width;
    const height = rect.height;

    // Dimensões canvas (considerando DPR)
    canvas.width = width * dpr;
    canvas.height = height * dpr;

    // Dimensões de desenho
    state.chart.dimensions.width = width * dpr;
    state.chart.dimensions.height = height * dpr;

    // Escala o contexto
    state.chart.ctx.scale(dpr, dpr);

    // Atualiza padding proporcional
    const basePadding = 40;
    state.chart.dimensions.padding = {
        top: basePadding,
        right: basePadding,
        bottom: basePadding * 1.5,
        left: basePadding * 2
    };
}

/**
 * Desenha o gráfico completo
 */
function drawChart() {
    const ctx = state.chart.ctx;
    const { width, height } = state.chart.dimensions;
    const dpr = state.chart.dpr;

    // Limpa o canvas
    ctx.clearRect(0, 0, width / dpr, height / dpr);

    // Calcula escalas
    calculateScales();

    // Desenha componentes
    drawGrid();
    drawAxes();

    if (state.chart.currentView === 'saldo') {
        drawSaldoChart();
    } else {
        drawComparativoChart();
    }

    // Desenha pontos interativos
    drawPoints();

    // Se houver ponto hovereado, desenha linha vertical
    if (state.chart.hoveredPoint !== null) {
        drawHoverLine(state.chart.hoveredPoint);
    }
}

/**
 * Calcula as escalas min/max e step do eixo Y
 */
function calculateScales() {
    const { receitas, despesas, saldos } = state.chart.data;

    let allValues = [];

    if (state.chart.currentView === 'saldo') {
        allValues = [...saldos];
    } else {
        allValues = [...receitas, ...despesas, ...saldos];
    }

    // Adiciona um buffer de 10%
    const max = Math.max(...allValues, 0);
    const min = Math.min(...allValues, 0);

    const range = max - min;
    const buffer = range * 0.1;

    state.chart.scales.max = max + buffer;
    state.chart.scales.min = min - buffer;

    // Calcula step (5 linhas de grid)
    const totalRange = state.chart.scales.max - state.chart.scales.min;
    state.chart.scales.stepY = totalRange / 5;
}

/**
 * Desenha o grid de fundo
 */
function drawGrid() {
    const ctx = state.chart.ctx;
    const { width, height, padding } = state.chart.dimensions;
    const dpr = state.chart.dpr;

    const chartWidth = width / dpr - padding.left - padding.right;
    const chartHeight = height / dpr - padding.top - padding.bottom;

    ctx.save();
    ctx.strokeStyle = 'rgba(255, 255, 255, 0.05)';
    ctx.lineWidth = 1;
    ctx.setLineDash([5, 5]);

    // Linhas horizontais (5 linhas)
    for (let i = 0; i <= 5; i++) {
        const y = padding.top + (chartHeight / 5) * i;
        ctx.beginPath();
        ctx.moveTo(padding.left, y);
        ctx.lineTo(padding.left + chartWidth, y);
        ctx.stroke();
    }

    // Linhas verticais (uma para cada mês)
    const stepX = chartWidth / (state.chart.data.labels.length - 1);
    for (let i = 0; i < state.chart.data.labels.length; i++) {
        const x = padding.left + stepX * i;
        ctx.beginPath();
        ctx.moveTo(x, padding.top);
        ctx.lineTo(x, padding.top + chartHeight);
        ctx.stroke();
    }

    ctx.restore();
}

/**
 * Desenha os eixos X e Y com labels
 */
function drawAxes() {
    const ctx = state.chart.ctx;
    const { width, height, padding } = state.chart.dimensions;
    const dpr = state.chart.dpr;

    const chartWidth = width / dpr - padding.left - padding.right;
    const chartHeight = height / dpr - padding.top - padding.bottom;

    ctx.save();

    // Eixos principais
    ctx.strokeStyle = 'rgba(255, 255, 255, 0.2)';
    ctx.lineWidth = 2;

    // Eixo Y
    ctx.beginPath();
    ctx.moveTo(padding.left, padding.top);
    ctx.lineTo(padding.left, padding.top + chartHeight);
    ctx.stroke();

    // Eixo X
    ctx.beginPath();
    ctx.moveTo(padding.left, padding.top + chartHeight);
    ctx.lineTo(padding.left + chartWidth, padding.top + chartHeight);
    ctx.stroke();

    // Labels do eixo Y
    ctx.fillStyle = 'rgba(255, 255, 255, 0.7)';
    ctx.font = '12px Inter, system-ui, sans-serif';
    ctx.textAlign = 'right';
    ctx.textBaseline = 'middle';

    for (let i = 0; i <= 5; i++) {
        const value = state.chart.scales.max - (state.chart.scales.stepY * i);
        const y = padding.top + (chartHeight / 5) * i;
        const label = formatCurrencyShort(value);
        ctx.fillText(label, padding.left - 10, y);
    }

    // Labels do eixo X
    ctx.textAlign = 'center';
    ctx.textBaseline = 'top';
    ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
    ctx.font = '600 13px Inter, system-ui, sans-serif';

    const stepX = chartWidth / (state.chart.data.labels.length - 1);
    state.chart.data.labels.forEach((label, i) => {
        const x = padding.left + stepX * i;
        ctx.fillText(label, x, padding.top + chartHeight + 10);
    });

    ctx.restore();
}

/**
 * Desenha o gráfico no modo "Saldo" (área preenchida)
 */
function drawSaldoChart() {
    const ctx = state.chart.ctx;
    const { saldos } = state.chart.data;

    const points = calculatePoints(saldos);

    if (points.length < 2) return;

    // Desenha área preenchida
    const gradient = ctx.createLinearGradient(0, state.chart.dimensions.padding.top, 0, state.chart.dimensions.height / state.chart.dpr - state.chart.dimensions.padding.bottom);
    gradient.addColorStop(0, 'rgba(147, 51, 234, 0.3)'); // Purple
    gradient.addColorStop(1, 'rgba(147, 51, 234, 0.05)');

    ctx.save();
    ctx.fillStyle = gradient;
    ctx.beginPath();
    ctx.moveTo(points[0].x, state.chart.dimensions.height / state.chart.dpr - state.chart.dimensions.padding.bottom);

    points.forEach(point => {
        ctx.lineTo(point.x, point.y);
    });

    ctx.lineTo(points[points.length - 1].x, state.chart.dimensions.height / state.chart.dpr - state.chart.dimensions.padding.bottom);
    ctx.closePath();
    ctx.fill();

    // Desenha linha
    ctx.strokeStyle = '#9333ea'; // Purple fixo
    ctx.lineWidth = 3;
    ctx.shadowColor = 'rgba(147, 51, 234, 0.5)';
    ctx.shadowBlur = 10;

    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y);
    points.forEach(point => {
        ctx.lineTo(point.x, point.y);
    });
    ctx.stroke();

    ctx.restore();

    // Armazena pontos para interação
    state.chart.saldoPoints = points;
}

/**
 * Desenha o gráfico no modo "Comparativo" (3 linhas)
 */
function drawComparativoChart() {
    const ctx = state.chart.ctx;
    const { receitas, despesas, saldos } = state.chart.data;

    // Calcula pontos para cada dataset
    const receitasPoints = calculatePoints(receitas);
    const despesasPoints = calculatePoints(despesas);
    const saldosPoints = calculatePoints(saldos);

    // Desenha linha de Receitas (verde)
    drawLine(receitasPoints, '#10b981', 'rgba(16, 185, 129, 0.5)'); // success

    // Desenha linha de Despesas (vermelho)
    drawLine(despesasPoints, '#ef4444', 'rgba(239, 68, 68, 0.5)'); // danger

    // Desenha linha de Saldo (azul)
    drawLine(saldosPoints, '#3b82f6', 'rgba(59, 130, 246, 0.5)'); // accent-primary

    // Armazena pontos para interação
    state.chart.receitasPoints = receitasPoints;
    state.chart.despesasPoints = despesasPoints;
    state.chart.saldosPoints = saldosPoints;
}

/**
 * Desenha uma linha no gráfico
 */
function drawLine(points, color, shadowColor) {
    if (points.length < 2) return;

    const ctx = state.chart.ctx;

    ctx.save();
    ctx.strokeStyle = color;
    ctx.lineWidth = 2.5;
    ctx.shadowColor = shadowColor;
    ctx.shadowBlur = 8;
    ctx.lineJoin = 'round';
    ctx.lineCap = 'round';

    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y);
    points.forEach(point => {
        ctx.lineTo(point.x, point.y);
    });
    ctx.stroke();

    ctx.restore();
}

/**
 * Calcula as coordenadas (x, y) dos pontos no canvas
 */
function calculatePoints(values) {
    const { width, height, padding } = state.chart.dimensions;
    const dpr = state.chart.dpr;

    const chartWidth = width / dpr - padding.left - padding.right;
    const chartHeight = height / dpr - padding.top - padding.bottom;

    const stepX = chartWidth / (values.length - 1);

    return values.map((value, i) => {
        const x = padding.left + stepX * i;
        const normalizedValue = (value - state.chart.scales.min) / (state.chart.scales.max - state.chart.scales.min);
        const y = padding.top + chartHeight - (normalizedValue * chartHeight);

        return { x, y, value, index: i };
    });
}

/**
 * Desenha os pontos interativos
 */
function drawPoints() {
    const ctx = state.chart.ctx;

    const pointsToRender = state.chart.currentView === 'saldo'
        ? [{ points: state.chart.saldoPoints, color: '#9333ea' }]
        : [
            { points: state.chart.receitasPoints, color: '#10b981' },
            { points: state.chart.despesasPoints, color: '#ef4444' },
            { points: state.chart.saldosPoints, color: '#3b82f6' }
        ];

    pointsToRender.forEach(({ points, color }) => {
        if (!points) return;

        points.forEach((point, i) => {
            const isHovered = state.chart.hoveredPoint === i;
            const radius = isHovered ? 8 : 5;

            ctx.save();
            ctx.fillStyle = color;

            if (isHovered) {
                ctx.shadowColor = color;
                ctx.shadowBlur = 15;
            }

            ctx.beginPath();
            ctx.arc(point.x, point.y, radius, 0, Math.PI * 2);
            ctx.fill();

            // Borda branca
            ctx.strokeStyle = '#fff';
            ctx.lineWidth = 2;
            ctx.stroke();

            ctx.restore();
        });
    });
}

/**
 * Desenha a linha vertical no hover
 */
function drawHoverLine(index) {
    const ctx = state.chart.ctx;
    const { height, padding } = state.chart.dimensions;
    const dpr = state.chart.dpr;

    const chartHeight = height / dpr - padding.top - padding.bottom;

    // Pega o X do ponto hovereado
    const points = state.chart.currentView === 'saldo'
        ? state.chart.saldoPoints
        : state.chart.saldosPoints;

    if (!points || !points[index]) return;

    const x = points[index].x;

    ctx.save();
    ctx.strokeStyle = 'rgba(147, 51, 234, 0.4)';
    ctx.lineWidth = 2;
    ctx.setLineDash([5, 5]);

    ctx.beginPath();
    ctx.moveTo(x, padding.top);
    ctx.lineTo(x, padding.top + chartHeight);
    ctx.stroke();

    ctx.restore();
}

/**
 * Handle do toggle entre visualizações
 */
function handleViewToggle(event) {
    const button = event.currentTarget;
    const view = button.dataset.view;

    if (view === state.chart.currentView) return;

    // Atualiza estado
    state.chart.currentView = view;

    // Atualiza UI dos botões
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.classList.remove('active');
        btn.setAttribute('aria-pressed', 'false');
    });
    button.classList.add('active');
    button.setAttribute('aria-pressed', 'true');

    // Anima transição
    animateChartTransition();
}

/**
 * Anima a transição entre visualizações
 */
function animateChartTransition() {
    const canvas = state.chart.canvas;

    // Fade out
    canvas.style.opacity = '0';

    setTimeout(() => {
        // Redesenha
        drawChart();

        // Fade in
        canvas.style.opacity = '1';
    }, 200);
}

/**
 * Handle do movimento do mouse sobre o canvas
 */
function handleCanvasMouseMove(event) {
    const canvas = state.chart.canvas;
    const rect = canvas.getBoundingClientRect();
    const mouseX = event.clientX - rect.left;
    const mouseY = event.clientY - rect.top;

    // Encontra o ponto mais próximo
    const points = state.chart.currentView === 'saldo'
        ? state.chart.saldoPoints
        : state.chart.saldosPoints;

    if (!points || points.length === 0) return;

    let closestPoint = null;
    let minDistance = Infinity;

    points.forEach((point, i) => {
        const distance = Math.sqrt(
            Math.pow(point.x - mouseX, 2) +
            Math.pow(point.y - mouseY, 2)
        );

        if (distance < minDistance && distance < 30) { // Raio de 30px
            minDistance = distance;
            closestPoint = i;
        }
    });

    // Se mudou o ponto hovereado
    if (closestPoint !== state.chart.hoveredPoint) {
        state.chart.hoveredPoint = closestPoint;

        if (closestPoint !== null) {
            canvas.style.cursor = 'pointer';
            showTooltip(closestPoint, event.clientX, event.clientY);
        } else {
            canvas.style.cursor = 'default';
            hideTooltip();
        }

        drawChart();
    } else if (closestPoint !== null) {
        // Atualiza posição do tooltip
        updateTooltipPosition(event.clientX, event.clientY);
    }
}

/**
 * Handle quando o mouse sai do canvas
 */
function handleCanvasMouseLeave() {
    state.chart.canvas.style.cursor = 'default';
    state.chart.hoveredPoint = null;
    hideTooltip();
    drawChart();
}

// =============================================================================
// TOOLTIP
// =============================================================================

/**
 * Inicializa o tooltip
 */
function initializeTooltip() {
    state.tooltip.element = document.getElementById('chartTooltip');
}

/**
 * Mostra o tooltip com os dados do ponto
 */
function showTooltip(index, mouseX, mouseY) {
    if (!state.tooltip.element) return;

    const { labels, receitas, despesas, saldos } = state.chart.data;

    // Popula dados
    const monthElement = state.tooltip.element.querySelector('.tooltip-month');
    monthElement.textContent = labels[index];

    const receitasValue = state.tooltip.element.querySelector('.tooltip-item-receitas .tooltip-value');
    receitasValue.textContent = formatCurrency(receitas[index]);

    const despesasValue = state.tooltip.element.querySelector('.tooltip-item-despesas .tooltip-value');
    despesasValue.textContent = formatCurrency(despesas[index]);

    const saldoValue = state.tooltip.element.querySelector('.tooltip-item-saldo .tooltip-value');
    saldoValue.textContent = formatCurrency(saldos[index]);

    // Mostra primeiro (para calcular dimensões)
    state.tooltip.element.classList.add('active');
    state.tooltip.element.setAttribute('aria-hidden', 'false');
    state.tooltip.isVisible = true;

    // Posiciona
    updateTooltipPosition(mouseX, mouseY);
}

/**
 * Atualiza a posição do tooltip
 * ✅ CORRIGIDO - Usa coordenadas absolutas da tela
 */
/**
 * Atualiza a posição do tooltip
 * ✅ CORRIGIDO - Tooltip sempre visível e próximo do cursor
 */
function updateTooltipPosition(clientX, clientY) {
    if (!state.tooltip.element || !state.tooltip.isVisible) return;

    const tooltip = state.tooltip.element;
    const tooltipRect = tooltip.getBoundingClientRect();

    const offset = 15; // ✅ Distância do cursor (bem próximo)
    const margin = 10; // ✅ Margem da borda da tela
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    // Posição padrão: à direita e ligeiramente abaixo do cursor
    let left = clientX + offset;
    let top = clientY + offset;

    // ✅ AJUSTE HORIZONTAL
    // Se sair pela direita, coloca à esquerda do cursor
    if (left + tooltipRect.width > viewportWidth - margin) {
        left = clientX - tooltipRect.width - offset;
    }

    // Se ainda sair pela esquerda, alinha com margem esquerda
    if (left < margin) {
        left = margin;
    }

    // ✅ AJUSTE VERTICAL
    // Se sair por baixo, coloca acima do cursor
    if (top + tooltipRect.height > viewportHeight - margin) {
        top = clientY - tooltipRect.height - offset;
    }

    // Se sair por cima, alinha com margem superior
    if (top < margin) {
        top = margin;
    }

    tooltip.style.left = `${left}px`;
    tooltip.style.top = `${top}px`;
}

/**
 * Esconde o tooltip
 */
function hideTooltip() {
    if (!state.tooltip.element) return;

    state.tooltip.element.classList.remove('active');
    state.tooltip.element.setAttribute('aria-hidden', 'true');
    state.tooltip.isVisible = false;
}

// =============================================================================
// FILTROS
// =============================================================================

/**
 * Inicializa os filtros
 */
function initializeFilters() {
    const form = document.getElementById('formFiltros');
    if (!form) return;

    const mesSelect = document.getElementById('mes');
    const anoInput = document.getElementById('ano');
    const tipoSelect = document.getElementById('tipo');
    const buscaInput = document.getElementById('busca');

    // Auto-submit nos selects
    if (mesSelect) {
        mesSelect.addEventListener('change', () => {
            form.submit();
        });
    }

    if (anoInput) {
        anoInput.addEventListener('change', () => {
            form.submit();
        });
    }

    if (tipoSelect) {
        tipoSelect.addEventListener('change', () => {
            form.submit();
        });
    }

    // Debounce na busca (aguarda 800ms após parar de digitar)
    if (buscaInput) {
        buscaInput.addEventListener('input', () => {
            clearTimeout(state.filters.autoSubmitTimer);

            state.filters.autoSubmitTimer = setTimeout(() => {
                form.submit();
            }, 800);
        });
    }

    console.log('🔧 Filtros inicializados');
}

// =============================================================================
// ANIMAÇÕES
// =============================================================================

/**
 * Inicializa as animações de entrada
 */
function initializeAnimations() {
    animateKPICards();
    animateTimelineItems();
}

/**
 * Anima os KPI cards com counter
 */
function animateKPICards() {
    const kpiCards = document.querySelectorAll('.kpi-card');

    kpiCards.forEach((card, index) => {
        // Stagger effect
        card.style.animationDelay = `${index * 100}ms`;

        // Counter animation nos valores
        const valueElement = card.querySelector('.kpi-value');
        if (valueElement) {
            const targetText = valueElement.textContent;
            const targetValue = parseFloat(targetText.replace(/[^\d,-]/g, '').replace(',', '.'));

            if (!isNaN(targetValue)) {
                animateCounter(valueElement, 0, targetValue, 1500, targetText.includes('R$'));
            }
        }
    });
}

/**
 * Anima contador de um valor
 */
function animateCounter(element, start, end, duration, isCurrency = false) {
    const startTime = performance.now();
    const isNegative = end < 0;
    const absoluteEnd = Math.abs(end);

    function update(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);

        // Easing: easeOutCubic
        const eased = 1 - Math.pow(1 - progress, 3);

        const current = start + (absoluteEnd - start) * eased;

        if (isCurrency) {
            const formatted = current.toLocaleString('pt-BR', {
                style: 'currency',
                currency: 'BRL'
            });
            element.textContent = isNegative ? '-' + formatted : formatted;
        } else {
            element.textContent = Math.round(current).toLocaleString('pt-BR');
        }

        if (progress < 1) {
            requestAnimationFrame(update);
        } else {
            // Garante o valor final exato
            if (isCurrency) {
                const formatted = absoluteEnd.toLocaleString('pt-BR', {
                    style: 'currency',
                    currency: 'BRL'
                });
                element.textContent = isNegative ? '-' + formatted : formatted;
            }
        }
    }

    requestAnimationFrame(update);
}

/**
 * Anima os itens da timeline com IntersectionObserver
 */
function animateTimelineItems() {
    const timelineItems = document.querySelectorAll('.timeline-item');

    if (timelineItems.length === 0) return;

    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry, index) => {
                if (entry.isIntersecting) {
                    setTimeout(() => {
                        entry.target.style.opacity = '1';
                        entry.target.style.transform = 'translateY(0)';
                    }, index * 80); // Stagger de 80ms

                    observer.unobserve(entry.target);
                }
            });
        },
        {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        }
    );

    timelineItems.forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(20px)';
        item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(item);
    });

    state.animations.observers.push(observer);
}

// =============================================================================
// UTILITÁRIOS
// =============================================================================

/**
 * Formata valores monetários de forma abreviada (K, M)
 */
function formatCurrencyShort(value) {
    const absValue = Math.abs(value);
    const sign = value < 0 ? '-' : '';

    if (absValue >= 1000000) {
        return sign + 'R$ ' + (absValue / 1000000).toFixed(1) + 'M';
    } else if (absValue >= 1000) {
        return sign + 'R$ ' + (absValue / 1000).toFixed(1) + 'K';
    } else {
        return sign + 'R$ ' + absValue.toFixed(0);
    }
}

/**
 * Formata valores monetários completos
 */
function formatCurrency(value) {
    return value.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
}

/**
 * Debounce genérico
 */
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

// =============================================================================
// CLEANUP
// =============================================================================

/**
 * Limpa recursos quando a página é descarregada
 */
window.addEventListener('beforeunload', () => {
    // Limpa observers
    state.animations.observers.forEach(observer => observer.disconnect());

    // Limpa timers
    if (state.filters.autoSubmitTimer) {
        clearTimeout(state.filters.autoSubmitTimer);
    }

    console.log('🧹 Recursos limpos');
});