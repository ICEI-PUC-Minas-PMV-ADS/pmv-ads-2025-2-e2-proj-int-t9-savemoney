// TendenciaFinanceira-Resultado.js
// Sistema de Análise de Tendências Financeiras - Página Resultado
// Vanilla JS - SEM BIBLIOTECAS

document.addEventListener('DOMContentLoaded', function () {
    console.log('📊 Resultado - Tendência Financeira carregado');

    // Inicializar módulos
    inicializarGraficoNativo();
    inicializarAnimacoes();
});

// ============================================
// GRÁFICO CANVAS NATIVO - SEM BIBLIOTECAS
// ============================================

let graficoState = null;

function inicializarGraficoNativo() {
    const canvas = document.getElementById('graficoTendencia');

    if (!canvas) {
        console.error('❌ Canvas do gráfico não encontrado');
        return;
    }

    // Buscar dados dos data attributes
    const labelsJson = canvas.getAttribute('data-labels');
    const receitasJson = canvas.getAttribute('data-receitas');
    const despesasJson = canvas.getAttribute('data-despesas');
    const saldosJson = canvas.getAttribute('data-saldos');

    if (!labelsJson || !receitasJson || !despesasJson || !saldosJson) {
        console.error('❌ Dados do gráfico não encontrados nos data attributes');
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
        if (!dados.labels.length || !dados.receitas.length || !dados.despesas.length || !dados.saldos.length) {
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
        criarGraficoNativo(canvas, dados);

    } catch (error) {
        console.error('❌ Erro ao parsear dados do gráfico:', error);
    }
}

function criarGraficoNativo(canvas, dados) {
    const ctx = canvas.getContext('2d');
    const dpr = window.devicePixelRatio || 1;

    // Ajustar canvas para alta resolução
    const rect = canvas.getBoundingClientRect();
    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;
    ctx.scale(dpr, dpr);

    const width = rect.width;
    const height = rect.height;

    // Definir padding do gráfico
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
    const valueRange = maxValue - minValue || 1; // Evitar divisão por zero

    // Salvar estado do gráfico
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
    desenharGrafico();

    // Event listeners
    canvas.addEventListener('mousemove', handleMouseMove);
    canvas.addEventListener('mouseleave', handleMouseLeave);

    // Redimensionar com debounce
    let resizeTimeout;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            console.log('🔄 Redimensionando gráfico...');
            criarGraficoNativo(canvas, dados);
        }, 250);
    });

    console.log('✅ Gráfico Canvas nativo criado com sucesso');
}

function desenharGrafico() {
    if (!graficoState) return;

    const { ctx, width, height, padding, graphWidth, graphHeight, dados, maxValue, minValue, valueRange } = graficoState;

    // Limpar canvas
    ctx.clearRect(0, 0, width, height);

    // Desenhar grid horizontal
    desenharGrid();

    // Desenhar eixos e labels
    desenharEixos();

    // Desenhar linhas dos dados
    desenharLinha(dados.receitas, '#10b981', 'rgba(16, 185, 129, 0.1)', false);
    desenharLinha(dados.despesas, '#ef4444', 'rgba(239, 68, 68, 0.1)', false);
    desenharLinha(dados.saldos, '#3b82f6', 'rgba(59, 130, 246, 0.1)', true);

    // Desenhar legenda
    desenharLegenda();

    // Desenhar tooltip se hover
    if (graficoState.hoveredPoint !== null) {
        desenharTooltip();
    }
}

function desenharGrid() {
    const { ctx, padding, graphWidth, graphHeight } = graficoState;

    ctx.strokeStyle = 'rgba(255, 255, 255, 0.05)';
    ctx.lineWidth = 1;

    // Linhas horizontais
    for (let i = 0; i <= 5; i++) {
        const y = padding.top + (graphHeight / 5) * i;
        ctx.beginPath();
        ctx.moveTo(padding.left, y);
        ctx.lineTo(padding.left + graphWidth, y);
        ctx.stroke();
    }
}

function desenharEixos() {
    const { ctx, padding, graphWidth, graphHeight, dados, maxValue, minValue, valueRange } = graficoState;

    // Labels do eixo Y
    ctx.fillStyle = '#aaaaaa';
    ctx.font = '12px Inter, sans-serif';
    ctx.textAlign = 'right';
    ctx.textBaseline = 'middle';

    for (let i = 0; i <= 5; i++) {
        const value = maxValue - (valueRange / 5) * i;
        const y = padding.top + (graphHeight / 5) * i;
        ctx.fillText(formatarMoedaCurta(value), padding.left - 10, y);
    }

    // Labels do eixo X
    ctx.textAlign = 'center';
    ctx.textBaseline = 'top';

    const numLabels = dados.labels.length;
    const step = numLabels > 1 ? graphWidth / (numLabels - 1) : 0;

    dados.labels.forEach((label, i) => {
        const x = padding.left + step * i;
        ctx.fillText(label, x, padding.top + graphHeight + 10);
    });
}

function desenharLinha(values, color, fillColor, dashed) {
    const { ctx, padding, graphWidth, graphHeight, dados, minValue, valueRange } = graficoState;

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

    // Fechar área
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

        // Fundo branco
        ctx.fillStyle = '#fff';
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.fill();

        // Borda colorida
        ctx.strokeStyle = color;
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.stroke();
    });
}

