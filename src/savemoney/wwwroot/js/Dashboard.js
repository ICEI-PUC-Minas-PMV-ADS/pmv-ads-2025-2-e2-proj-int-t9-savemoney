// ============ CONTROLE DE MODAIS ============

// Modal de Widget
function abrirModalWidget() {
    document.getElementById('modalWidget').style.display = 'block';
    document.getElementById('modalTitle').textContent = 'Adicionar Widget';
    limparFormulario();
}

function fecharModalWidget() {
    document.getElementById('modalWidget').style.display = 'none';
    limparFormulario();
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

    // Limpar preview
    const preview = document.getElementById('imagemPreview');
    if (preview) {
        preview.style.display = 'none';
    }
}

// Modal de Confirmação
function confirmarDeletar(id) {
    document.getElementById('deletarId').value = id;
    document.getElementById('modalConfirmar').style.display = 'block';
}

function fecharModalConfirmar() {
    document.getElementById('modalConfirmar').style.display = 'none';
}

// Fechar modal ao clicar fora
window.onclick = function (event) {
    const modalWidget = document.getElementById('modalWidget');
    const modalConfirmar = document.getElementById('modalConfirmar');
    const modalTemas = document.getElementById('modalTemas');
    const modalCriarTema = document.getElementById('modalCriarTema');

    if (event.target === modalWidget) {
        fecharModalWidget();
    }

    if (event.target === modalConfirmar) {
        fecharModalConfirmar();
    }

    if (event.target === modalTemas) {
        fecharModalTemas();
    }

    if (event.target === modalCriarTema) {
        fecharModalCriarTema();
    }
}

// ============ PREVIEW DE IMAGEM ============

function previewImagem() {
    const imagemUrlInput = document.getElementById('imagemUrl');
    const imagemPreview = document.getElementById('imagemPreview');
    const imagemPreviewImg = document.getElementById('imagemPreviewImg');
    const corFundoInput = document.getElementById('corFundo');

    const url = imagemUrlInput.value.trim();

    // Remover mensagem de erro anterior
    const erroAnterior = imagemUrlInput.parentElement.querySelector('.erro-imagem');
    if (erroAnterior) {
        erroAnterior.remove();
    }

    if (url) {
        // Mostrar preview
        imagemPreviewImg.src = url;
        imagemPreview.style.display = 'block';

        // Desabilitar cor de fundo
        corFundoInput.style.opacity = '0.5';
        corFundoInput.title = 'Cor de fundo não será usada quando houver imagem';

        // Validar se a imagem carrega
        imagemPreviewImg.onerror = function () {
            // Criar mensagem de erro inline
            const erroMsg = document.createElement('div');
            erroMsg.className = 'erro-imagem';
            erroMsg.textContent = '❌ URL de imagem inválida ou inacessível';
            erroMsg.style.color = '#ef4444';
            erroMsg.style.fontSize = '0.875rem';
            erroMsg.style.marginTop = '0.5rem';
            erroMsg.style.padding = '0.5rem';
            erroMsg.style.background = 'rgba(239, 68, 68, 0.1)';
            erroMsg.style.border = '1px solid rgba(239, 68, 68, 0.3)';
            erroMsg.style.borderRadius = '0.5rem';

            imagemUrlInput.parentElement.appendChild(erroMsg);
            imagemPreview.style.display = 'none';
        };

        imagemPreviewImg.onload = function () {
            // Sucesso - não precisa fazer nada
        };
    } else {
        // Esconder preview
        imagemPreview.style.display = 'none';
        corFundoInput.style.opacity = '1';
        corFundoInput.title = '';
    }
}

// ============ EDITAR WIDGET ============

async function editarWidget(id) {
    try {
        const response = await fetch(`/Dashboard/ObterWidget/${id}`);

        if (!response.ok) {
            throw new Error('Erro ao carregar widget');
        }

        const widget = await response.json();

        // Preencher o formulário
        document.getElementById('widgetId').value = widget.id;
        document.getElementById('titulo').value = widget.titulo;
        document.getElementById('descricao').value = widget.descricao || '';
        document.getElementById('imagemUrl').value = widget.imagemUrl || '';
        document.getElementById('link').value = widget.link || '';
        document.getElementById('colunas').value = widget.colunas;
        document.getElementById('largura').value = widget.largura;
        document.getElementById('corFundo').value = widget.corFundo;

        // Preview da imagem se houver
        if (widget.imagemUrl) {
            previewImagem();
        }

        // Abrir modal
        document.getElementById('modalTitle').textContent = 'Editar Widget';
        document.getElementById('modalWidget').style.display = 'block';

    } catch (error) {
        console.error('Erro:', error);
        alert('Erro ao carregar dados do widget');
    }
}

