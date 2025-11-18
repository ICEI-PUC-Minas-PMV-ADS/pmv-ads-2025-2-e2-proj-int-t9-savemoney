// TendenciaFinanceira.js - VERSÃO SEM BIBLIOTECAS (Vanilla JS)

// ============================================
// 1. INICIALIZAÇÃO E DETECÇÃO DE PÁGINA
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('📊 Tendência Financeira JS carregado');

    const isIndexPage = document.getElementById('meses') !== null;
    const isResultadoPage = document.getElementById('graficoTendencia') !== null;

    if (isIndexPage) {
        console.log('✅ Página Index detectada');
        inicializarFormulario();
    }

    if (isResultadoPage) {
        console.log('✅ Página Resultado detectada');
        inicializarGrafico();
        inicializarAnimacoes();
    }
});

// ============================================
// 2. FORMULÁRIO - VALIDAÇÕES E INTERATIVIDADE
// ============================================

function inicializarFormulario() {
    const form = document.querySelector('form[action*="GerarAnalise"]');
    const btnGerar = document.querySelector('.btn-tendencia');
    const selectMeses = document.getElementById('meses');

    if (!form || !selectMeses) {
        console.error('❌ Formulário ou select de meses não encontrado');
        return;
    }

    selectMeses.addEventListener('change', function () {
        this.classList.remove('is-invalid');

        if (this.value) {
            this.classList.add('is-valid');
            mostrarInfoPeriodo(parseInt(this.value));
        } else {
            this.classList.remove('is-valid');
            esconderInfoPeriodo();
        }
    });

    form.addEventListener('submit', function (e) {
        if (!validarFormulario()) {
            e.preventDefault();
            return false;
        }

        if (btnGerar) {
            btnGerar.disabled = true;
            btnGerar.innerHTML = '<i class="bi bi-hourglass-split me-2"></i> Analisando...';
        }
    });

    console.log('✅ Formulário inicializado com validações');
}

function validarFormulario() {
    const selectMeses = document.getElementById('meses');
    selectMeses.classList.remove('is-invalid', 'is-valid');

    if (!selectMeses.value || selectMeses.value === '') {
        selectMeses.classList.add('is-invalid');
        mostrarErro('Por favor, selecione um período de análise');
        selectMeses.focus();
        return false;
    }

    const meses = parseInt(selectMeses.value);

    if (meses < 1 || meses > 12) {
        selectMeses.classList.add('is-invalid');
        mostrarErro('O período deve estar entre 1 e 12 meses');
        return false;
    }

    selectMeses.classList.add('is-valid');
    console.log('✅ Formulário válido - Período:', meses, 'meses');
    return true;
}

function mostrarErro(mensagem) {
    let alertErro = document.querySelector('.alert-erro-validacao');

    if (!alertErro) {
        alertErro = document.createElement('div');
        alertErro.className = 'alert-custom alert-danger mt-3 alert-erro-validacao';
        alertErro.innerHTML = `
            <i class="bi bi-exclamation-triangle me-2"></i>
            <span>${mensagem}</span>
        `;

        const form = document.querySelector('form[action*="GerarAnalise"]');
        const formParent = form.parentElement;
        formParent.appendChild(alertErro);
    } else {
        alertErro.querySelector('span').textContent = mensagem;
    }

    alertErro.classList.add('animate-shake');
    setTimeout(() => {
        alertErro.classList.remove('animate-shake');
    }, 500);
}

