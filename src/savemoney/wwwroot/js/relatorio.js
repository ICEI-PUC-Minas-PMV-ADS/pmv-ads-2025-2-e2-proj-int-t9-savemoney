document.addEventListener('DOMContentLoaded', function () {
    const data = window.relatorioData || { labels: [], receitas: [], despesas: [], totalReceitas: 0, totalDespesas: 0 };
    const ctxBar = document.getElementById('barChart')?.getContext('2d');
    const ctxPie = document.getElementById('pieChart')?.getContext('2d');

    if (ctxBar) {
        new Chart(ctxBar, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [
                    { label: 'Receitas', data: data.receitas, backgroundColor: '#198754' },
                    { label: 'Despesas', data: data.despesas, backgroundColor: '#dc3545' }
                ]
            },
            options: { responsive: true, maintainAspectRatio: false, scales: { y: { beginAtZero: true } } }
        });
    }

    if (ctxPie) {
        new Chart(ctxPie, {
            type: 'pie',
            data: {
                labels: ['Receitas', 'Despesas'],
                datasets: [{ data: [data.totalReceitas, data.totalDespesas], backgroundColor: ['#198754', '#dc3545'] }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
    }
});

// captura imagens dos gráficos antes do envio (coloca dados base64 nos hidden inputs)
function prepareExport() {
    const bar = document.getElementById('barChart');
    const pie = document.getElementById('pieChart');
    const chartInput = document.getElementById('ChartImage');
    const pieInput = document.getElementById('PieImage');

    if (bar && chartInput) {
        try { chartInput.value = bar.toDataURL('image/png'); } catch { chartInput.value = ''; }
    }
    if (pie && pieInput) {
        try { pieInput.value = pie.toDataURL('image/png'); } catch { pieInput.value = ''; }
    }
}