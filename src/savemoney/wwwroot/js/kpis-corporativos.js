/* ============================================
   KPIS CORPORATIVOS - JavaScript
   Interacoes e tooltips
   ============================================ */

document.addEventListener('DOMContentLoaded', function () {
    initTooltips();
    initAnimacoes();
});

/**
 * Inicializa tooltips nas barras dos graficos
 */
function initTooltips() {
    const barras = document.querySelectorAll('.evolucao-barra, .categoria-progresso');

    barras.forEach(barra => {
        barra.addEventListener('mouseenter', function (e) {
            const titulo = this.getAttribute('title');
            if (!titulo) return;

            const tooltip = document.createElement('div');
            tooltip.className = 'kpi-tooltip';
            tooltip.textContent = titulo;
            tooltip.style.cssText = `
                position: fixed;
                background: var(--bg-card);
                color: var(--text-primary);
                padding: 0.5rem 0.75rem;
                border-radius: 0.375rem;
                font-size: 0.8rem;
                box-shadow: var(--shadow-deep);
                border: 1px solid var(--border-color);
                z-index: 1000;
                pointer-events: none;
                white-space: nowrap;
            `;

            document.body.appendChild(tooltip);

            const rect = this.getBoundingClientRect();
            tooltip.style.left = (rect.left + rect.width / 2 - tooltip.offsetWidth / 2) + 'px';
            tooltip.style.top = (rect.top - tooltip.offsetHeight - 8) + 'px';

            this._tooltip = tooltip;
        });

        barra.addEventListener('mouseleave', function () {
            if (this._tooltip) {
                this._tooltip.remove();
                this._tooltip = null;
            }
        });
    });
}

/**
 * Inicializa animacoes de entrada
 */
function initAnimacoes() {
    const cards = document.querySelectorAll('.kpi-card-grande');

    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';

        setTimeout(() => {
            card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 150);
    });
}

/**
 * Atualiza KPIs via AJAX (para uso futuro)
 */
async function atualizarKpisAjax(dataInicio, dataFim) {
    try {
        const response = await fetch(
            `/KpisCorporativos/ObterKpis?dataInicio=${dataInicio}&dataFim=${dataFim}`
        );
        const data = await response.json();

        if (data.success) {
            console.log('KPIs atualizados:', data.dados);
            // Implementar atualizacao dinamica dos valores
        }
    } catch (error) {
        console.error('Erro ao atualizar KPIs:', error);
    }
}

/**
 * Formata valor para moeda BRL
 */
function formatarMoeda(valor) {
    return valor.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
}

/**
 * Formata percentual
 */
function formatarPercentual(valor) {
    return valor.toFixed(1) + '%';
}