function mostrarInfoPeriodo(meses) {
    esconderInfoPeriodo();

    const infoDiv = document.createElement('div');
    infoDiv.className = 'alert-custom alert-info mt-3 alert-info-periodo';
    infoDiv.style.animation = 'fade-in-up 0.3s ease-out';

    let icone = '📊';
    let texto = '';

    if (meses === 1) {
        icone = '🗓️';
        texto = 'Análise de <strong>1 mês</strong> - Ideal para verificar mudanças recentes';
    } else if (meses <= 3) {
        icone = '📅';
        texto = `Análise de <strong>${meses} meses</strong> - Período recomendado para identificar tendências`;
    } else if (meses <= 6) {
        icone = '📊';
        texto = `Análise de <strong>${meses} meses</strong> - Ótimo para análises de médio prazo`;
    } else {
        icone = '📈';
        texto = `Análise de <strong>${meses} meses</strong> - Ideal para identificar padrões anuais`;
    }

    infoDiv.innerHTML = `
        <i class="bi bi-info-circle me-2"></i>
        <span>${icone} ${texto}</span>
    `;

    const form = document.querySelector('form[action*="GerarAnalise"]');
    const formParent = form.parentElement;
    formParent.appendChild(infoDiv);
}

function esconderInfoPeriodo() {
    const infoExistente = document.querySelector('.alert-info-periodo');
    if (infoExistente) {
        infoExistente.remove();
    }

    const erroExistente = document.querySelector('.alert-erro-validacao');
    if (erroExistente) {
        erroExistente.remove();
    }
}

// ============================================
// 3. GRÁFICO CANVAS NATIVO - SEM BIBLIOTECAS
// ============================================

let graficoState = null;

function inicializarGrafico() {
    const canvas = document.getElementById('graficoTendencia');

    if (!canvas) {
        console.error('❌ Canvas do gráfico não encontrado');
        return;
    }

    const labelsJson = canvas.getAttribute('data-labels');
    const receitasJson = canvas.getAttribute('data-receitas');
    const despesasJson = canvas.getAttribute('data-despesas');
    const saldosJson = canvas.getAttribute('data-saldos');

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

        console.log('📊 Dados do gráfico:', dados);
        criarGraficoNativo(canvas, dados);

    } catch (error) {
        console.error('❌ Erro ao parsear dados do gráfico:', error);
    }
}

function criarGraficoNativo(canvas, dados) {
    const ctx = canvas.getContext('2d');
    const dpr = window.devicePixelRatio || 1;

    const rect = canvas.getBoundingClientRect();
    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;
    ctx.scale(dpr, dpr);

    const width = rect.width;
    const height = rect.height;

    const padding = { top: 60, right: 40, bottom: 60, left: 80 };
    const graphWidth = width - padding.left - padding.right;
    const graphHeight = height - padding.top - padding.bottom;

    const allValues = [...dados.receitas, ...dados.despesas, ...dados.saldos];
    const maxValue = Math.max(...allValues);
    const minValue = Math.min(...allValues, 0);
    const valueRange = maxValue - minValue;

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

    desenharGrafico();

    canvas.addEventListener('mousemove', handleMouseMove);
    canvas.addEventListener('mouseleave', handleMouseLeave);
}

function desenharGrafico() {
    const { ctx, width, height, padding, graphWidth, graphHeight, dados, maxValue, minValue, valueRange } = graficoState;

    ctx.clearRect(0, 0, width, height);

    // Grid
    ctx.strokeStyle = 'rgba(255, 255, 255, 0.05)';
    ctx.lineWidth = 1;
    for (let i = 0; i <= 5; i++) {
        const y = padding.top + (graphHeight / 5) * i;
        ctx.beginPath();
        ctx.moveTo(padding.left, y);
        ctx.lineTo(padding.left + graphWidth, y);
        ctx.stroke();
    }

    // Eixo Y - Labels
    ctx.fillStyle = '#aaaaaa';
    ctx.font = '12px Inter, sans-serif';
    ctx.textAlign = 'right';
    ctx.textBaseline = 'middle';
    for (let i = 0; i <= 5; i++) {
        const value = maxValue - (valueRange / 5) * i;
        const y = padding.top + (graphHeight / 5) * i;
        ctx.fillText(formatarMoedaCurta(value), padding.left - 10, y);
    }

    // Eixo X - Labels
    ctx.textAlign = 'center';
    ctx.textBaseline = 'top';
    dados.labels.forEach((label, i) => {
        const x = padding.left + (graphWidth / (dados.labels.length - 1)) * i;
        ctx.fillText(label, x, padding.top + graphHeight + 10);
    });

    // Desenhar linhas e pontos
    desenharLinha(dados.receitas, '#10b981', 'rgba(16, 185, 129, 0.1)', false);
    desenharLinha(dados.despesas, '#ef4444', 'rgba(239, 68, 68, 0.1)', false);
    desenharLinha(dados.saldos, '#3b82f6', 'rgba(59, 130, 246, 0.1)', true);

    // Legenda
    desenharLegenda();

    // Tooltip
    if (graficoState.hoveredPoint) {
        desenharTooltip();
    }
}

