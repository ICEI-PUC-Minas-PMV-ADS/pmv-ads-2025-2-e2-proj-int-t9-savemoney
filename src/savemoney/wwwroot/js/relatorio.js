(function () {
    // mantém instâncias para destruir antes de recriar
    function createCharts() {
        const data = window.relatorioData || { labels: [], receitas: [], despesas: [], totalReceitas: 0, totalDespesas: 0 };
        const labels = data.labels || [];
        const receitas = data.receitas || [];
        const despesas = data.despesas || [];

        const barEl = document.getElementById('barChart');
        const pieEl = document.getElementById('pieChart');

        try {
            // destruir charts antigos se existirem
            if (window._barChart) { window._barChart.destroy(); window._barChart = null; }
            if (window._pieChart) { window._pieChart.destroy(); window._pieChart = null; }
        } catch (err) {
            console.warn('Erro ao destruir charts antigos', err);
        }

        if (barEl && barEl.getContext) {
            const ctx = barEl.getContext('2d');
            window._barChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        { label: 'Receitas', data: receitas },
                        { label: 'Despesas', data: despesas }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: { y: { beginAtZero: true } }
                }
            });
        }

        if (pieEl && pieEl.getContext) {
            const ctx2 = pieEl.getContext('2d');
            window._pieChart = new Chart(ctx2, {
                type: 'pie',
                data: {
                    labels: ['Receitas', 'Despesas'],
                    datasets: [{ data: [data.totalReceitas || 0, data.totalDespesas || 0] }]
                },
                options: { responsive: true, maintainAspectRatio: false }
            });
        }

        // gerar imagens base64 (caso canvas exista)
        try {
            if (barEl && barEl.toDataURL) {
                window.__chartImageBase64 = barEl.toDataURL('image/png');
            } else {
                window.__chartImageBase64 = '';
            }
            if (pieEl && pieEl.toDataURL) {
                window.__pieImageBase64 = pieEl.toDataURL('image/png');
            } else {
                window.__pieImageBase64 = '';
            }

            // preencher os hidden inputs automaticamente (ajuda export sem clique extra)
            const barInput = document.getElementById('ChartImage');
            const pieInput = document.getElementById('PieImage');
            if (barInput) barInput.value = window.__chartImageBase64 || '';
            if (pieInput) pieInput.value = window.__pieImageBase64 || '';
        } catch (err) {
            console.warn('Não foi possível gerar imagens dos charts:', err);
        }
    }

    // function chamada pelos botões de export no Razor
    window.prepareExport = function () {
        // tenta atualizar os hidden inputs a partir das imagens geradas
        try {
            const barInput = document.getElementById('ChartImage');
            const pieInput = document.getElementById('PieImage');

            if (window.__chartImageBase64 && barInput) barInput.value = window.__chartImageBase64;
            if (window.__pieImageBase64 && pieInput) pieInput.value = window.__pieImageBase64;

            // sempre retornar true para permitir o submit (o backend tratará ausência de imagens)
            return true;
        } catch (err) {
            console.error('Erro em prepareExport:', err);
            return true;
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        createCharts();
    });

    // expor função para re-criar manualmente (útil após AJAX ou reconstruição)
    window.relatorio_createCharts = createCharts;
})();