function desenharLegenda() {
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

        // Círculo colorido
        ctx.fillStyle = item.color;
        ctx.beginPath();
        ctx.arc(x, y, 5, 0, Math.PI * 2);
        ctx.fill();

        // Texto
        ctx.fillStyle = '#f5f5ff';
        ctx.fillText(item.label, x + 12, y);
    });
}

function desenharTooltip() {
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

    // Fundo do tooltip
    ctx.fillStyle = 'rgba(27, 29, 41, 0.95)';
    ctx.strokeStyle = 'rgba(59, 130, 246, 0.3)';
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

        // Círculo colorido
        ctx.fillStyle = item.color;
        ctx.beginPath();
        ctx.arc(tooltipX + 15, yPos + 5, 4, 0, Math.PI * 2);
        ctx.fill();

        // Texto
        ctx.fillStyle = '#aaaaaa';
        ctx.fillText(`${item.label}: ${formatarMoeda(item.value)}`, tooltipX + 28, yPos);
    });
}

function handleMouseMove(e) {
    if (!graficoState) return;

    const { canvas, padding, graphWidth, graphHeight, dados } = graficoState;
    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    // Verificar se está dentro da área do gráfico
    if (x < padding.left || x > padding.left + graphWidth ||
        y < padding.top || y > padding.top + graphHeight) {
        graficoState.hoveredPoint = null;
        desenharGrafico();
        return;
    }

    // Calcular índice do ponto mais próximo
    const numPoints = dados.labels.length;
    const step = numPoints > 1 ? graphWidth / (numPoints - 1) : 0;
    const relativeX = x - padding.left;
    const index = Math.round(relativeX / step);

    if (index >= 0 && index < dados.labels.length) {
        graficoState.hoveredPoint = { index };
        desenharGrafico();
    }
}

function handleMouseLeave() {
    if (!graficoState) return;

    graficoState.hoveredPoint = null;
    desenharGrafico();
}

// ============================================
// ANIMAÇÕES E INTERATIVIDADE
// ============================================

function inicializarAnimacoes() {
    // Animar cards ao carregar
    animarCards();

    // Animar números (counter animation)
    animarNumeros();

    // Adicionar efeitos hover
    adicionarEfeitosHover();

    // Animar badges
    animarBadges();

    console.log('✅ Animações inicializadas');
}

function animarCards() {
    const cards = document.querySelectorAll('.metric-card, .glass-card, .summary-banner');

    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';

        setTimeout(() => {
            card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });
}

function animarNumeros() {
    const numeros = document.querySelectorAll('.metric-value, .summary-stat p');

    numeros.forEach(elemento => {
        const textoOriginal = elemento.textContent.trim();

        // Extrair número do texto
        const numeroMatch = textoOriginal.match(/[\d,.]+/);
        if (!numeroMatch) return;

        const numeroStr = numeroMatch[0].replace(/\./g, '').replace(',', '.');
        const valorNumerico = parseFloat(numeroStr);

        if (isNaN(valorNumerico)) return;

        animarContador(elemento, 0, valorNumerico, 1500, textoOriginal);
    });
}

function animarContador(elemento, inicio, fim, duracao, formatoOriginal) {
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

        // Formatar conforme original
        if (formatoOriginal.includes('R$')) {
            elemento.textContent = formatarMoeda(atual);
        } else if (formatoOriginal.includes('%')) {
            elemento.textContent = atual.toFixed(1) + '%';
        } else {
            elemento.textContent = Math.round(atual).toLocaleString('pt-BR');
        }
    }, intervalo);
}

function adicionarEfeitosHover() {
    const cards = document.querySelectorAll('.metric-card, .glass-card');

    cards.forEach(card => {
        card.style.transition = 'transform 0.3s ease, box-shadow 0.3s ease';

        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-5px)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    });
}

function animarBadges() {
    const badges = document.querySelectorAll('.badge-custom');

    badges.forEach((badge, index) => {
        setTimeout(() => {
            badge.style.animation = 'pulse 0.6s ease-out';
        }, 600 + (index * 150));
    });
}

// ============================================
// UTILITÁRIOS - FORMATAÇÃO
// ============================================

function formatarMoeda(valor) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(valor);
}

function formatarMoedaCurta(valor) {
    const absValor = Math.abs(valor);

    if (absValor >= 1000000) {
        return (valor >= 0 ? 'R$ ' : '-R$ ') + (absValor / 1000000).toFixed(1) + 'M';
    } else if (absValor >= 1000) {
        return (valor >= 0 ? 'R$ ' : '-R$ ') + (absValor / 1000).toFixed(1) + 'K';
    }

    return (valor >= 0 ? 'R$ ' : '-R$ ') + absValor.toFixed(0);
}

// ============================================
// ANIMAÇÕES CSS
// ============================================

if (!document.getElementById('tendencia-resultado-animations')) {
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

// ============================================
// EXPORT PARA DEBUGGING
// ============================================

window.TendenciaFinanceiraResultado = {
    graficoState: () => graficoState,
    redesenhar: () => desenharGrafico()
};

console.log('✅ Módulo TendenciaFinanceira-Resultado carregado e pronto');