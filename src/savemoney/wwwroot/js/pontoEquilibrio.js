document.addEventListener("DOMContentLoaded", function () {
    
    const form = document.getElementById('pe-form');
    const inputCustosFixos = document.getElementById('custos-fixos');
    const inputPrecoVenda = document.getElementById('preco-venda');
    const inputCustoVariavel = document.getElementById('custo-variavel');
    const btnCalcular = document.getElementById('btn-calcular');
    const resultadoDiv = document.getElementById('resultado-placeholder');

    const formatCurrency = (value) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };
    
    const parseCurrency = (value) => {
        if (!value) return 0;
        return parseFloat(value.replace(/[^0-9,-]+/g, "").replace('.', '').replace(',', '.')) || 0;
    };
    
    const applyCurrencyMask = (e) => {
        let value = e.target.value;
        value = value.replace(/\D/g, "");
        value = (parseInt(value) / 100).toFixed(2) + "";
        value = value.replace(".", ",");
        value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
        e.target.value = "R$ " + value;
    };

    inputCustosFixos.addEventListener('input', applyCurrencyMask);
    inputPrecoVenda.addEventListener('input', applyCurrencyMask);
    inputCustoVariavel.addEventListener('input', applyCurrencyMask);

    // --- 3. LÓGICA PRINCIPAL DA CALCULADORA ---
    
    function calcularPontoEquilibrio(e) {
        e.preventDefault();

        const custosFixos = parseCurrency(inputCustosFixos.value);
        const precoVenda = parseCurrency(inputPrecoVenda.value);
        const custoVariavel = parseCurrency(inputCustoVariavel.value);

        const margemContribuicao = precoVenda - custoVariavel;

        // Validação
        if (custosFixos <= 0) {
            exibirErro("Os Custos Fixos devem ser maiores que R$ 0,00.");
            return;
        }
        if (precoVenda <= 0) {
            exibirErro("O Preço de Venda deve ser maior que R$ 0,00.");
            return;
        }
        if (margemContribuicao <= 0) {
            exibirErro("O Preço de Venda deve ser maior que o Custo Variável para que o negócio seja lucrativo.");
            return;
        }

        const pontoEquilibrioUnidades = custosFixos / margemContribuicao;

        exibirResultado(pontoEquilibrioUnidades.toFixed(0)); 
    }

    // FUNÇÕES DE DISPLAY

    function exibirResultado(unidades) {
        resultadoDiv.className = 'resultado-calculado';
        resultadoDiv.innerHTML = `
            <h5>Ponto de Equilíbrio (Unidades)</h5>
            <h2>${unidades}</h2>
            <p>Você precisa vender <strong>${unidades} unidades</strong> do seu produto apenas para cobrir todos os seus custos fixos e variáveis. A partir da unidade ${parseInt(unidades) + 1}, você começa a ter lucro.</p>
        `;
    }

    function exibirErro(mensagem) {
        resultadoDiv.className = 'resultado-placeholder';
        resultadoDiv.innerHTML = `
            <span class="material-symbols-outlined icon-placeholder" style="color: #F87171;">warning</span>
            <h5 style="color: #F87171;">Erro no Cálculo</h5>
            <p>${mensagem}</p>
        `;
    }
    
    form.addEventListener('submit', calcularPontoEquilibrio);

    // --- 5. LÓGICA DO ACORDEÃO ---
    
    const accordionHeaders = document.querySelectorAll('.accordion-header');

    accordionHeaders.forEach(header => {
        header.addEventListener('click', () => {
            const content = document.getElementById(header.getAttribute('aria-controls'));
            const isExpanded = header.getAttribute('aria-expanded') === 'true';

            if (isExpanded) {
                header.setAttribute('aria-expanded', 'false');
                content.setAttribute('aria-hidden', 'true');
                content.style.maxHeight = null;
                content.style.paddingBottom = null;
            } else {
                header.setAttribute('aria-expanded', 'true');
                content.setAttribute('aria-hidden', 'false');
                content.style.maxHeight = content.scrollHeight + 'px'; // Define altura
                content.style.paddingBottom = '1.5rem'; // Adiciona padding
            }
        });
    });
});