// ============ UPLOAD DE FOTO DE PERFIL ============

// Criar input file invisível
const avatarContainer = document.getElementById('avatarContainer');
if (avatarContainer) {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.style.display = 'none';
    fileInput.id = 'avatarFileInput';
    document.body.appendChild(fileInput);

    // Clicar no avatar abre seleção de arquivo
    avatarContainer.addEventListener('click', () => {
        fileInput.click();
    });

    // Quando selecionar arquivo
    fileInput.addEventListener('change', async (e) => {
        const file = e.target.files[0];

        if (!file) return;

        // Validar tamanho (2MB)
        const maxSize = 2 * 1024 * 1024; // 2MB em bytes
        if (file.size > maxSize) {
            alert('A imagem deve ter no máximo 2MB!');
            return;
        }

        // Validar tipo
        if (!file.type.startsWith('image/')) {
            alert('Por favor, selecione uma imagem válida!');
            return;
        }

        // Preview da imagem
        const reader = new FileReader();
        reader.onload = (event) => {
            document.getElementById('avatarImage').src = event.target.result;
        };
        reader.readAsDataURL(file);

        // Upload para o servidor
        await uploadFotoPerfil(file);
    });
}

async function uploadFotoPerfil(file) {
    const formData = new FormData();
    formData.append('foto', file);

    try {
        const response = await fetch('/Usuarios/UploadFotoPerfil', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            throw new Error('Erro ao fazer upload');
        }

        const result = await response.json();

        if (result.success) {
            // Atualizar a imagem
            document.getElementById('avatarImage').src = result.caminhoFoto;

            // Mostrar mensagem de sucesso
            mostrarNotificacao('Foto atualizada com sucesso!', 'success');
        } else {
            throw new Error(result.message || 'Erro ao salvar foto');
        }

    } catch (error) {
        console.error('Erro:', error);
        alert('Erro ao fazer upload da foto: ' + error.message);
    }
}

// ============ NOTIFICAÇÕES ============

function mostrarNotificacao(mensagem, tipo = 'success') {
    // Criar elemento de notificação
    const notificacao = document.createElement('div');
    notificacao.className = `notificacao notificacao-${tipo}`;
    notificacao.textContent = mensagem;

    // Estilos inline
    notificacao.style.position = 'fixed';
    notificacao.style.top = '1.25rem';
    notificacao.style.right = '1.25rem';
    notificacao.style.padding = '1rem 1.5rem';
    notificacao.style.borderRadius = '0.5rem';
    notificacao.style.zIndex = '9999';
    notificacao.style.animation = 'slideInRight 0.3s ease';
    notificacao.style.fontWeight = '600';
    notificacao.style.fontSize = '0.9375rem';

    if (tipo === 'success') {
        notificacao.style.background = 'rgba(16, 185, 129, 0.2)';
        notificacao.style.border = '1px solid rgba(16, 185, 129, 0.4)';
        notificacao.style.color = '#10b981';
    } else if (tipo === 'error') {
        notificacao.style.background = 'rgba(239, 68, 68, 0.2)';
        notificacao.style.border = '1px solid rgba(239, 68, 68, 0.4)';
        notificacao.style.color = '#ef4444';
    }

    document.body.appendChild(notificacao);

    // Remover após 3 segundos
    setTimeout(() => {
        notificacao.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => {
            notificacao.remove();
        }, 300);
    }, 3000);
}

// Adicionar animações de notificação
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(25rem);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(25rem);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// ============ ATALHOS DE TECLADO ============

document.addEventListener('keydown', (e) => {
    // ESC fecha modais
    if (e.key === 'Escape') {
        fecharModalWidget();
        fecharModalConfirmar();
        fecharModalTemas();
        fecharModalCriarTema();
    }

    // Ctrl + N abre modal de novo widget
    if (e.ctrlKey && e.key === 'n') {
        e.preventDefault();
        abrirModalWidget();
    }
});

// ============ VALIDAÇÃO DO FORMULÁRIO ============

const formWidget = document.getElementById('formWidget');
if (formWidget) {
    formWidget.addEventListener('submit', (e) => {
        const titulo = document.getElementById('titulo').value.trim();

        if (!titulo) {
            e.preventDefault();
            alert('O título é obrigatório!');
            document.getElementById('titulo').focus();
            return;
        }

        // Validar URL da imagem se preenchida
        const imagemUrl = document.getElementById('imagemUrl').value.trim();
        if (imagemUrl) {
            try {
                new URL(imagemUrl);
            } catch (error) {
                e.preventDefault();
                alert('Por favor, insira uma URL de imagem válida!');
                document.getElementById('imagemUrl').focus();
                return;
            }
        }
    });
}

