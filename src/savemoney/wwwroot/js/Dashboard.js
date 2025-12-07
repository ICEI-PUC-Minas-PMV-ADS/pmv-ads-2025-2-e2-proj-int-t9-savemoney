// ============================================
// DASHBOARD.JS - REFATORADO PROFISSIONAL
// SaveMoney Dashboard v2.0
// ============================================

// ============ CONFIGURAÇÕES GLOBAIS ============
const CONFIG = {
    GRID_COLS: 3,
    DEBOUNCE_DELAY: 300,
    ANIMATION_DURATION: 300,
    MAX_IMAGE_SIZE: 2 * 1024 * 1024, // 2MB
    NOTIFICATION_DURATION: 3000
};

// ============ ESTADO DA APLICAÇÃO ============
const AppState = {
    draggedWidget: null,
    isDragging: false,
    currentTab: 'galeria',
    newsletterShown: false
};

// ============ INICIALIZAÇÃO ============
document.addEventListener('DOMContentLoaded', () => {
    console.log('🚀 SaveMoney Dashboard v2.0 Iniciando...');

    inicializarDragDrop();
    inicializarAvatarUpload();
    carregarTemaInicial();
    verificarNewsletter();

    console.log('✅ Dashboard carregado com sucesso!');
    console.log('⌨️ Atalhos: ESC | Ctrl+N | Ctrl+T');
});

// ============================================
// CONTROLE DE MODAIS
// ============================================

function abrirModalWidget() {
    const modal = document.getElementById('modalWidget');
    modal.classList.add('active');
    document.getElementById('modalTitle').innerHTML = '<i class="bi bi-plus-square-fill"></i> Adicionar Widget';
    switchTab('galeria');
    limparFormulario();
}

function fecharModalWidget() {
    const modal = document.getElementById('modalWidget');
    modal.classList.remove('active');
    setTimeout(() => limparFormulario(), CONFIG.ANIMATION_DURATION);
}

function confirmarDeletar(id) {
    document.getElementById('deletarId').value = id;
    document.getElementById('modalConfirmar').classList.add('active');
}

function fecharModalConfirmar() {
    document.getElementById('modalConfirmar').classList.remove('active');
}

function limparFormulario() {
    document.getElementById('widgetId').value = '0';
    document.getElementById('titulo').value = '';
    document.getElementById('descricao').value = '';
    document.getElementById('imagemUrl').value = '';
    document.getElementById('link').value = '';
    document.getElementById('colunas').value = '1';
    document.getElementById('largura').value = '1';
    document.getElementById('corFundo').value = '#1b1d29';

    const tipoWidget = document.getElementById('tipoWidget');
    if (tipoWidget) tipoWidget.value = 'custom';

    const preview = document.getElementById('imagemPreview');
    if (preview) preview.style.display = 'none';

    document.querySelectorAll('.erro-imagem').forEach(e => e.remove());
}

// Fechar modal ao clicar fora
window.onclick = function (event) {
    const modals = ['modalWidget', 'modalConfirmar', 'modalTemas', 'modalCriarTema', 'modalNewsletter'];
    modals.forEach(modalId => {
        const modal = document.getElementById(modalId);
        if (modal && event.target === modal) {
            modal.classList.remove('active');
        }
    });
};

// ============================================
// SISTEMA DE TABS
// ============================================

function switchTab(tabName) {
    AppState.currentTab = tabName;

    // Remover active de todas as tabs
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.remove('active');
    });

    // Ativar tab selecionada
    document.querySelector(`[onclick="switchTab('${tabName}')"]`)?.classList.add('active');
    document.getElementById(`tab${tabName.charAt(0).toUpperCase() + tabName.slice(1)}`)?.classList.add('active');
}

// ============================================
// WIDGETS PRÉ-DEFINIDOS COM SELEÇÃO DE TAMANHO
// ============================================

async function criarWidgetPredefinido(tipo, titulo, icon, url, cor) {
    // Abrir modal para escolher tamanho ANTES de criar
    abrirModalTamanhoWidget(tipo, titulo, icon, url, cor);
}