function desenharLinha(values, color, fillColor, dashed) {
    const { ctx, padding, graphWidth, graphHeight, dados, maxValue, minValue, valueRange } = graficoState;

    // Área preenchida
    ctx.fillStyle = fillColor;
    ctx.beginPath();
    values.forEach((value, i) => {
        const x = padding.left + (graphWidth / (dados.labels.length - 1)) * i;
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
        const x = padding.left + (graphWidth / (dados.labels.length - 1)) * i;
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
        const x = padding.left + (graphWidth / (dados.labels.length - 1)) * i;
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

        ctx.fillStyle = item.color;
        ctx.beginPath();
        ctx.arc(x, y, 5, 0, Math.PI * 2);
        ctx.fill();

        ctx.fillStyle = '#f5f5ff';
        ctx.fillText(item.label, x + 12, y);
    });
}

function desenharTooltip() {
    const { ctx, padding, graphWidth, dados, maxValue, minValue, valueRange, hoveredPoint } = graficoState;

    if (!hoveredPoint) return;

    const i = hoveredPoint.index;
    const x = padding.left + (graphWidth / (dados.labels.length - 1)) * i;

    const tooltipData = [
        { label: 'Receitas', value: dados.receitas[i], color: '#10b981' },
        { label: 'Despesas', value: dados.despesas[i], color: '#ef4444' },
        { label: 'Saldo', value: dados.saldos[i], color: '#3b82f6' }
    ];

    const tooltipWidth = 180;
    const tooltipHeight = 100;
    const tooltipX = x > graficoState.width / 2 ? x - tooltipWidth - 15 : x + 15;
    const tooltipY = padding.top + 20;

    // Fundo do tooltip
    ctx.fillStyle = 'rgba(27, 29, 41, 0.95)';
    ctx.strokeStyle = 'rgba(59, 130, 246, 0.3)';
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.roundRect(tooltipX, tooltipY, tooltipWidth, tooltipHeight, 5);
    ctx.fill();
    ctx.stroke();

    // Título
    ctx.fillStyle = '#f5f5ff';
    ctx.font = 'bold 14px Inter, sans-serif';
    ctx.textAlign = 'left';
    ctx.textBaseline = 'top';
    ctx.fillText(dados.labels[i], tooltipX + 10, tooltipY + 10);

    // Valores
    ctx.font = '13px Inter, sans-serif';
    tooltipData.forEach((item, idx) => {
        const yPos = tooltipY + 35 + idx * 20;

        ctx.fillStyle = item.color;
        ctx.beginPath();
        ctx.arc(tooltipX + 15, yPos, 4, 0, Math.PI * 2);
        ctx.fill();

        ctx.fillStyle = '#aaaaaa';
        ctx.fillText(`${item.label}: ${formatarMoeda(item.value)}`, tooltipX + 25, yPos - 7);
    });
}

