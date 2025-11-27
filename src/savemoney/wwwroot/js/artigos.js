document.addEventListener('DOMContentLoaded', () => {

    /* ==========================================
       Estado da Aplicação
       ========================================== */
    let currentPage = 1;
    const PAGE_SIZE = 6;
    let isLoading = false;
    let hasMoreData = true;

    /* ==========================================
       Elementos DOM
       ========================================== */
    const container = document.getElementById('artigos-container');
    const spinner = document.getElementById('loading-spinner');
    const statusMessageContainer = document.getElementById('status-message-container');
    const statusMessage = document.getElementById('status-message');
    const formBusca = document.getElementById('form-busca');
    const filtroTexto = document.getElementById('filtro-texto');
    const filtroRegiao = document.getElementById('filtro-regiao');
    const filtroOrdem = document.getElementById('filtro-ordem');
    const btnPesquisar = document.getElementById('btn-pesquisar');
    const paginationContainer = document.getElementById('pagination-container');
    const btnExibirMais = document.getElementById('btn-exibir-mais');

    /* ==========================================
       Event Listeners
       ========================================== */
    if (formBusca) {
        formBusca.addEventListener('submit', (e) => {
            e.preventDefault();
            iniciarNovaBusca();
        });
    }

    if (btnExibirMais) {
        btnExibirMais.addEventListener('click', carregarMaisArtigos);
    }

    /* ==========================================
       Funções Principais
       ========================================== */

    function iniciarNovaBusca() {
        currentPage = 1;
        hasMoreData = true;
        carregarArtigos(true);
    }

    function carregarMaisArtigos() {
        currentPage++;
        carregarArtigos(false);
    }

    async function carregarArtigos(isNewSearch = false) {
        // Verificações de estado
        if (isLoading || (!hasMoreData && !isNewSearch)) return;

        const apiUrl = '/Artigos/GetArtigos';

        // Construir parâmetros da query
        const params = new URLSearchParams({
            searchTerm: filtroTexto.value.trim(),
            region: filtroRegiao.value,
            sortOrder: filtroOrdem.value,
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

            // Validar resposta
            if (!dadosCompletos || dadosCompletos.status !== "ok" || !Array.isArray(dadosCompletos.articles)) {
                console.error("Resposta da API inválida:", dadosCompletos);
                const mensagem = dadosCompletos.message || "Erro na comunicação com a API.";
                mostrarStatus(mensagem, true);
                return;
            }

            const listaDeArtigos = dadosCompletos.articles;

            // Atualizar estado de paginação
            hasMoreData = dadosCompletos.hasNextPage || (listaDeArtigos.length === PAGE_SIZE);

            // Renderizar artigos
            renderizarArtigos(listaDeArtigos, isNewSearch);

        } catch (error) {
            console.error("Falha ao carregar artigos:", error);
            mostrarStatus(
                "Ocorreu um erro inesperado ao buscar os artigos. Verifique a conexão ou tente novamente.",
                true
            );
        } finally {
            setLoadingState(false);
            atualizarVisibilidadePaginacao();
        }
    }

    /* ==========================================
       Funções de UI
       ========================================== */

    function setLoadingState(loading, isNewSearch = false) {
        isLoading = loading;

        // Atualizar estado dos botões
        btnPesquisar.disabled = loading;
        btnExibirMais.disabled = loading;

        // Atualizar ARIA
        container.setAttribute('aria-busy', loading);

        if (loading) {
            spinner.style.display = 'flex';
            mostrarStatus(null);

            if (isNewSearch) {
                container.innerHTML = '';
                paginationContainer.style.display = 'none';
            }

            if (currentPage > 1) {
                btnExibirMais.textContent = "Carregando...";
            }
        } else {
            spinner.style.display = 'none';
            btnExibirMais.textContent = "Exibir Mais Artigos";
        }
    }

    function renderizarArtigos(artigos, isNewSearch) {
        if (isNewSearch) {
            container.innerHTML = '';
        }

        if (artigos.length === 0 && currentPage === 1) {
            mostrarStatus("Nenhum artigo encontrado para os critérios de busca selecionados.", false);
            return;
        }

        artigos.forEach(artigo => {
            const artigoProcessado = processarArtigo(artigo);
            container.insertAdjacentHTML('beforeend', criarCardArtigo(artigoProcessado));
        });
    }

    function mostrarStatus(mensagem, isError = false) {
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
        if (hasMoreData && container.children.length > 0) {
            paginationContainer.style.display = 'flex';
        } else {
            paginationContainer.style.display = 'none';
        }
    }

    /* ==========================================
       Processamento de Dados
       ========================================== */

    function processarArtigo(artigo) {
        let anoPublicacao = "Ano desc.";

        // Processar data com segurança
        if (artigo.publicationDate) {
            try {
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

    function criarCa