// TendenciaFinanceira-Index.js
// Sistema de Análise de Tendências Financeiras - Página Index

document.addEventListener('DOMContentLoaded', function () {
    console.log('📊 Index - Tendência Financeira carregado');

    // Adicionar animações CSS se não existirem
    adicionarAnimacoesCSS();

    // Inicializar formulário
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
            const textoOriginal = btnGerar.innerHTML;
            btnGerar.innerHTML = '<i class="bi bi-hourglass-split me-2"></i> Analisando...';
            btnGerar.setAttribute('data-texto-original', textoOriginal);
        }
    });

    console.log('✅ Formulário inicializado com validações');
}

function validarFormulario() {
    const selectMeses = document.getElementById('meses');

    if (!selectMeses) {
        console.error('❌ Select de meses não encontrado');
        return false;
    }

    // Limpar validações anteriores
    selectMeses.classList.remove('is-invalid', 'is-valid');

    // Validar se um período foi selecionado
    if (!selectMeses.value || selectMeses.value === '' || selectMeses.value === '0') {
        selectMeses.classList.add('is-invalid');
        mostrarErro('Por favor, selecione um período de análise');
        selectMeses.focus();
        return false;
    }

    const meses = parseInt(selectMeses.value);

    // Validar se é número válido
    if (isNaN(meses)) {
        selectMeses.classList.add('is-invalid');
        mostrarErro('Período inválido');
        return false;
    }

    // Validar valor mínimo e máximo
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
    // Remover erro anterior
    esconderInfoPeriodo();

    // Criar ou atualizar alerta de erro
    let alertErro = document.querySelector('.alert-erro-validacao');

    if (!alertErro) {
        alertErro = document.createElement('div');
        alertErro.className = 'alert-custom alert-danger mt-3 alert-erro-validacao';
        alertErro.innerHTML = `
            <i class="bi bi-exclamation-triangle me-2"></i>
            <span>${mensagem}</span>
        `;

        const form = document.querySelector('form[action*="GerarAnalise"]');
        if (form && form.parentElement) {
            form.parentElement.appendChild(alertErro);
        }
    } else {
        const span = alertErro.querySelector('span');
        if (span) {
            span.textContent = mensagem;
        }
    }

    // Adicionar animação shake
    alertErro.classList.remove('animate-shake');
    setTimeout(() => {
        alertErro.classList.add('animate-shake');
    }, 10);

    setTimeout(() => {
        alertErro.classList.remove('animate-shake');
    }, 510);
}

function mostrarInfoPeriodo(meses) {
    // Remover info/erro anterior
    esconderInfoPeriodo();

    // Criar mensagem informativa
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
    if (form && form.parentElement) {
        form.parentElement.appendChild(infoDiv);
    }
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
// ANIMAÇÕES CSS DINÂMICAS
// ============================================

function adicionarAnimacoesCSS() {
    if (!document.getElementById('tendencia-index-animations')) {
        const style = document.createElement('style');
        style.id = 'tendencia-index-animations';
        style.textContent = `
            @keyframes animate-shake {
                0%, 100% { transform: translateX(0); }
                25% { transform: translateX(-10px); }
                75% { transform: translateX(10px); }
            }
            
            @keyframes fade-in-up {
                0% {
                    opacity: 0;
                    transform: translateY(10px);
                }
                100% {
                    opacity: 1;
                    transform: translateY(0);
                }
            }
            
            .animate-shake {
                animation: animate-shake 0.5s ease-in-out;
            }
            
            .is-valid {
                border-color: #10b981 !important;
                background-image: none !important;
            }
            
            .is-invalid {
                border-color: #ef4444 !important;
                background-image: none !important;
            }
            
            .form-select.is-valid:focus,
            .form-select.is-invalid:focus {
                box-shadow: 0 0 0 0.25rem rgba(16, 185, 129, 0.25);
            }
            
            .form-select.is-invalid:focus {
                box-shadow: 0 0 0 0.25rem rgba(239, 68, 68, 0.25);
            }
        `;
        document.head.appendChild(style);
        console.log('✅ Animações CSS adicionadas');
    }
}

// ============================================
// EXPORT PARA DEBUGGING
// ============================================

window.TendenciaFinanceiraIndex = {
    validarFormulario: validarFormulario,
    mostrarErro: mostrarErro,
    esconderInfoPeriodo: esconderInfoPeriodo
};

console.log('✅ Módulo TendenciaFinanceira-Index carregado e pronto');