function abrirModalTamanhoWidget(tipo, titulo, icon, url, cor) {
    // Criar modal dinâmico
    const modalHTML = `
        <div class="modal active" id="modalTamanhoWidget" style="z-index: 10001;">
            <div class="modal-content" style="max-width: 32rem; animation: slideUp 0.3s ease;">
                
                <!-- Header -->
                <div class="modal-header" style="border-bottom: 1px solid var(--border-color); padding-bottom: 1rem; margin-bottom: 1.5rem;">
                    <div>
                        <h3 style="display: flex; align-items: center; gap: 0.75rem; margin: 0; font-size: 1.25rem;">
                            <i class="bi bi-grid-3x3-gap-fill" style="color: var(--accent-primary);"></i>
                            Escolher Tamanho
                        </h3>
                        <p style="color: var(--text-secondary); margin: 0.5rem 0 0 0; font-size: 0.875rem;">
                            Widget: <strong style="color: ${cor};">${titulo}</strong>
                        </p>
                    </div>
                    <button onclick="fecharModalTamanho()" class="modal-close" style="background: none; border: none; color: var(--text-secondary); cursor: pointer; font-size: 1.5rem; padding: 0.5rem; border-radius: 0.5rem; transition: all 0.2s;">
                        <i class="bi bi-x-lg"></i>
                    </button>
                </div>
                
                <!-- Body -->
                <div class="modal-body">
                    
                    <!-- Preview Widget -->
                    <div style="background: ${cor}; border-radius: 1rem; padding: 1.5rem; margin-bottom: 1.5rem; position: relative; overflow: hidden;">
                        <div style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; background: linear-gradient(135deg, transparent 0%, rgba(0,0,0,0.2) 100%);"></div>
                        <div style="position: relative; z-index: 1;">
                            <i class="bi ${icon}" style="font-size: 2.5rem; color: white; opacity: 0.9;"></i>
                            <h4 style="color: white; margin: 0.75rem 0 0 0; font-size: 1.125rem;">${titulo}</h4>
                        </div>
                    </div>
                    
                    <!-- Largura -->
                    <div class="form-group" style="margin-bottom: 1.25rem;">
                        <label class="form-label" style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.75rem; font-weight: 600; color: var(--text-primary);">
                            <i class="bi bi-arrows-expand" style="color: var(--accent-primary);"></i>
                            Largura (Colunas)
                        </label>
                        <select id="widgetCols" class="form-input" style="width: 100%; padding: 0.75rem 1rem; background: var(--bg-card); border: 1px solid var(--border-color); border-radius: 0.5rem; color: var(--text-primary); font-size: 0.9375rem; cursor: pointer;">
                            <option value="1">🟦 1 Coluna - Pequeno (33% largura)</option>
                            <option value="2" selected>🟦🟦 2 Colunas - Médio (66% largura)</option>
                            <option value="3">🟦🟦🟦 3 Colunas - Grande (100% largura)</option>
                        </select>
                    </div>
                    
                    <!-- Altura -->
                    <div class="form-group">
                        <label class="form-label" style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.75rem; font-weight: 600; color: var(--text-primary);">
                            <i class="bi bi-arrows-vertical" style="color: var(--accent-primary);"></i>
                            Altura (Linhas)
                        </label>
                        <select id="widgetRows" class="form-input" style="width: 100%; padding: 0.75rem 1rem; background: var(--bg-card); border: 1px solid var(--border-color); border-radius: 0.5rem; color: var(--text-primary); font-size: 0.9375rem; cursor: pointer;">
                            <option value="1">📏 1 Linha - Baixo</option>
                            <option value="2" selected>📏📏 2 Linhas - Médio</option>
                            <option value="3">📏📏📏 3 Linhas - Alto</option>
                        </select>
                    </div>
                    
                </div>
                
                <!-- Footer -->
                <div class="modal-footer" style="display: flex; gap: 0.75rem; margin-top: 1.5rem; padding-top: 1.5rem; border-top: 1px solid var(--border-color);">
                    <button onclick="confirmarCriacaoWidget('${tipo}', '${titulo}', '${icon}', '${url}', '${cor}')" 
                            class="btn btn-primary" 
                            style="flex: 1; display: flex; align-items: center; justify-content: center; gap: 0.5rem; padding: 0.875rem 1.5rem; background: var(--accent-primary); color: white; border: none; border-radius: 0.5rem; font-weight: 600; cursor: pointer; transition: all 0.2s;">
                        <i class="bi bi-plus-circle-fill"></i>
                        Criar Widget
                    </button>
                    <button onclick="fecharModalTamanho()" 
                            class="btn btn-secondary" 
                            style="padding: 0.875rem 1.5rem; background: var(--bg-card); color: var(--text-primary); border: 1px solid var(--border-color); border-radius: 0.5rem; font-weight: 600; cursor: pointer; transition: all 0.2s;">
                        <i class="bi bi-x-lg"></i>
                        Cancelar
                    </button>
                </div>
                
            </div>
        </div>
    `;

    // Inserir no body
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    // Adicionar estilos hover dinâmicos
    const style = document.createElement('style');
    style.id = 'modal-tamanho-styles';
    style.textContent = `
        #modalTamanhoWidget .modal-close:hover {
            background: rgba(239, 68, 68, 0.1) !important;
            color: var(--danger) !important;
        }
        #modalTamanhoWidget .btn-primary:hover {
            background: var(--accent-primary-hover) !important;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
        }
        #modalTamanhoWidget .btn-secondary:hover {
            background: rgba(59, 130, 246, 0.1) !important;
            border-color: var(--accent-primary) !important;
        }
        #modalTamanhoWidget .form-input:focus {
            outline: none;
            border-color: var(--accent-primary);
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
        }
    `;
    document.head.appendChild(style);
}