// ============ SISTEMA DE TEMAS ============

// Modal de Temas
function abrirModalTemas() {
    document.getElementById('modalTemas').style.display = 'block';
    carregarTemasCustomizados();
    marcarTemaAtivo();
}

function fecharModalTemas() {
    document.getElementById('modalTemas').style.display = 'none';
}

// Modal Criar/Editar Tema
function abrirModalCriarTema() {
    document.getElementById('modalCriarTema').style.display = 'block';
    document.getElementById('tituloModalTema').textContent = 'Criar Tema Customizado';
    limparFormularioTema();
}

function fecharModalCriarTema() {
    document.getElementById('modalCriarTema').style.display = 'none';
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

// Aplicar Tema Pré-definido
// Aplicar Tema Pré-definido
async function aplicarTemaPadrao(nomeTema) {
    try {
        // LIMPAR cores customizadas primeiro
        const root = document.documentElement;
        root.style.removeProperty('--bg-primary');
        root.style.removeProperty('--bg-secondary');
        root.style.removeProperty('--bg-card');
        root.style.removeProperty('--border-color');
        root.style.removeProperty('--text-primary');
        root.style.removeProperty('--text-secondary');
        root.style.removeProperty('--accent-primary');
        root.style.removeProperty('--accent-primary-hover');
        root.style.removeProperty('--btn-primary-text');

        // Desativar temas customizados no backend
        const response = await fetch('/Theme/AplicarTemaPadrao', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(nomeTema)
        });

        if (response.ok) {
            // Carregar CSS do tema
            carregarCssTema(nomeTema);
            await marcarTemaAtivo();
            mostrarNotificacao('Tema aplicado com sucesso!', 'success');
        }
    } catch (error) {
        console.error('Erro ao aplicar tema:', error);
        mostrarNotificacao('Erro ao aplicar tema', 'error');
    }
}

// Carregar CSS do Tema
function carregarCssTema(nomeTema) {
    // Remover CSS de tema anterior
    const temaAnterior = document.getElementById('tema-css');
    if (temaAnterior) {
        temaAnterior.remove();
    }

    // Adicionar novo CSS
    const link = document.createElement('link');
    link.id = 'tema-css';
    link.rel = 'stylesheet';
    link.href = `/css/themes/theme-${nomeTema}.css`;
    document.head.appendChild(link);

    // Salvar no localStorage
    localStorage.setItem('tema-atual', nomeTema);
}

