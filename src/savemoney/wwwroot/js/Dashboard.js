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
    document.getElementById('link').value = '';
    document.getElementById('colunas').value = '1';
    document.getElementById('largura').value = '1';
    document.getElementById('corFundo').value = '#1b1d29';
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

    if (event.target === modalWidget) {
        fecharModalWidget();
    }

    if (event.target === modalConfirmar) {
        fecharModalConfirmar();
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
        document.getElementById('link').value = widget.link || '';
        document.getElementById('colunas').value = widget.colunas;
        document.getElementById('largura').value = widget.largura;
        document.getElementById('corFundo').value = widget.corFundo;

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

            // Mostrar mensagem de sucesso (opcional)
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

    // Estilos inline (ou adicionar no CSS)
    notificacao.style.position = 'fixed';
    notificacao.style.top = '20px';
    notificacao.style.right = '20px';
    notificacao.style.padding = '1rem 1.5rem';
    notificacao.style.borderRadius = '8px';
    notificacao.style.zIndex = '9999';
    notificacao.style.animation = 'slideInRight 0.3s ease';

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
            transform: translateX(400px);
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
            transform: translateX(400px);
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

        // Se chegou aqui, submit normal (servidor processa)
    });
}

// ============ PREVIEW DE COR ============

const corFundoInput = document.getElementById('corFundo');
if (corFundoInput) {
    corFundoInput.addEventListener('input', (e) => {
        // Opcional: mostrar preview da cor escolhida
        const previewText = document.querySelector('.color-preview-text');
        if (previewText) {
            previewText.textContent = e.target.value;
        }
    });
}

// ============ CONSOLE LOG ============

console.log('Dashboard.js carregado com sucesso! 🚀');
console.log('Atalhos disponíveis:');
console.log('- ESC: Fechar modais');
console.log('- Ctrl+N: Novo widget');