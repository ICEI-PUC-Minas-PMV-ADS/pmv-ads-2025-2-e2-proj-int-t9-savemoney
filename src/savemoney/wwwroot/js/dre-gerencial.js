/* ============================================
   DRE GERENCIAL - JavaScript
   Toggle comparativo, tooltips, exportação
   ============================================ */

document.addEventListener('DOMContentLoaded', function () {
    initComparativoToggle();
    initBarrasTooltip();
    initExportacao();
    initPeriodosRapidos();
});

/**
 * Toggle dos campos de comparativo
 */
function initComparativoToggle() {
    const checkbox = document.getElementById('chk-comparativo');
    const camposComparativos = document.querySelectorAll('.filtro-comparativo');

    if (!checkbox) return;

    checkbox.addEventListener('change', function () {
        camposComparativos.forEach(campo => {
            if (this.checked) {
                campo.classList.remove('hidden');
            } else {
                campo.classList.add('hidden');
                // Limpa valores quando desabilita
                const input = campo.querySelector('input');
                if (input) input.value = '';
            }
        });
    });
}

/**
 * Tooltip nas barras do gráfico
 */
function initBarrasTooltip() {
    const barras = document.querySelectorAll('.barra');

    barras.forEach(barra => {
        barra.addEventListener('mouseenter', function (e) {
            const titulo = this.getAttribute('title');
            if (!titulo) return;

            // Cria tooltip
            const tooltip = document.createElement('div');
            tooltip.className = 'barra-tooltip';
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

            // Posiciona tooltip
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
 * Exportação do DRE
 */
function initExportacao() {
    const btnExportar = document.getElementById('btn-exportar');

    if (!btnExportar) return;

    btnExportar.addEventListener('click', function () {
        // Cria menu de opções
        const menu = document.createElement('div');
        menu.className = 'export-menu';
        menu.innerHTML = `
            <div class="export-menu-content">
                <button class="export-option" data-format="pdf">
                    <i class="bi bi-file-pdf"></i> Exportar PDF
                </button>
                <button class="export-option" data-format="excel">
                    <i class="bi bi-file-excel"></i> Exportar Excel
                </button>
                <button class="export-option" data-format="print">
                    <i class="bi bi-printer"></i> Imprimir
                </button>
            </div>
        `;

        menu.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 1000;
        `;

        const content = menu.querySelector('.export-menu-content');
        content.style.cssText = `
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 0.75rem;
            padding: 1rem;
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            min-width: 12rem;
        `;

        const options = menu.querySelectorAll('.export-option');
        options.forEach(opt => {
            opt.style.cssText = `
                display: flex;
                align-items: center;
                gap: 0.5rem;
                padding: 0.75rem 1rem;
                background: transparent;
                border: 1px solid var(--border-color);
                border-radius: 0.5rem;
                color: var(--text-primary);
                cursor: pointer;
                transition: all 0.2s ease;
                font-size: 0.9rem;
            `;

            opt.addEventListener('mouseenter', function () {
                this.style.background = 'var(--bg-secondary)';
                this.style.borderColor = 'var(--accent-primary)';
            });

            opt.addEventListener('mouseleave', function () {
                this.style.background = 'transparent';
                this.style.borderColor = 'var(--border-color)';
            });

            opt.addEventListener('click', function () {
                const format = this.dataset.format;
                executarExportacao(format);
                menu.remove();
            });
        });

        // Fechar ao clicar fora
        menu.addEventListener('click', function (e) {
            if (e.target === menu) {
                menu.remove();
            }
        });

        document.body.appendChild(menu);
    });
}

/**
 * Executa a exportação no formato escolhido
 */
function executarExportacao(formato) {
    const form = document.getElementById('form-filtros');
    const dataInicio = form.querySelector('[name="DataInicio"]').value;
    const dataFim = form.querySelector('[name="DataFim"]').value;
    const regime = form.querySelector('[name="Regime"]').value;

    switch (formato) {
        case 'pdf':
            // Redireciona para action de exportação PDF
            window.location.href = `/DreGerencial/ExportarPdf?dataInicio=${dataInicio}&dataFim=${dataFim}&regime=${regime}`;
            break;

        case 'excel':
            // Redireciona para action de exportação Excel
            window.location.href = `/DreGerencial/ExportarExcel?dataInicio=${dataInicio}&dataFim=${dataFim}&regime=${regime}`;
            break;

        case 'print':
            // Abre janela de impressão
            window.print();
            break;
    }
}

/**
 * Atalhos para períodos rápidos (futuro)
 */
function initPeriodosRapidos() {
    // Pode ser expandido para adicionar botões de "Mês Atual", "Trimestre", "Ano"
}

/**
 * Atualiza dados via AJAX (para uso futuro)
 */
async function atualizarDadosAjax(dataInicio, dataFim, regime) {
    try {
        const response = await fetch(`/DreGerencial/ObterDados?dataInicio=${dataInicio}&dataFim=${dataFim}&regime=${regime}`);
        const data = await response.json();

        if (data.success) {
            // Atualiza KPIs
            atualizarKpis(data.dados);
            // Atualiza tabela
            atualizarTabela(data.dados);
        }
    } catch (error) {
        console.error('Erro ao atualizar dados:', error);
    }
}

/**
 * Atualiza os cards de KPI
 */
function atualizarKpis(dados) {
    // Implementar atualização dinâmica dos KPIs
}

/**
 * Atualiza a tabela do DRE
 */
function atualizarTabela(dados) {
    // Implementar atualização dinâmica da tabela
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