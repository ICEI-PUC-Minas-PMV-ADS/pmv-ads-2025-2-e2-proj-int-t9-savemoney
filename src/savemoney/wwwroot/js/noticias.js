/* ============================================================================
   NOTÍCIAS DO MERCADO FINANCEIRO
   ============================================================================ */

(() => {
    'use strict';

    // Estado da aplicação
    let currentPage = 1;
    let currentQuery = '';
    let isLoading = false;
    let debounceTimer = null;

    // Elementos do DOM
    const container = document.getElementById('noticias-container');
    const searchInput = document.getElementById('searchInput');
    const btnCarregarMais = document.getElementById('btnCarregarMais');
    const loadingState = document.getElementById('loading-state');

    // Configurações
    const CONFIG = {
        debounceDelay: 500,
        pageSize: 10,
        apiEndpoint: '/Noticias/GetNoticias',
        placeholderImage: 'https://placehold.co/600x400/3b82f6/white?text=Sem+Imagem'
    };

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeNoticias);

    function initializeNoticias() {
        console.log('Noticias module loaded');

        if (!container || !searchInput || !btnCarregarMais) {
            console.error('Elementos necessários não encontrados');
            return;
        }

        setupEventListeners();
        carregarNoticias(currentQuery, currentPage, false);
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Search input com debounce
        searchInput.addEventListener('input', handleSearchInput);

        // Botão carregar mais
        btnCarregarMais.addEventListener('click', handleLoadMore);

        // Enter no search
        searchInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                clearTimeout(debounceTimer);
                handleSearch();
            }
        });
    }

    /* Handlers
       ======================================================================== */
    function handleSearchInput(event) {
        const query = event.target.value.trim();

        // Debounce
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            handleSearch(query);
        }, CONFIG.debounceDelay);
    }

    function handleSearch(query = '') {
        currentQuery = query;
        currentPage = 1;

        console.log(`Busca iniciada para: "${query}"`);
        carregarNoticias(currentQuery, currentPage, false);
    }

    function handleLoadMore() {
        currentPage++;
        carregarNoticias(currentQuery, currentPage, true);
    }

    /* Carregar Notícias - Função Principal
       ======================================================================== */
    async function carregarNoticias(query, page, isLoadMore = false) {
        if (isLoading) {
            console.log('Já está carregando...');
            return;
        }

        isLoading = true;
        setLoadingState(isLoadMore);

        try {
            const noticias = await fetchNoticias(query, page);

            if (noticias.length === 0) {
                handleEmptyResult(isLoadMore);
                return;
            }

            renderNoticias(noticias, isLoadMore);
            updateLoadMoreButton(noticias.length);

        } catch (error) {
            handleError(error);
        } finally {
            isLoading = false;
            resetLoadingState();
        }
    }

    /* Fetch API
       ======================================================================== */
    async function fetchNoticias(query, page) {
        const params = new URLSearchParams({
            query: query || '',
            page: page.toString()
        });

        const url = `${CONFIG.apiEndpoint}?${params.toString()}`;
        console.log('Fetching:', url);

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();

        // Validação da resposta
        if (!data || data.status !== 'ok') {
            throw new Error('Resposta inválida da API');
        }

        if (!Array.isArray(data.articles)) {
            throw new Error('Formato de resposta inesperado');
        }

        return data.articles;
    }

    /* Renderização
       ======================================================================== */
    function renderNoticias(noticias, isLoadMore) {
        // Limpa container se não for "carregar mais"
        if (!isLoadMore) {
            container.innerHTML = '';
            container.setAttribute('aria-busy', 'false');
        }

        // Renderiza cada notícia
        noticias.forEach((noticia, index) => {
            const card = createNoticiaCard(noticia);
            container.appendChild(card);

            // Animação de entrada
            if ('IntersectionObserver' in window) {
                animateCardEntry(card, index);
            }
        });

        console.log(`${noticias.length} notícias carregadas`);
    }

    function createNoticiaCard(noticia) {
        const card = document.createElement('a');
        card.className = 'noticia-card';
        card.href = noticia.url || '#';
        card.target = '_blank';
        card.rel = 'noopener noreferrer';
        card.setAttribute('role', 'listitem');

        const title = sanitizeText(noticia.title || 'Sem título');
        const description = sanitizeText(noticia.description || 'Clique para ler mais na fonte original.');
        const imageUrl = noticia.urlToImage || CONFIG.placeholderImage;

        card.innerHTML = `
            <img src="${imageUrl}" 
                 class="noticia-card-image" 
                 alt="${title}"
                 loading="lazy"
                 onerror="this.src='${CONFIG.placeholderImage}'">
            <div class="noticia-card-content">
                <h2 class="noticia-card-title">${title}</h2>
                <p class="noticia-card-description">${description}</p>
            </div>
        `;

        return card;
    }

    /* Animações
       ======================================================================== */
    function animateCardEntry(card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    setTimeout(() => {
                        entry.target.style.opacity = '1';
                        entry.target.style.transform = 'translateY(0)';
                    }, index * 50);
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '50px'
        });

        observer.observe(card);
    }

    /* Estados da UI
       ======================================================================== */
    function setLoadingState(isLoadMore) {
        container.setAttribute('aria-busy', 'true');

        if (isLoadMore) {
            btnCarregarMais.disabled = true;
            btnCarregarMais.innerHTML = `
                <span class="material-symbols-outlined spinning">sync</span>
                Carregando...
            `;
        } else {
            if (loadingState) {
                loadingState.style.display = 'flex';
            }
        }

        // Adiciona animação de rotação
        addSpinnerAnimation();
    }

    function resetLoadingState() {
        container.setAttribute('aria-busy', 'false');

        if (loadingState) {
            loadingState.style.display = 'none';
        }

        btnCarregarMais.disabled = false;
        btnCarregarMais.innerHTML = `
            <span class="material-symbols-outlined">add</span>
            Carregar Mais Notícias
        `;
    }

    function updateLoadMoreButton(loadedCount) {
        if (loadedCount < CONFIG.pageSize) {
            btnCarregarMais.style.display = 'none';
        } else {
            btnCarregarMais.style.display = 'block';
        }
    }

    function addSpinnerAnimation() {
        if (document.querySelector('.spinning-animation')) return;

        const style = document.createElement('style');
        style.className = 'spinning-animation';
        style.textContent = `
            @keyframes spin-icon {
                from { transform: rotate(0deg); }
                to { transform: rotate(360deg); }
            }
            .spinning {
                display: inline-block;
                animation: spin-icon 1s linear infinite;
            }
        `;
        document.head.appendChild(style);
    }

    /* Tratamento de Erros e Estados Vazios
       ======================================================================== */
    function handleEmptyResult(isLoadMore) {
        if (isLoadMore) {
            console.log('Não há mais notícias para carregar');
            btnCarregarMais.style.display = 'none';
            return;
        }

        container.innerHTML = `
            <div class="empty-state">
                <span class="material-symbols-outlined">inbox</span>
                <h2>Nenhuma notícia encontrada</h2>
                <p>Tente pesquisar por outro termo ou volte mais tarde</p>
            </div>
        `;
        btnCarregarMais.style.display = 'none';
    }

    function handleError(error) {
        console.error('Erro ao carregar notícias:', error);

        const errorMessage = getErrorMessage(error);

        container.innerHTML = `
            <div class="alert alert-danger">
                <span class="material-symbols-outlined">error</span>
                <div>
                    <strong>Erro ao carregar notícias</strong>
                    <p>${errorMessage}</p>
                </div>
            </div>
        `;

        btnCarregarMais.style.display = 'none';
    }

    function getErrorMessage(error) {
        if (error.message.includes('HTTP')) {
            return 'Erro de conexão com o servidor. Tente novamente mais tarde.';
        }
        if (error.message.includes('API')) {
            return 'A API de notícias está temporariamente indisponível.';
        }
        return 'Não foi possível carregar as notícias. Verifique sua conexão.';
    }

    /* Utilitários
       ======================================================================== */
    function sanitizeText(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    /* Smooth Scroll para Links Internos
       ======================================================================== */
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

            if (href === '#' || href === '#!') return;

            e.preventDefault();

            const target = document.querySelector(href);
            if (target) {
                const headerOffset = 100;
                const elementPosition = target.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.scrollY - headerOffset;

                window.scrollTo({
                    top: offsetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });

})();