function fecharModalTamanho() {
    const modal = document.getElementById('modalTamanhoWidget');
    const styles = document.getElementById('modal-tamanho-styles');

    if (modal) {
        modal.style.animation = 'fadeOut 0.3s ease';
        setTimeout(() => {
            modal.remove();
            if (styles) styles.remove();
        }, 300);
    }
}

async function confirmarCriacaoWidget(tipo, titulo, icon, url, cor) {
    const cols = document.getElementById('widgetCols').value;
    const rows = document.getElementById('widgetRows').value;

    mostrarLoader();
    fecharModalTamanho();
    fecharModalWidget(); // Fecha o modal principal também

    const formData = new FormData();
    formData.append('Id', '0');
    formData.append('Titulo', titulo);
    formData.append('Link', url);
    formData.append('TipoWidget', tipo);
    formData.append('Colunas', cols);
    formData.append('Largura', rows);
    formData.append('CorFundo', cor);
    formData.append('Descricao', '');
    formData.append('ImagemUrl', '');

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const response = await fetch('/Dashboard/SalvarWidget', {
            method: 'POST',
            headers: { 'RequestVerificationToken': token },
            body: formData
        });

        if (response.ok) {
            mostrarNotificacao(`Widget "${titulo}" criado!`, 'success');
            setTimeout(() => window.location.reload(), 500);
        } else {
            throw new Error('Erro ao criar widget');
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao criar widget', 'error');
    } finally {
        ocultarLoader();
    }
}


// ============================================
// PREVIEW DE IMAGEM (COM DEBOUNCE)
// ============================================

let previewTimeout;

function previewImagem() {
    clearTimeout(previewTimeout);

    previewTimeout = setTimeout(() => {
        const imagemUrlInput = document.getElementById('imagemUrl');
        const imagemPreview = document.getElementById('imagemPreview');
        const imagemPreviewImg = document.getElementById('imagemPreviewImg');
        const corFundoInput = document.getElementById('corFundo');
        const url = imagemUrlInput.value.trim();

        // Remover erros anteriores
        document.querySelectorAll('.erro-imagem').forEach(e => e.remove());

        if (url) {
            try {
                new URL(url);

                imagemPreviewImg.src = url;
                imagemPreview.style.display = 'block';
                corFundoInput.style.opacity = '0.5';

                imagemPreviewImg.onerror = function () {
                    const erroMsg = document.createElement('div');
                    erroMsg.className = 'erro-imagem';
                    erroMsg.innerHTML = '<i class="bi bi-exclamation-circle-fill"></i> URL de imagem inválida';
                    erroMsg.style.cssText = `
                        color: var(--danger);
                        font-size: 0.875rem;
                        margin-top: 0.5rem;
                        padding: 0.75rem 1rem;
                        background: rgba(239, 68, 68, 0.1);
                        border: 1px solid rgba(239, 68, 68, 0.3);
                        border-radius: 0.5rem;
                        display: flex;
                        align-items: center;
                        gap: 0.5rem;
                    `;
                    imagemUrlInput.parentElement.appendChild(erroMsg);
                    imagemPreview.style.display = 'none';
                };

            } catch (error) {
                const erroMsg = document.createElement('div');
                erroMsg.className = 'erro-imagem';
                erroMsg.textContent = 'URL inválida';
                erroMsg.style.color = 'var(--danger)';
                imagemUrlInput.parentElement.appendChild(erroMsg);
            }
        } else {
            imagemPreview.style.display = 'none';
            corFundoInput.style.opacity = '1';
        }
    }, CONFIG.DEBOUNCE_DELAY);
}

// ============================================
// EDITAR WIDGET - CORRIGIDO (BUG DE DUPLICAÇÃO)
// ============================================