// Carregar Temas Customizados
async function carregarTemasCustomizados() {
    try {
        const response = await fetch('/Theme/ListarTemas');
        const temas = await response.json();

        const container = document.getElementById('temasCustomizados');
        container.innerHTML = '';

        if (temas.length === 0) {
            container.innerHTML = '<p style="color: #aaaaaa; text-align: center; grid-column: 1 / -1;">Nenhum tema customizado ainda</p>';
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
                    <button onclick="editarTemaCustomizado(${tema.id})">✏️</button>
                    <button onclick="confirmarDeletarTema(${tema.id})" class="btn-delete-tema">🗑️</button>
                </div>
            `;

            container.appendChild(temaCard);
        });
    } catch (error) {
        console.error('Erro ao carregar temas:', error);
    }
}

// Ativar Tema Customizado
async function ativarTemaCustomizado(temaId) {
    try {
        const response = await fetch('/Theme/AtivarTema', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(temaId)
        });

        if (response.ok) {
            // Carregar cores do tema
            const temaResponse = await fetch('/Theme/ObterTemaAtivo');
            const tema = await temaResponse.json();
            aplicarCoresCustomizadas(tema);
            marcarTemaAtivo();
            mostrarNotificacao('Tema aplicado com sucesso!', 'success');
        }
    } catch (error) {
        console.error('Erro ao ativar tema:', error);
        mostrarNotificacao('Erro ao ativar tema', 'error');
    }
}

// Aplicar Cores Customizadas
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

    // Remover CSS de tema pré-definido
    const temaAnterior = document.getElementById('tema-css');
    if (temaAnterior) {
        temaAnterior.remove();
    }

    localStorage.removeItem('tema-atual');
}

// Salvar Tema Customizado
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

            // Ativar o tema automaticamente após salvar
            if (result.temaId) {
                await ativarTemaCustomizado(result.temaId);
            }

            carregarTemasCustomizados();
            mostrarNotificacao('Tema salvo e aplicado com sucesso!', 'success');
        }
    } catch (error) {
        console.error('Erro ao salvar tema:', error);
        mostrarNotificacao('Erro ao salvar tema', 'error');
    }
}

// Editar Tema Customizado
async function editarTemaCustomizado(temaId) {
    try {
        const response = await fetch('/Theme/ListarTemas');
        const temas = await response.json();

        const tema = temas.find(t => t.id === temaId);

        if (!tema) {
            mostrarNotificacao('Tema não encontrado', 'error');
            return;
        }

        // Preencher formulário
        document.getElementById('temaId').value = tema.id;
        document.getElementById('nomeTema').value = tema.nomeTema;

        // Abrir modal
        document.getElementById('tituloModalTema').textContent = 'Editar Tema';
        document.getElementById('modalCriarTema').style.display = 'block';

    } catch (error) {
        console.error('Erro ao editar tema:', error);
        mostrarNotificacao('Erro ao carregar tema', 'error');
    }
}

// Confirmar Deletar Tema
function confirmarDeletarTema(temaId) {
    if (confirm('Tem certeza que deseja excluir este tema?')) {
        deletarTema(temaId);
    }
}

// Deletar Tema
async function deletarTema(temaId) {
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch('/Theme/DeletarTema', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `id=${temaId}&__RequestVerificationToken=${token}`
        });

        if (response.ok) {
            carregarTemasCustomizados();
            // Resetar para tema padrão
            aplicarTemaPadrao('default');
            mostrarNotificacao('Tema excluído com sucesso!', 'success');
        }
    } catch (error) {
        console.error('Erro ao deletar tema:', error);
        mostrarNotificacao('Erro ao excluir tema', 'error');
    }
}

// Resetar Tema de Emergência
async function resetarTemaEmergencia() {
    try {
        // Limpar todas as variáveis CSS customizadas
        const root = document.documentElement;
        root.style.removeProperty('--bg-primary');
        root.style.removeProperty('--bg-secondary');
        root.style.removeProperty('--bg-card');
        root.style.removeProperty('--border-color');
        root.style.removeProperty('--text-primary');
        root.style.removeProperty('--text-secondary');
        root.style.removeProperty('--accent-primary');
        root.style.removeProperty('--accent-primary-hover');
        root.style.removeProperty('--btn-primary-text');

        // Remover CSS de tema pré-definido
        const temaAnterior = document.getElementById('tema-css');
        if (temaAnterior) {
            temaAnterior.remove();
        }

        // Limpar localStorage
        localStorage.removeItem('tema-atual');

        // Desativar todos os temas customizados no backend
        await fetch('/Theme/AplicarTemaPadrao', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify('default')
        });

        mostrarNotificacao('Tema resetado para o padrão!', 'success');

        // Recarregar página após 1 segundo
        setTimeout(() => {
            window.location.reload();
        }, 1000);

    } catch (error) {
        console.error('Erro ao resetar tema:', error);
        // Forçar reload mesmo com erro
        window.location.reload();
    }
}

// Marcar Tema Ativo
async function marcarTemaAtivo() {
    try {
        // Remover active de todos
        document.querySelectorAll('.tema-card').forEach(card => {
            card.classList.remove('active');
        });

        // Verificar se tem tema customizado ativo
        const response = await fetch('/Theme/ObterTemaAtivo');
        const tema = await response.json();

        if (tema.id) {
            // Tema customizado está ativo
            const temaCard = document.querySelector(`.tema-card[data-tema-id="${tema.id}"]`);
            if (temaCard) {
                temaCard.classList.add('active');
            }
        } else {
            // Tema pré-definido está ativo
            const temaAtual = localStorage.getItem('tema-atual') || 'default';
            const temaCard = document.querySelector(`.tema-card[data-theme="${temaAtual}"]`);
            if (temaCard) {
                temaCard.classList.add('active');
            }
        }
    } catch (error) {
        console.error('Erro ao marcar tema ativo:', error);
    }
}

// Carregar Tema ao Iniciar
window.addEventListener('DOMContentLoaded', async () => {
    try {
        // Verificar se tem tema customizado ativo
        const response = await fetch('/Theme/ObterTemaAtivo');
        const tema = await response.json();

        if (tema.id) {
            // Tem tema customizado ativo
            aplicarCoresCustomizadas(tema);
        } else {
            // Carregar tema pré-definido do localStorage
            const temaSalvo = localStorage.getItem('tema-atual');
            if (temaSalvo && temaSalvo !== 'default') {
                carregarCssTema(temaSalvo);
            }
        }
    } catch (error) {
        console.error('Erro ao carregar tema inicial:', error);
    }
});

// ============ CONSOLE LOG ============
console.log('Sistema de Temas carregado! 🎨');
console.log('Dashboard.js carregado com sucesso! 🚀');
console.log('Atalhos disponíveis:');
console.log('- ESC: Fechar modais');
console.log('- Ctrl+N: Novo widget');