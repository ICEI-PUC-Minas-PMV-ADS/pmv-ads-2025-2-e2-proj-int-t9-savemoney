/* ============================================================================
   CALCULADORA DE PONTO DE EQUILÍBRIO
   ============================================================================ */

(() => {
    'use strict';

    // Elementos principais
    const form = document.getElementById('pe-form');
    const inputCustosFixos = document.getElementById('custos-fixos');
    const inputPrecoVenda = document.getElementById('preco-venda');
    const inputCustoVariavel = document.getElementById('custo-variavel');
    const btnCalcular = document.getElementById('btn-calcular');
    const resultadoDiv = document.getElementById('resultado-placeholder');

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeCalculadora);

    function initializeCalculadora() {
        console.log('Calculadora de Ponto de Equilibrio loaded');
        setupEventListeners();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        if (inputCustosFixos) {
            inputCustosFixos.addEventListener('input', applyCurrencyMask);
        }
        if (inputPrecoVenda) {
            inputPrecoVenda.addEventListener('input', applyCurrencyMask);
        }
        if (inputCustoVariavel) {
            inputCustoVariavel.addEventListener('input', applyCurrencyMask);
        }
        if (form) {
            form.addEventListener('submit', calcularPontoEquilibrio);
        }

        const accordionHeaders = document.querySelectorAll('.accordion-header');
        accordionHeaders.forEach(header => {
            header.addEventListener('click', () => toggleAccordion(header));
        });
    }

    /* Formatação de Moeda
       ======================================================================== */
    function formatCurrency(value) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    }

    function parseCurrency(value) {
        if (!value) return 0;
        return parseFloat(
            value.replace(/[^0-9,-]+/g, '')
                .replace(/\./g, '')
                .replace(',', '.')
        ) || 0;
    }

    function applyCurrencyMask(e) {
        let value = e.target.value;

        // Remove tudo que não é dígito
        value = value.replace(/\D/g, '');

        // Converte para decimal
        value = (parseInt(value) / 100).toFixed(2);

        // Troca ponto por vírgula
        value = value.replace('.', ',');

        // Adiciona separador de milhares
        value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');

        e.target.value = 'R$ ' + value;
    }

    /* Cálculo do Ponto de Equilíbrio
       ======================================================================== */
    function calcularPontoEquilibrio(e) {
        e.preventDefault();

        const custosFixos = parseCurrency(inputCustosFixos.value);
        const precoVenda = parseCurrency(inputPrecoVenda.value);
        const custoVariavel = parseCurrency(inputCustoVariavel.value);

        const margemContribuicao = precoVenda - custoVariavel;

        // Validações
        if (custosFixos <= 0) {
            exibirErro('Os Custos Fixos devem ser maiores que R$ 0,00.');
            return;
        }
        if (precoVenda <= 0) {
            exibirErro('O Preço de Venda deve ser maior que R$ 0,00.');
            return;
        }
        if (margemContribuicao <= 0) {
            exibirErro('O Preço de Venda deve ser maior que o Custo Variável para que o negócio seja lucrativo.');
            return;
        }

        // Cálculo: PE = Custos Fixos / (Preço de Venda - Custo Variável)
        const pontoEquilibrioUnidades = custosFixos / margemContribuicao;

        exibirResultado(Math.ceil(pontoEquilibrioUnidades));
    }

    /* Display de Resultados
       ======================================================================== */
    function exibirResultado(unidades) {
        if (!resultadoDiv) return;

        resultadoDiv.className = 'resultado-calculado';
        resultadoDiv.innerHTML = `
            <h5>Ponto de Equilíbrio (Unidades)</h5>
            <h2>${unidades}</h2>
            <p>
                Você precisa vender <strong>${unidades} unidades</strong> do seu produto 
                para cobrir todos os seus custos fixos e variáveis. A partir da unidade 
                ${unidades + 1}, você começa a ter lucro.
            </p>
        `;
    }

    function exibirErro(mensagem) {
        if (!resultadoDiv) return;

        resultadoDiv.className = 'resultado-placeholder';
        resultadoDiv.innerHTML = `
            <span class="material-symbols-outlined icon-placeholder" style="color: #F87171;">warning</span>
            <h3 style="color: #F87171;">Erro no Cálculo</h3>
            <p>${mensagem}</p>
        `;
    }

    /* Accordion
       ======================================================================== */
    function toggleAccordion(header) {
        const content = document.getElementById(header.getAttribute('aria-controls'));
        if (!content) return;

        const isExpanded = header.getAttribute('aria-expanded') === 'true';

        if (isExpanded) {
            header.setAttribute('aria-expanded', 'false');
            content.setAttribute('aria-hidden', 'true');
            content.style.maxHeight = null;
            content.style.paddingBottom = null;
        } else {
            header.setAttribute('aria-expanded', 'true');
            content.setAttribute('aria-hidden', 'false');
            content.style.maxHeight = content.scrollHeight + 'px';
            content.style.paddingBottom = '1.5rem';
        }
    }

})();