async function editarWidget(id) {
    try {
        mostrarLoader();

        const response = await fetch(`/Dashboard/ObterWidget/${id}`);
        if (!response.ok) throw new Error('Erro ao carregar widget');

        const widget = await response.json();

        // ✅ ABRIR MODAL MANUALMENTE (sem chamar abrirModalWidget)
        const modal = document.getElementById('modalWidget');
        modal.classList.add('active');

        // ✅ MUDAR PARA ABA PERSONALIZADO
        switchTab('personalizado');

        // ✅ MUDAR TÍTULO PARA "EDITAR"
        document.getElementById('modalTitle').innerHTML = '<i class="bi bi-pencil-square"></i> Editar Widget';

        // ✅ AGORA SIM, PREENCHER OS CAMPOS (depois de abrir o modal)
        document.getElementById('widgetId').value = widget.id;  // ID > 0 para EDIÇÃO
        document.getElementById('titulo').value = widget.titulo || '';
        document.getElementById('descricao').value = widget.descricao || '';
        document.getElementById('imagemUrl').value = widget.imagemUrl || '';
        document.getElementById('link').value = widget.link || '';
        document.getElementById('colunas').value = widget.colunas || 1;
        document.getElementById('largura').value = widget.largura || 1;
        document.getElementById('corFundo').value = widget.corFundo || '#1b1d29';

        // TipoWidget
        const tipoWidget = document.getElementById('tipoWidget');
        if (tipoWidget) {
            tipoWidget.value = widget.tipoWidget || 'custom';
        }

        // Preview da imagem se houver
        if (widget.imagemUrl) {
            setTimeout(() => previewImagem(), 100);
        }

        ocultarLoader();

        console.log('✅ Widget carregado para EDIÇÃO (ID=' + widget.id + ')');

    } catch (error) {
        console.error('Erro ao carregar widget:', error);
        ocultarLoader();
        mostrarNotificacao('Erro ao carregar widget', 'error');
    }
}

// ============================================
// DRAG & DROP SISTEMA COMPLETO
// ============================================

function inicializarDragDrop() {
    const widgets = document.querySelectorAll('.widget-card');
    const grid = document.getElementById('widgetsGrid');

    widgets.forEach(widget => {
        const isPinned = widget.dataset.pinned === 'true';

        if (!isPinned) {
            widget.draggable = true;
            widget.addEventListener('dragstart', handleDragStart);
            widget.addEventListener('dragend', handleDragEnd);
        }
    });

    if (grid) {
        grid.addEventListener('dragover', handleDragOver);
        grid.addEventListener('drop', handleDrop);
    }
}

function handleDragStart(e) {
    AppState.draggedWidget = this;
    AppState.isDragging = true;

    this.classList.add('dragging');
    e.dataTransfer.effectAllowed = 'move';

    document.querySelectorAll('.widget-card:not(.dragging)').forEach(w => {
        w.classList.add('drag-target');
    });
}

function handleDragEnd(e) {
    this.classList.remove('dragging');
    AppState.isDragging = false;

    document.querySelectorAll('.widget-card').forEach(w => {
        w.classList.remove('drag-over', 'drag-target');
    });
}

function handleDragOver(e) {
    if (e.preventDefault) e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    return false;
}

function handleDrop(e) {
    if (e.stopPropagation) e.stopPropagation();

    if (AppState.draggedWidget) {
        const allWidgets = Array.from(document.querySelectorAll('.widget-card'));
        const draggedIndex = allWidgets.indexOf(AppState.draggedWidget);
        const dropTarget = e.target.closest('.widget-card');

        if (dropTarget && dropTarget !== AppState.draggedWidget) {
            const dropIndex = allWidgets.indexOf(dropTarget);

            if (draggedIndex < dropIndex) {
                dropTarget.parentNode.insertBefore(AppState.draggedWidget, dropTarget.nextSibling);
            } else {
                dropTarget.parentNode.insertBefore(AppState.draggedWidget, dropTarget);
            }
        }

        salvarPosicoesWidgets();
    }

    return false;
}

async function salvarPosicoesWidgets() {
    const widgets = document.querySelectorAll('.widget-card');
    const posicoes = [];

    widgets.forEach((widget, index) => {
        posicoes.push({
            Id: parseInt(widget.dataset.id),
            X: index % CONFIG.GRID_COLS,
            Y: Math.floor(index / CONFIG.GRID_COLS),
            ZIndex: index
        });
    });

    try {
        const response = await fetch('/Dashboard/AtualizarPosicoes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(posicoes)
        });

        if (response.ok) {
            mostrarNotificacao('Posições salvas!', 'success');
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao salvar posições', 'error');
    }
}

// ============================================
// FIXAR/OCULTAR WIDGETS
// ============================================