function handleMouseMove(e) {
    const { canvas, padding, graphWidth, graphHeight, dados } = graficoState;
    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    if (x < padding.left || x > padding.left + graphWidth ||
        y < padding.top || y > padding.top + graphHeight) {
        graficoState.hoveredPoint = null;
        desenharGrafico();
        return;
    }

    const pointWidth = graphWidth / (dados.labels.length - 1);
    const index = Math.round((x - padding.left) / pointWidth);

    if (index >= 0 && index < dados.labels.length) {
        graficoState.hoveredPoint = { index };
        desenharGrafico();
    }
}

function handleMouseLeave() {
    graficoState.hoveredPoint = null;
    desenharGrafico();
}

// ============================================
// 4. ANIMAÇÕES E INTERATIVIDADE
// ============================================

function inicializarAnimacoes() {
    const cards = document.querySelectorAll('.metric-card, .glass-card');

    cards.forEach((card, index) => {
        setTimeout(() => {
            card.classList.add('animate-fade-in');
        }, index * 100);
    });

    animarNumeros();
    adicionarEfeitosHover();
    animarBadges();

    console.log('✅ Animações inicializadas');
}

function animarNumeros() {
    const numeros = document.querySelectorAll('.metric-value, .summary-stat p');

    numeros.forEach(elemento => {
        const textoOriginal = elemento.textContent;
        const valorNumerico = parseFloat(textoOriginal.replace(/[^0-9,-]/g, '').replace(',', '.'));

        if (isNaN(valorNumerico)) return;

        animarContador(elemento, 0, valorNumerico, 1200, textoOriginal);
    });
}

function animarContador(elemento, inicio, fim, duracao, formatoOriginal) {
    const incremento = (fim - inicio) / (duracao / 16);
    let atual = inicio;

    const timer = setInterval(() => {
        atual += incremento;

        if ((incremento > 0 && atual >= fim) || (incremento < 0 && atual <= fim)) {
            atual = fim;
            clearInterval(timer);
        }

        if (formatoOriginal.includes('R$')) {
            elemento.textContent = formatarMoeda(atual);
        } else if (formatoOriginal.includes('%')) {
            elemento.textContent = atual.toFixed(1) + '%';
        } else {
            elemento.textContent = Math.round(atual).toString();
        }
    }, 16);
}

function adicionarEfeitosHover() {
    const cards = document.querySelectorAll('.metric-card, .glass-card');

    cards.forEach(card => {
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
            badge.style.animation = 'pulse 0.5s ease-out';
        }, 500 + (index * 100));
    });
}

// ============================================
// 5. UTILITÁRIOS - FORMATAÇÃO
// ============================================

function formatarMoeda(valor) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(valor);
}

function formatarMoedaCurta(valor) {
    if (Math.abs(valor) >= 1000000) {
        return 'R$ ' + (valor / 1000000).toFixed(1) + 'M';
    } else if (Math.abs(valor) >= 1000) {
        return 'R$ ' + (valor / 1000).toFixed(1) + 'K';
    }
    return 'R$ ' + valor.toFixed(0);
}

// ============================================
// 6. ANIMAÇÕES CSS ADICIONAIS
// ============================================

if (!document.getElementById('tendencia-animations')) {
    const style = document.createElement('style');
    style.id = 'tendencia-animations';
    style.textContent = `
        @keyframes pulse {
            0%, 100% {
                transform: scale(1);
            }
            50% {
                transform: scale(1.05);
            }
        }
        
        @keyframes animate-shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-10px); }
            75% { transform: translateX(10px); }
        }
        
        .animate-shake {
            animation: animate-shake 0.5s ease-in-out;
        }
        
        .is-valid {
            border-color: #10b981 !important;
        }
        
        .is-invalid {
            border-color: #ef4444 !important;
        }
    `;
    document.head.appendChild(style);
}

// ============================================
// 7. EXPORT PARA DEBUGGING
// ============================================

window.TendenciaFinanceira = {
    validarFormulario,
    inicializarGrafico,
    graficoState: () => graficoState
};

console.log('✅ Módulo TendenciaFinanceira carregado e pronto');