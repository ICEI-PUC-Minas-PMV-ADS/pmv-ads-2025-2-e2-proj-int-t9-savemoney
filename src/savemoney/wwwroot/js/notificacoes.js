/**
 * Gerenciador Central de Notificações (R11)
 * Responsável por buscar, renderizar e gerenciar interações do usuário.
 */
const notificacoesManager = {
    isOpen: false,
    pollInterval: null,

    // Inicialização do componente
    init: function () {
        // 1. Busca inicial do contador
        this.atualizarContador();

        // 2. Configura atualização automática a cada 60 segundos (Polling)
        // Isso garante que alertas de orçamento/contas apareçam mesmo sem refresh
        this.pollInterval = setInterval(() => this.atualizarContador(), 60000);

        // 3. Listener para fechar ao clicar fora
        document.addEventListener('click', (e) => {
            const container = document.querySelector('.notification-container');
            if (this.isOpen && container && !container.contains(e.target)) {
                this.close();
            }
        });
    },

    // Alterna visibilidade do dropdown
    toggle: function (event) {
        if (event) event.stopPropagation();

        const dropdown = document.getElementById('notification-dropdown');

        if (this.isOpen) {
            this.close();
        } else {
            dropdown.style.display = 'block';
            this.carregarLista(); // Busca o conteúdo atualizado
            this.isOpen = true;
        }
    },

    // Fecha o dropdown
    close: function () {
        const dropdown = document.getElementById('notification-dropdown');
        if (dropdown) {
            dropdown.style.display = 'none';
            this.isOpen = false;
        }
    },

    // Busca apenas o número de não lidas para o badge (leve)
    atualizarContador: function () {
        fetch('/Notificacoes/ObterRecentes')
            .then(response => {
                if (!response.ok) throw new Error('Erro na rede');
                return response.json();
            })
            .then(data => {
                const badge = document.getElementById('badge-contador');
                if (badge) {
                    if (data.naoLidas > 0) {
                        badge.innerText = data.naoLidas > 99 ? '99+' : data.naoLidas;
                        badge.style.display = 'flex';
                        badge.classList.add('animate-pop'); // Efeito visual
                    } else {
                        badge.style.display = 'none';
                    }
                }
            })
            .catch(err => console.warn('Falha silenciosa ao buscar notificações:', err));
    },

    // Busca a lista completa e renderiza o HTML
    carregarLista: function () {
        const listContainer = document.getElementById('notification-list');
        // Loading state
        listContainer.innerHTML = `
            <div class="d-flex justify-content-center align-items-center p-4">
                <div class="spinner-border spinner-border-sm text-primary" role="status"></div>
            </div>`;

        fetch('/Notificacoes/ObterRecentes')
            .then(response => response.json())
            .then(data => {
                listContainer.innerHTML = ''; // Limpa loading

                if (!data.notificacoes || data.notificacoes.length === 0) {
                    listContainer.innerHTML = `
                        <div class="notification-empty">
                            <i class="bi bi-bell-slash"></i>
                            <p class="m-0 small">Nenhuma notificação no momento.</p>
                        </div>`;
                    return;
                }

                // Renderiza cada item
                data.notificacoes.forEach(notif => {
                    const itemHtml = this.construirHtmlItem(notif);
                    listContainer.appendChild(itemHtml);
                });
            })
            .catch(err => {
                listContainer.innerHTML = '<div class="p-3 text-center text-danger small">Erro ao carregar. Tente novamente.</div>';
                console.error(err);
            });
    },

    // Constrói o elemento DOM para uma notificação
    construirHtmlItem: function (notif) {
        const div = document.createElement('div');
        // Adiciona classe baseada no tipo (ex: type-AlertaOrcamento) para coloração CSS
        div.className = `notification-item type-${notif.tipo} ${notif.lida ? '' : 'unread'}`;
        div.dataset.id = notif.id;

        // Ícone baseado no Tipo (R11)
        let iconClass = 'bi-info-circle';
        switch (notif.tipo) {
            case 'AlertaOrcamento': iconClass = 'bi-exclamation-triangle-fill'; break;
            case 'ContaPendente': iconClass = 'bi-calendar-x-fill'; break;
            case 'Sucesso': iconClass = 'bi-check-circle-fill'; break;
            case 'Erro': iconClass = 'bi-x-circle-fill'; break;
            case 'Sistema': iconClass = 'bi-cpu-fill'; break;
        }

        // Link de Ação (se houver)
        let linkHtml = '';
        if (notif.linkAcao) {
            linkHtml = `<a href="${notif.linkAcao}" class="notif-link">Ver detalhes <i class="bi bi-arrow-right-short"></i></a>`;
        }

        div.innerHTML = `
            <div class="notif-icon">
                <i class="bi ${iconClass}"></i>
            </div>
            <div class="notif-content">
                <div class="notif-title">
                    <span>${notif.titulo}</span>
                    <span class="notif-date">${notif.data}</span>
                </div>
                <div class="notif-body">
                    ${notif.mensagem}
                    ${linkHtml}
                </div>
            </div>
            <button class="btn-delete-notif" onclick="notificacoesManager.excluir(${notif.id}, event)" title="Excluir">
                <i class="bi bi-x"></i>
            </button>
        `;

        // Evento: Marcar como lida ao passar o mouse
        div.addEventListener('mouseenter', () => {
            if (!notif.lida) {
                this.marcarComoLida(notif.id, div);
            }
        });

        return div;
    },

    // Ações de API
    marcarComoLida: function (id, element) {
        // Evita múltiplas chamadas se já estiver lida visualmente
        if (!element.classList.contains('unread')) return;

        fetch(`/Notificacoes/MarcarComoLida?id=${id}`, { method: 'POST' })
            .then(res => {
                if (res.ok) {
                    element.classList.remove('unread');
                    this.atualizarContador(); // Atualiza badge
                }
            });
    },

    marcarTodasLidas: function () {
        fetch('/Notificacoes/MarcarTodasComoLidas', { method: 'POST' })
            .then(res => {
                if (res.ok) {
                    // Atualiza visualmente sem recarregar tudo
                    document.querySelectorAll('.notification-item.unread').forEach(el => {
                        el.classList.remove('unread');
                    });
                    this.atualizarContador();
                }
            });
    },

    excluir: function (id, event) {
        if (event) event.stopPropagation(); // Não fecha o dropdown

        // Feedback visual imediato (Otimistic UI update)
        const item = document.querySelector(`.notification-item[data-id="${id}"]`);
        if (item) {
            item.style.opacity = '0.5'; // Indica processamento
        }

        fetch(`/Notificacoes/Excluir?id=${id}`, { method: 'POST' })
            .then(res => {
                if (res.ok) {
                    if (item) {
                        // Animação de saída
                        item.style.transform = 'translateX(20px)';
                        item.style.opacity = '0';
                        setTimeout(() => {
                            item.remove();
                            // Verifica se ficou vazio
                            const list = document.getElementById('notification-list');
                            if (list.children.length === 0) {
                                list.innerHTML = `<div class="notification-empty"><p class="m-0 small">Nenhuma notificação.</p></div>`;
                            }
                        }, 200);
                    }
                    this.atualizarContador();
                }
            });
    }
};

// Inicializa quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', () => {
    notificacoesManager.init();
});