async function toggleFixar(id) {
    try {
        const response = await fetch(`/Dashboard/FixarWidget?id=${id}`, {
            method: 'POST'
        });

        const result = await response.json();

        if (result.success) {
            const widget = document.querySelector(`[data-id="${id}"]`);
            widget.dataset.pinned = result.isPinned;

            if (result.isPinned) {
                widget.draggable = false;
                widget.classList.add('widget-pinned');
                mostrarNotificacao('Widget fixado!', 'success');
            } else {
                widget.draggable = true;
                widget.classList.remove('widget-pinned');
                mostrarNotificacao('Widget liberado!', 'success');
            }

            setTimeout(() => location.reload(), 1000);
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao fixar widget', 'error');
    }
}

async function toggleVisibilidade(id) {
    try {
        const response = await fetch(`/Dashboard/ToggleVisibilidade?id=${id}`, {
            method: 'POST'
        });

        const result = await response.json();

        if (result.success) {
            const widget = document.querySelector(`[data-id="${id}"]`);
            widget.style.animation = 'fadeOut 0.3s ease';

            setTimeout(() => {
                widget.remove();
                mostrarNotificacao('Widget ocultado!', 'success');
            }, 300);
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao ocultar widget', 'error');
    }
}

// ============================================
// USER HUB - DROPDOWN & TOGGLE PF/PJ
// ============================================

function toggleDropdown() {
    const dropdown = document.getElementById('userDropdown');
    dropdown.classList.toggle('active');
}

function togglePerfil(tipo) {
    // Feedback visual imediato
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.classList.remove('active');
    });

    const btnAtivo = document.querySelector(`[data-type="${tipo}"]`);
    if (btnAtivo) btnAtivo.classList.add('active');

    // Salvar no backend (Cookie + Session + Banco)
    fetch('/Contexto/DefinirContexto', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(tipo)
    })
        .then(r => r.json())
        .then(data => {
            console.log('Contexto salvo:', data);
            mostrarNotificacao(`Modo ${tipo.toUpperCase()} ativado`, 'success');
            setTimeout(() => window.location.reload(), 500);
        })
        .catch(err => {
            console.error('Erro ao salvar contexto:', err);
            mostrarNotificacao('Erro ao salvar preferência', 'error');
        });
}

//function abrirNotificacoes() {
//    mostrarNotificacao('Sistema de notificações em breve!', 'info');
//} <--- Essa função foi movida para notificacoes.js

// ============================================
// UPLOAD DE AVATAR
// ============================================

function inicializarAvatarUpload() {
    const avatarContainer = document.getElementById('avatarContainer');
    if (!avatarContainer) return;

    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.style.display = 'none';
    fileInput.id = 'avatarFileInput';
    document.body.appendChild(fileInput);

    avatarContainer.addEventListener('click', () => fileInput.click());

    fileInput.addEventListener('change', async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        if (file.size > CONFIG.MAX_IMAGE_SIZE) {
            mostrarNotificacao('Imagem deve ter no máximo 2MB!', 'error');
            return;
        }

        if (!file.type.startsWith('image/')) {
            mostrarNotificacao('Selecione uma imagem válida!', 'error');
            return;
        }

        const reader = new FileReader();
        reader.onload = (event) => {
            document.getElementById('avatarImage').src = event.target.result;
        };
        reader.readAsDataURL(file);

        await uploadFotoPerfil(file);
    });
}

async function uploadFotoPerfil(file) {
    const formData = new FormData();
    formData.append('foto', file);

    try {
        mostrarLoader();

        const response = await fetch('/Usuarios/UploadFotoPerfil', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) throw new Error('Erro ao fazer upload');

        const result = await response.json();

        if (result.success) {
            document.getElementById('avatarImage').src = result.caminhoFoto;
            mostrarNotificacao('Foto atualizada!', 'success');
        }

        ocultarLoader();

    } catch (error) {
        console.error('Erro:', error);
        ocultarLoader();
        mostrarNotificacao('Erro ao fazer upload', 'error');
    }
}

// ============================================
// NEWSLETTER
// ============================================

function verificarNewsletter() {
    const jaViu = localStorage.getItem('newsletter-decidido');
    const jaViuSession = sessionStorage.getItem('newsletter-visto');

    if (!jaViuSession && !jaViu) {
        setTimeout(() => {
            abrirModalNewsletter();
            sessionStorage.setItem('newsletter-visto', 'visto');
        }, 2000); // Abre 2s após carregar
    }
}

