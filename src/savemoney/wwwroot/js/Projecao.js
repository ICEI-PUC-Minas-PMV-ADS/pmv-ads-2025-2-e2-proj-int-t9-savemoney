// Inicialização ao carregar a página
document.addEventListener("DOMContentLoaded", function () {
    initChart();
    initAccordion();
});

/* =========================================
   FUNÇÃO GLOBAL (Acessível pelo onclick do HTML)
   ========================================= */
function toggleInfoSection() {
    const section = document.getElementById('infoSection');
    const btnText = document.getElementById('btnInfoText');

    if (!section || !btnText) return; // Segurança

    if (section.style.display === 'none') {
        section.style.display = 'block';
        btnText.textContent = 'Ocultar Explicação';
        // Scroll suave
        setTimeout(() => {
            section.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }, 100);
    } else {
        section.style.display = 'none';
        btnText.textContent = 'Entenda o Cálculo';
    }
}

/* =========================================
   LÓGICA DO ACORDEÃO (FAQ)
   ========================================= */
function initAccordion() {
    const accordionHeaders = document.querySelectorAll('.accordion-header');

    accordionHeaders.forEach(header => {
        header.addEventListener('click', () => {
            const contentId = header.getAttribute('aria-controls');
            const content = document.getElementById(contentId);
            const isExpanded = header.getAttribute('aria-expanded') === 'true';

            // Opcional: Fechar outros ao abrir um
            document.querySelectorAll('.accordion-header').forEach(h => {
                h.setAttribute('aria-expanded', 'false');
                const c = document.getElementById(h.getAttribute('aria-controls'));
                if (c) {
                    c.style.maxHeight = null;
                    c.style.paddingBottom = null;
                }
            });

            // Toggle do atual
            if (!isExpanded) {
                header.setAttribute('aria-expanded', 'true');
                if (content) {
                    content.style.maxHeight = content.scrollHeight + 'px';
                    content.style.paddingBottom = '1.5rem';
                }
            }
        });
    });
}

/* =========================================
   GRÁFICO (Canvas)
   ========================================= */
function initChart() {
    const canvas = document.getElementById("meuGraficoCanvas");
    if (!canvas) return;

    // Se não houver dados, para
    if (!window.saveMoneyDados) return;

    const ctx = canvas.getContext("2d");
    const labels = window.saveMoneyDados.labels;
    const values = window.saveMoneyDados.values;

    const colors = {
        primary: "#3b82f6",
        text: "#aaaaaa",
        grid: "#2a2c3c",
        bgStart: "rgba(59, 130, 246, 0.4)",
        bgEnd: "rgba(59, 130, 246, 0.0)"
    };

    function draw() {
        const container = canvas.parentElement;
        canvas.width = container.clientWidth;
        canvas.height = container.clientHeight;

        const w = canvas.width;
        const h = canvas.height;
        const padding = { top: 40, right: 30, bottom: 30, left: 60 };

        ctx.clearRect(0, 0, w, h);

        const chartW = w - padding.left - padding.right;
        const chartH = h - padding.top - padding.bottom;

        if (values.length === 0) return;

        const maxVal = Math.max(...values);
        const minVal = Math.min(...values);

        // Evita divisão por zero se todos os valores forem iguais
        let range = maxVal - minVal;
        if (range === 0) range = 100;

        const offset = range * 0.2;
        const upperLimit = maxVal + offset;
        const lowerLimit = minVal - offset;
        const adjustedRange = upperLimit - lowerLimit;

        const getY = (val) => padding.top + chartH - ((val - lowerLimit) / adjustedRange) * chartH;
        const getX = (i) => padding.left + (i / (labels.length - 1)) * chartW;

        // 1. Grade
        ctx.strokeStyle = colors.grid;
        ctx.lineWidth = 1;
        ctx.beginPath();
        for (let i = 0; i <= 4; i++) {
            const y = padding.top + (chartH / 4) * i;
            ctx.moveTo(padding.left, y);
            ctx.lineTo(w - padding.right, y);

            const val = upperLimit - (adjustedRange / 4) * i;
            ctx.fillStyle = colors.text;
            ctx.font = "11px Inter, sans-serif";
            ctx.textAlign = "right";
            ctx.fillText(val.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 }), padding.left - 10, y + 4);
        }
        ctx.stroke();

        // 2. Área (Gradiente)
        const gradient = ctx.createLinearGradient(0, padding.top, 0, h);
        gradient.addColorStop(0, colors.bgStart);
        gradient.addColorStop(1, colors.bgEnd);
        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.moveTo(getX(0), getY(values[0]));
        for (let i = 1; i < values.length; i++) ctx.lineTo(getX(i), getY(values[i]));
        ctx.lineTo(getX(values.length - 1), h - padding.bottom);
        ctx.lineTo(getX(0), h - padding.bottom);
        ctx.fill();

        // 3. Linha
        ctx.strokeStyle = colors.primary;
        ctx.lineWidth = 3;
        ctx.beginPath();
        ctx.moveTo(getX(0), getY(values[0]));
        for (let i = 1; i < values.length; i++) ctx.lineTo(getX(i), getY(values[i]));
        ctx.stroke();

        // 4. Pontos
        for (let i = 0; i < values.length; i++) {
            const x = getX(i);
            const y = getY(values[i]);
            ctx.fillStyle = "#1b1d29";
            ctx.beginPath();
            ctx.arc(x, y, 5, 0, Math.PI * 2);
            ctx.fill();
            ctx.strokeStyle = colors.primary;
            ctx.stroke();

            ctx.fillStyle = colors.text;
            ctx.textAlign = "center";
            ctx.fillText(labels[i], x, h - 10);
        }
    }

    draw();
    window.addEventListener('resize', draw);
}