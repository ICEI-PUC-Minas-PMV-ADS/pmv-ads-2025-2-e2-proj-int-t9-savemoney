/* =========================================
   RELATORIOS FINANCEIROS - JavaScript
   100% Vanilla JS - Sem dependencias
   ========================================= */

document.addEventListener('DOMContentLoaded', function () {
    initCounters();
    initDateChips();
    initSearch();
    initTooltips();
});

/* =========================================
   1. Animacao de Contadores (KPIs)
   ========================================= */
function initCounters() {
    const counters = document.querySelectorAll('.kpi-counter');
    const countersPercent = document.querySelectorAll('.kpi-counter-percent');

    const currencyFormatter = new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });

    function animateCounter(el, isPercent) {
        const rawValue = el.getAttribute('data-target');
        if (!rawValue) return;

        const target = parseFloat(rawValue.replace(',', '.'));
        if (isNaN(target)) return;

        const duration = 1500;
        const startTimestamp = performance.now();

        function step(timestamp) {
            const progress = Math.min((timestamp - startTimestamp) / duration, 1);
            // Easing: easeOutExpo
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
                // Valor final exato
                if (isPercent) {
                    el.innerText = target.toFixed(1).replace('.', ',') + '%';
                } else {
                    el.innerText = currencyFormatter.format(target);
                }
            }
        }

        window.requestAnimationFrame(step);
    }

    counters.forEach(c => animateCounter(c, false));
    countersPercent.forEach(c => animateCounter(c, true));
}

/* =========================================
   2. Chips de Data (Filtros Rapidos)
   ========================================= */
function initDateChips() {
    const chips = document.querySelectorAll('.date-chip');
    const inputInicio = document.querySelector('input[name="dataInicio"]');
    const inputFim = document.querySelector('input[name="dataFim"]');

    if (!inputInicio || !inputFim) return;

    chips.forEach(chip => {
        chip.addEventListener('click', function () {
            // Remove active de todos
            chips.forEach(c => c.classList.remove('active'));
            this.classList.add('active');

            const period = this.getAttribute('data-period');
            const today = new Date();
            let start = new Date();
            let end = new Date();

            switch (period) {
                case 'today':
                    start = today;
                    end = today;
                    break;
                case 'week':
                    start.setDate(today.getDate() - 7);
                    break;
                case 'month':
                    start.setDate(1);
                    break;
                case 'quarter':
                    start.setMonth(today.getMonth() - 3);
                    break;
                case 'year':
                    start.setMonth(0);
                    start.setDate(1);
                    break;
            }

            inputInicio.value = formatDate(start);
            inputFim.value = formatDate(end);
        });
    });

    // Marca chip ativo baseado no periodo atual
    markActiveChip();
}

function formatDate(date) {
    return date.toISOString().split('T')[0];
}

function markActiveChip() {
    const inputInicio = document.querySelector('input[name="dataInicio"]');
    const inputFim = document.querySelector('input[name="dataFim"]');
    const chips = document.querySelectorAll('.date-chip');

    if (!inputInicio || !inputFim) return;

    const inicio = new Date(inputInicio.value);
    const fim = new Date(inputFim.value);
    const today = new Date();
    const diffDays = Math.round((fim - inicio) / (1000 * 60 * 60 * 24));

    chips.forEach(chip => {
        const period = chip.getAttribute('data-period');
        let isActive = false;

        switch (period) {
            case 'today':
                isActive = diffDays === 0;
                break;
            case 'week':
                isActive = diffDays >= 6 && diffDays <= 8;
                break;
            case 'month':
                isActive = diffDays >= 28 && diffDays <= 31 && inicio.getDate() === 1;
                break;
            case 'quarter':
                isActive = diffDays >= 89 && diffDays <= 92;
                break;
            case 'year':
                isActive = inicio.getMonth() === 0 && inicio.getDate() === 1;
                break;
        }

        if (isActive) {
            chip.classList.add('active');
        }
    });
}

/* =========================================
   3. Busca na Tabela
   ========================================= */
function initSearch() {
    const searchInput = document.getElementById('searchTransactions');
    const tableRows = document.querySelectorAll('#transactionsTable tbody tr');

    if (!searchInput || tableRows.length === 0) return;

    searchInput.addEventListener('keyup', function (e) {
        const term = e.target.value.toLowerCase().trim();

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

/* =========================================
   4. Tooltips Nativos (Barras e Pontos)
   ========================================= */
function initTooltips() {
    // Tooltips nas barras do grafico
    const bars = document.querySelectorAll('.bar, .horizontal-bar-fill, .donut-segment, .line-chart-point');

    bars.forEach(bar => {
        bar.addEventListener('mouseenter', function (e) {
            const title = this.getAttribute('title') || this.querySelector('title')?.textContent;
            if (!title) return;

            showTooltip(e, title);
        });

        bar.addEventListener('mousemove', function (e) {
            moveTooltip(e);
        });

        bar.addEventListener('mouseleave', function () {
            hideTooltip();
        });
    });
}

function showTooltip(e, text) {
    let tooltip = document.getElementById('chart-tooltip');

    if (!tooltip) {
        tooltip = document.createElement('div');
        tooltip.id = 'chart-tooltip';
        tooltip.style.cssText = `
            position: fixed;
            background: rgba(20, 22, 30, 0.95);
            color: #fff;
            padding: 0.5rem 0.75rem;
            border-radius: 6px;
            font-size: 0.8rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            border: 1px solid rgba(255,255,255,0.1);
            z-index: 10000;
            pointer-events: none;
            white-space: nowrap;
            transition: opacity 0.15s;
        `;
        document.body.appendChild(tooltip);
    }

    tooltip.textContent = text;
    tooltip.style.opacity = '1';
    moveTooltip(e);
}

function moveTooltip(e) {
    const tooltip = document.getElementById('chart-tooltip');
    if (!tooltip) return;

    const offsetX = 15;
    const offsetY = 15;

    let x = e.clientX + offsetX;
    let y = e.clientY + offsetY;

    // Ajusta se sair da tela
    const rect = tooltip.getBoundingClientRect();
    if (x + rect.width > window.innerWidth) {
        x = e.clientX - rect.width - offsetX;
    }
    if (y + rect.height > window.innerHeight) {
        y = e.clientY - rect.height - offsetY;
    }

    tooltip.style.left = x + 'px';
    tooltip.style.top = y + 'px';
}

function hideTooltip() {
    const tooltip = document.getElementById('chart-tooltip');
    if (tooltip) {
        tooltip.style.opacity = '0';
    }
}

/* =========================================
   5. Utilitarios
   ========================================= */

// Formata valor para moeda BRL
function formatarMoeda(valor) {
    return valor.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
}

// Formata percentual
function formatarPercentual(valor) {
    return valor.toFixed(1).replace('.', ',') + '%';
}