function abrirModalNewsletter() {
    document.getElementById('modalNewsletter').classList.add('active');
    document.getElementById('newsletterBemVindo').style.display = 'block';
    document.getElementById('newsletterForm').style.display = 'none';
}

function fecharModalNewsletter() {
    document.getElementById('modalNewsletter').classList.remove('active');
}

function mostrarFormNewsletter() {
    document.getElementById('newsletterBemVindo').style.display = 'none';
    document.getElementById('newsletterForm').style.display = 'block';
}

function recusarNewsletter() {
    localStorage.setItem('newsletter-decidido', 'recusado');
    fecharModalNewsletter();
    mostrarNotificacao('Você não verá este aviso novamente', 'info');
}

async function inscreverNewsletter(event) {
    event.preventDefault();

    const nome = document.getElementById('newsletterNome').value;
    const email = document.getElementById('newsletterEmail').value;

    // TODO: Enviar para backend
    console.log('Newsletter:', { nome, email });

    localStorage.setItem('newsletter-decidido', 'inscrito');
    fecharModalNewsletter();
    mostrarNotificacao('Inscrito com sucesso!', 'success');
}

// ============================================
// NOTIFICAÇÕES
// ============================================

function mostrarNotificacao(mensagem, tipo = 'success') {
    const iconMap = {
        success: 'bi-check-circle-fill',
        error: 'bi-x-circle-fill',
        warning: 'bi-exclamation-triangle-fill',
        info: 'bi-info-circle-fill'
    };

    const notificacao = document.createElement('div');
    notificacao.className = `notificacao notificacao-${tipo}`;
    notificacao.innerHTML = `
        <div style="display: flex; align-items: center; gap: 0.75rem;">
            <i class="bi ${iconMap[tipo]}" style="font-size: 1.25rem;"></i>
            <span>${mensagem}</span>
        </div>
    `;

    notificacao.style.cssText = `
        position: fixed;
        top: 5.5rem;
        right: 1.5rem;
        padding: 1rem 1.5rem;
        border-radius: 0.75rem;
        z-index: 10000;
        font-weight: 600;
        font-size: 0.9375rem;
        box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
        backdrop-filter: blur(10px);
        animation: slideInRight 0.3s ease;
        min-width: 18rem;
    `;

    const colors = {
        success: { bg: 'rgba(16, 185, 129, 0.15)', border: 'rgba(16, 185, 129, 0.4)', color: '#10b981' },
        error: { bg: 'rgba(239, 68, 68, 0.15)', border: 'rgba(239, 68, 68, 0.4)', color: '#ef4444' },
        warning: { bg: 'rgba(245, 158, 11, 0.15)', border: 'rgba(245, 158, 11, 0.4)', color: '#f59e0b' },
        info: { bg: 'rgba(6, 182, 212, 0.15)', border: 'rgba(6, 182, 212, 0.4)', color: '#06b6d4' }
    };

    const style = colors[tipo];
    notificacao.style.background = style.bg;
    notificacao.style.border = `2px solid ${style.border}`;
    notificacao.style.color = style.color;

    document.body.appendChild(notificacao);

    setTimeout(() => {
        notificacao.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => notificacao.remove(), 300);
    }, CONFIG.NOTIFICATION_DURATION);
}

// ============================================
// LOADER
// ============================================

