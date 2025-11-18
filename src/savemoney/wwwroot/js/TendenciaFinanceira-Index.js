// TendenciaFinanceira-Index.js - Página de seleção de período
// Sistema de Análise de Tendências Financeiras

document.addEventListener('DOMContentLoaded', function () {
    console.log('📊 Index - Tendência Financeira carregado');
    inicializarFormulario();
});

// ============================================
// FORMULÁRIO - VALIDAÇÕES E INTERATIVIDADE
// ============================================

function inicializarFormulario() {
    const form = document.querySelector('form[action*="GerarAnalise"]');
    const btnGerar = document.querySelector('.btn-tendencia');
    const selectMeses = document.getElementById('meses');

    if (!form || !selectMeses) {
        console.error('❌ Formulário ou select de meses não encontrado');
        return;
    }

    // Feedback visual no select
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

    // Submissão do formulário
    form.addEventListener('submit', function (e) {
        if (!validarFormulario()) {
            e.preventDefault();
            return false;
        }

        // Mostrar loading no botão
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
// EXPORT PARA DEBUGGING
// ============================================

window.TendenciaFinanceiraIndex = {
    validarFormulario
};

console.log('✅ Módulo TendenciaFinanceira-Index carregado');