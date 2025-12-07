document.addEventListener('DOMContentLoaded', function () {

    // Estado da Aplicação
    let currentPage = 1;
    const PAGE_SIZE = 6; // Limite de 6 artigos por página
    let isLoading = false;
    let hasMoreData = true; // Controla a visibilidade do botão "Exibir Mais"

    // Elementos da UI
    const container = document.getElementById('artigos-container');
    const spinner = document.getElementById('loading-spinner');
    const statusMessageContainer = document.getElementById('status-message-container');
    const statusMessage = document.getElementById('status-message');
    const formBusca = document.getElementById('form-busca');
    const filtroTexto = document.getElementById('filtro-texto');
    const filtroRegiao = document.getElementById('filtro-regiao');
    const filtroOrdem = document.getElementById('filtro-ordem');
    const filtroTopico = document.getElementById('filtro-topico');
    const btnPesquisar = document.getElementById('btn-pesquisar');
    const paginationContainer = document.getElementById('pagination-container');
    const btnExibirMais = document.getElementById('btn-exibir-mais');

    // Event Listeners
    if (formBusca) formBusca.addEventListener('submit', iniciarNovaBusca);
    if (btnExibirMais) btnExibirMais.addEventListener('click', carregarMaisArtigos);

    // 1. Iniciar uma nova busca (acionado pelo botão ou Enter)
    function iniciarNovaBusca(event) {
        if (event) event.preventDefault();
        currentPage = 1; // Reseta a página para 1
        hasMoreData = true; // Reseta o estado da paginação
        carregarArtigos(true);
    }

    // 2. Carregar mais artigos (paginação)
    function carregarMaisArtigos() {
        currentPage++; // Incrementa a página
        carregarArtigos(false);
    }

    // 3. Função principal para carregar os dados da API
    // 3. Função principal para carregar os dados da API
    async function carregarArtigos(isNewSearch = false) {
        if (isLoading || (!hasMoreData && !isNewSearch)) return;

        const apiUrl = '/Artigos/GetArtigos';

        // FIX: Adicionamos verificação de nulidade (element ? element.value : '')
        // Se o elemento não existir no HTML, enviamos uma string vazia.
        const termValue = filtroTexto ? filtroTexto.value.trim() : '';
        const regionValue = filtroRegiao ? filtroRegiao.value : '';
        const sortValue = filtroOrdem ? filtroOrdem.value : '';
        const topicValue = filtroTopico ? filtroTopico.value : '';

        // Coleta os parâmetros dos filtros para enviar ao backend
        const params = new URLSearchParams({
            searchTerm: termValue,
            region: regionValue,
            sortOrder: sortValue,
            topic: topicValue,
            page: currentPage,
            pageSize: PAGE_SIZE
        });

        const fullUrl = `${apiUrl}?${params.toString()}`;

        setLoadingState(true, isNewSearch);

        try {
            const response = await fetch(fullUrl);

            if (!response.ok) {
                throw new Error(`Erro no servidor: ${response.statusText} (Status: ${response.status})`);
            }

            const dadosCompletos = await response.json();

            // Validação da resposta
            if (!dadosCompletos || dadosCompletos.status !== "ok" || !Array.isArray(dadosCompletos.articles)) {
                console.error("Resposta da API inválida:", dadosCompletos);
                const mensagem = dadosCompletos.message || "Erro na comunicação com a API.";
                mostrarStatus(mensagem, true);
                return;
            }

            const listaDeArtigos = dadosCompletos.articles;

            // Gerencia a paginação
            hasMoreData = dadosCompletos.hasNextPage || (listaDeArtigos.length === PAGE_SIZE);

            // Renderiza os resultados
            renderizarArtigos(listaDeArtigos, isNewSearch);

        } catch (error) {
            console.error("Falha ao carregar artigos:", error);
            mostrarStatus("Ocorreu um erro inesperado ao buscar os artigos. Verifique a conexão ou tente novamente.", true);
        } finally {
            setLoadingState(false);
            atualizarVisibilidadePaginacao();
        }
    }

    // 4. Funções de Renderização e Estado

    // Define o estado de carregamento (Spinner e Botões)
    function setLoadingState(loading, isNewSearch = false) {
        isLoading = loading;

        btnPesquisar.disabled = loading;
        btnExibirMais.disabled = loading;

        if (loading) {
            spinner.style.display = 'flex';
            mostrarStatus(null); // Esconde mensagens de erro/status anteriores
            if (isNewSearch) {
                // Limpa o container apenas se for uma nova busca
                container.innerHTML = '';
                paginationContainer.style.display = 'none';
            }
            // Altera o texto do botão durante o carregamento
            if (currentPage > 1) {
                btnExibirMais.textContent = "Carregando...";
            }
        } else {
            spinner.style.display = 'none';
            // Restaura o texto do botão
            btnExibirMais.textContent = "Exibir Mais Artigos";
        }
    }

    // Renderiza a lista de artigos no DOM
    function renderizarArtigos(artigos, isNewSearch) {
        if (isNewSearch) {
            container.innerHTML = '';
        }

        if (artigos.length === 0 && currentPage === 1) {
            mostrarStatus("Nenhum artigo encontrado para os critérios de busca selecionados.", false);
            return;
        }

        artigos.forEach(artigo => {
            // Processa os dados do artigo antes de criar o card
            const artigoProcessado = processarArtigo(artigo);
            container.innerHTML += criarCardArtigo(artigoProcessado);
        });
    }

    // Função auxiliar para mostrar/esconder mensagens de status
    function mostrarStatus(mensagem, isError = false) {
        // Remove classes de status anteriores
        statusMessage.classList.remove('status-error');

        if (mensagem) {
            statusMessage.textContent = mensagem;
            statusMessageContainer.style.display = 'block';
            if (isError) {
                statusMessage.classList.add('status-error');
            }
        } else {
            statusMessageContainer.style.display = 'none';
        }
    }

    function atualizarVisibilidadePaginacao() {
        // Mostra o botão apenas se houver mais dados E se já houver algum artigo na tela
        if (hasMoreData && container.children.length > 0) {
            paginationContainer.style.display = 'flex';
        } else {
            paginationContainer.style.display = 'none';
        }
    }

    // 5. Funções Auxiliares (Formatação e Criação de Cards)

    // Processa e formata os dados de um único artigo
    function processarArtigo(artigo) {
        let anoPublicacao = "Ano desc.";

        // Tratamento robusto da data
        if (artigo.publicationDate) {
            try {
                // Adicionar 'T00:00:00' força o JS a interpretar como meia-noite local, 
                // evitando erros de fuso horário ao analisar apenas a data (YYYY-MM-DD).
                const data = new Date(artigo.publicationDate + 'T00:00:00');
                if (!isNaN(data.getTime())) {
                    anoPublicacao = data.getFullYear();
                }
            } catch (e) {
                console.warn("Erro ao processar data:", artigo.publicationDate);
            }
        }

        const autoresFormatados = formatarAutores(artigo.authors);
        return {
            ...artigo,
            anoPublicacao,
            autoresFormatados
        };
    }

    // Função auxiliar para formatar autores
    function formatarAutores(autoresList) {
        if (!autoresList || autoresList.length === 0) {
            return "Autores não disponíveis";
        }
        const maxAutores = 2;
        if (autoresList.length > maxAutores) {
            return `${autoresList.slice(0, maxAutores).join(', ')} et al.`;
        }
        return autoresList.join(', ');
    }

    // Função auxiliar para criar o HTML de cada artigo
    function criarCardArtigo(artigo) {
        const title = artigo.title || "Sem título";
        const url = artigo.url || "#";
        const authorsFormatted = artigo.autoresFormatados;
        const source = artigo.source || "Fonte desconhecida";
        const citedByCount = artigo.citedByCount || 0;
        const publicationYear = artigo.anoPublicacao;

        // Trunca o abstract
        let abstractText = artigo.abstractText || "Abstract (Resumo) não disponível.";
        const maxLength = 160;
        if (abstractText.length > maxLength) {
            abstractText = abstractText.substring(0, maxLength) + '...';
        }

        const categoryTag = `Citações: ${citedByCount}`;

        const cardHTML = `
            <a href="${url}" target="_blank" rel="noopener noreferrer" class="article-card glass-effect">
                <span class="card-category">${categoryTag}</span>
                
                <h3 class="card-title">${title}</h3>
                
                <p class="card-meta">
                    ${authorsFormatted} • ${source}, ${publicationYear}
                </p>
                
                <p class="card-abstract">
                    ${abstractText}
                </p>
                
                <div class="card-footer-link">
                    Ler Artigo
                    <span class="material-symbols-outlined card-link-icon">arrow_forward</span>
                </div>
            </a>
        `;
        return cardHTML;
    }

    // Inicia o carregamento inicial (Página 1, com filtros padrão)
    carregarArtigos(true);
});