function mostrarLoader() {
    let loader = document.getElementById('globalLoader');

    if (!loader) {
        loader = document.createElement('div');
        loader.id = 'globalLoader';
        loader.innerHTML = `
            <div class="loader-spinner"></div>
            <p style="color: var(--text-primary); margin-top: 1rem; font-weight: 600;">Carregando...</p>
        `;
        loader.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            backdrop-filter: blur(5px);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            z-index: 99999;
        `;
        document.body.appendChild(loader);
    }

    loader.style.display = 'flex';
}

function ocultarLoader() {
    const loader = document.getElementById('globalLoader');
    if (loader) loader.style.display = 'none';
}

// ============================================
// VALIDAÇÃO DE FORMULÁRIO
// ============================================

const formWidget = document.getElementById('formWidget');
if (formWidget) {
    formWidget.addEventListener('submit', (e) => {
        const titulo = document.getElementById('titulo').value.trim();

        if (!titulo) {
            e.preventDefault();
            mostrarNotificacao('O título é obrigatório!', 'error');
            document.getElementById('titulo').focus();
            return;
        }

        const imagemUrl = document.getElementById('imagemUrl').value.trim();
        if (imagemUrl) {
            try {
                new URL(imagemUrl);
            } catch (error) {
                e.preventDefault();
                mostrarNotificacao('URL de imagem inválida!', 'error');
                document.getElementById('imagemUrl').focus();
                return;
            }
        }
    });
}

// ============================================
// SISTEMA DE TEMAS
// ============================================

function abrirModalTemas() {
    document.getElementById('modalTemas').classList.add('active');
    carregarTemasCustomizados();
    marcarTemaAtivo();
}

function fecharModalTemas() {
    document.getElementById('modalTemas').classList.remove('active');
}

function abrirModalCriarTema() {
    document.getElementById('modalCriarTema').classList.add('active');
    document.getElementById('tituloModalTema').innerHTML = '<i class="bi bi-brush-fill"></i> Criar Tema Customizado';
    limparFormularioTema();
}

function fecharModalCriarTema() {
    document.getElementById('modalCriarTema').classList.remove('active');
}

function limparFormularioTema() {
    document.getElementById('temaId').value = '0';
    document.getElementById('nomeTema').value = '';
    document.getElementById('bgPrimary').value = '#10111a';
    document.getElementById('bgSecondary').value = '#0d0e16';
    document.getElementById('bgCard').value = '#1b1d29';
    document.getElementById('borderColor').value = '#2a2c3c';
    document.getElementById('textPrimary').value = '#f5f5ff';
    document.getElementById('textSecondary').value = '#aaaaaa';
    document.getElementById('accentPrimary').value = '#3b82f6';
    document.getElementById('accentPrimaryHover').value = '#2563eb';
    document.getElementById('btnPrimaryText').value = '#ffffff';
}

async function aplicarTemaPadrao(nomeTema) {
    try {
        const root = document.documentElement;
        ['--bg-primary', '--bg-secondary', '--bg-card', '--border-color', '--text-primary',
            '--text-secondary', '--accent-primary', '--accent-primary-hover', '--btn-primary-text']
            .forEach(prop => root.style.removeProperty(prop));

        const response = await fetch('/Theme/AplicarTemaPadrao', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(nomeTema)
        });

        if (response.ok) {
            carregarCssTema(nomeTema);
            await marcarTemaAtivo();
            mostrarNotificacao('Tema aplicado!', 'success');
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao aplicar tema', 'error');
    }
}

function carregarCssTema(nomeTema) {
    const temaAnterior = document.getElementById('tema-css');
    if (temaAnterior) temaAnterior.remove();

    const link = document.createElement('link');
    link.id = 'tema-css';
    link.rel = 'stylesheet';
    link.href = `/css/themes/theme-${nomeTema}.css`;
    document.head.appendChild(link);

    localStorage.setItem('tema-atual', nomeTema);
}

async function carregarTemasCustomizados() {
    try {
        const response = await fetch('/Theme/ListarTemas');
        const temas = await response.json();

        const container = document.getElementById('temasCustomizados');
        container.innerHTML = '';

        if (temas.length === 0) {
            container.innerHTML = '<p style="color: var(--text-secondary); text-align: center; grid-column: 1 / -1;">Nenhum tema customizado ainda</p>';
            return;
        }

        temas.forEach(tema => {
            const temaCard = document.createElement('div');
            temaCard.className = `tema-card${tema.isAtivo ? ' active' : ''}`;
            temaCard.onclick = () => ativarTemaCustomizado(tema.id);

            temaCard.innerHTML = `
                <div class="tema-preview" style="background: linear-gradient(135deg, ${tema.accentPrimary}, ${tema.accentPrimary}40);"></div>
                <span class="tema-nome">${tema.nomeTema}</span>
                <div class="tema-actions" onclick="event.stopPropagation();">
                    <button onclick="editarTemaCustomizado(${tema.id})"><i class="bi bi-pencil-fill"></i></button>
                    <button onclick="confirmarDeletarTema(${tema.id})" class="btn-delete-tema"><i class="bi bi-trash-fill"></i></button>
                </div>
            `;

            container.appendChild(temaCard);
        });
    } catch (error) {
        console.error('Erro:', error);
    }
}

async function ativarTemaCustomizado(temaId) {
    try {
        const response = await fetch('/Theme/AtivarTema', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(temaId)
        });

        if (response.ok) {
            const temaResponse = await fetch('/Theme/ObterTemaAtivo');
            const tema = await temaResponse.json();
            aplicarCoresCustomizadas(tema);
            marcarTemaAtivo();
            mostrarNotificacao('Tema aplicado!', 'success');
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao ativar tema', 'error');
    }
}

function aplicarCoresCustomizadas(tema) {
    const root = document.documentElement;
    root.style.setProperty('--bg-primary', tema.bgPrimary);
    root.style.setProperty('--bg-secondary', tema.bgSecondary);
    root.style.setProperty('--bg-card', tema.bgCard);
    root.style.setProperty('--border-color', tema.borderColor);
    root.style.setProperty('--text-primary', tema.textPrimary);
    root.style.setProperty('--text-secondary', tema.textSecondary);
    root.style.setProperty('--accent-primary', tema.accentPrimary);
    root.style.setProperty('--accent-primary-hover', tema.accentPrimaryHover);
    root.style.setProperty('--btn-primary-text', tema.btnPrimaryText);

    const temaAnterior = document.getElementById('tema-css');
    if (temaAnterior) temaAnterior.remove();

    localStorage.removeItem('tema-atual');
}

async function salvarTema(event) {
    event.preventDefault();

    const tema = {
        Id: parseInt(document.getElementById('temaId').value),
        NomeTema: document.getElementById('nomeTema').value,
        BgPrimary: document.getElementById('bgPrimary').value,
        BgSecondary: document.getElementById('bgSecondary').value,
        BgCard: document.getElementById('bgCard').value,
        BorderColor: document.getElementById('borderColor').value,
        TextPrimary: document.getElementById('textPrimary').value,
        TextSecondary: document.getElementById('textSecondary').value,
        AccentPrimary: document.getElementById('accentPrimary').value,
        AccentPrimaryHover: document.getElementById('accentPrimaryHover').value,
        BtnPrimaryText: document.getElementById('btnPrimaryText').value
    };

    try {
        const response = await fetch('/Theme/SalvarTema', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(tema)
        });

        if (response.ok) {
            const result = await response.json();
            fecharModalCriarTema();

            if (result.temaId) {
                await ativarTemaCustomizado(result.temaId);
            }

            carregarTemasCustomizados();
            mostrarNotificacao('Tema salvo!', 'success');
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao salvar tema', 'error');
    }
}

async function editarTemaCustomizado(temaId) {
    try {
        const response = await fetch('/Theme/ListarTemas');
        const temas = await response.json();
        const tema = temas.find(t => t.id === temaId);

        if (!tema) {
            mostrarNotificacao('Tema não encontrado', 'error');
            return;
        }

        document.getElementById('temaId').value = tema.id;
        document.getElementById('nomeTema').value = tema.nomeTema;
        document.getElementById('tituloModalTema').innerHTML = '<i class="bi bi-pencil-fill"></i> Editar Tema';
        abrirModalCriarTema();

    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao carregar tema', 'error');
    }
}

function confirmarDeletarTema(temaId) {
    if (confirm('Tem certeza que deseja excluir este tema?')) {
        deletarTema(temaId);
    }
}

async function deletarTema(temaId) {
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch('/Theme/DeletarTema', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `id=${temaId}&__RequestVerificationToken=${token}`
        });

        if (response.ok) {
            carregarTemasCustomizados();
            aplicarTemaPadrao('default');
            mostrarNotificacao('Tema excluído!', 'success');
        }
    } catch (error) {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao excluir tema', 'error');
    }
}

async function marcarTemaAtivo() {
    try {
        document.querySelectorAll('.tema-card').forEach(card => card.classList.remove('active'));

        const response = await fetch('/Theme/ObterTemaAtivo');
        const tema = await response.json();

        if (tema.id) {
            const temaCard = document.querySelector(`.tema-card[data-tema-id="${tema.id}"]`);
            if (temaCard) temaCard.classList.add('active');
        } else {
            const temaAtual = localStorage.getItem('tema-atual') || 'default';
            const temaCard = document.querySelector(`.tema-card[data-theme="${temaAtual}"]`);
            if (temaCard) temaCard.classList.add('active');
        }
    } catch (error) {
        console.error('Erro:', error);
    }
}

async function carregarTemaInicial() {
    try {
        const response = await fetch('/Theme/ObterTemaAtivo');
        const tema = await response.json();

        if (tema.id) {
            aplicarCoresCustomizadas(tema);
        } else {
            const temaSalvo = localStorage.getItem('tema-atual');
            if (temaSalvo && temaSalvo !== 'default') {
                carregarCssTema(temaSalvo);
            }
        }
    } catch (error) {
        console.error('Erro ao carregar tema:', error);
    }
}

// ============================================
// ATALHOS DE TECLADO
// ============================================

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal.active').forEach(modal => {
            modal.classList.remove('active');
        });
    }

    if (e.ctrlKey && e.key === 'n') {
        e.preventDefault();
        abrirModalWidget();
    }

    if (e.ctrlKey && e.key === 't') {
        e.preventDefault();
        abrirModalTemas();
    }
});