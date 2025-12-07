/**
 * GERENCIADOR DO MODAL DE EXPORTAÇÃO
 */
document.addEventListener('DOMContentLoaded', () => {
    const modalElement = document.getElementById('modalExportacao');
    if (!modalElement) return;

    // Cache de elementos
    const els = {
        form: document.getElementById('formExportacao'),
        inputInicio: document.getElementById('exportDataInicio'),
        inputFim: document.getElementById('exportDataFim'),
        btnSubmit: document.getElementById('btnExportarSubmit'),
        btnText: document.querySelector('#btnExportarSubmit .btn-text'),
        btnLoader: document.querySelector('#btnExportarSubmit .btn-loader')
    };

    // 1. Ao Abrir: Define datas padrão (Mês Atual)
    modalElement.addEventListener('show.bs.modal', () => {
        resetButtonState(els);

        if (!els.inputInicio.value || !els.inputFim.value) {
            const hoje = new Date();
            const inicioMes = new Date(hoje.getFullYear(), hoje.getMonth(), 1);

            // Helper para formatar YYYY-MM-DD
            const format = (d) => d.toISOString().split('T')[0];

            els.inputInicio.value = format(inicioMes);
            els.inputFim.value = format(hoje);
        }
    });

    // 2. Ao Fechar: Limpeza de Backdrop (Essencial para evitar travamento)
    modalElement.addEventListener('hidden.bs.modal', () => {
        if (els.btnSubmit) els.btnSubmit.blur();

        // Remove backdrops residuais
        document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
        document.body.classList.remove('modal-open');
        document.body.style = '';
    });

    // 3. Validação de Datas (Início <= Fim)
    const validarDatas = () => {
        if (els.inputInicio.value && els.inputFim.value) {
            if (els.inputFim.value < els.inputInicio.value) {
                els.inputFim.value = els.inputInicio.value;
            }
        }
    };

    if (els.inputInicio) els.inputInicio.addEventListener('change', validarDatas);
    if (els.inputFim) els.inputFim.addEventListener('change', validarDatas);

    // 4. Submit (Download)
    if (els.form) {
        els.form.addEventListener('submit', () => {
            // Mostra loading
            if (els.btnSubmit) els.btnSubmit.disabled = true;
            if (els.btnText) els.btnText.classList.add('d-none');
            if (els.btnLoader) els.btnLoader.classList.remove('d-none');

            // Fecha modal após 1.5s (tempo estimado para o browser iniciar o download)
            setTimeout(() => {
                const instance = bootstrap.Modal.getInstance(modalElement);
                if (instance) instance.hide();

                // Reseta botão
                setTimeout(() => resetButtonState(els), 500);
            }, 1500);
        });
    }
});

function resetButtonState(els) {
    if (els.btnSubmit) els.btnSubmit.disabled = false;
    if (els.btnText) els.btnText.classList.remove('d-none');
    if (els.btnLoader) els.btnLoader.classList.add('d-none');
}