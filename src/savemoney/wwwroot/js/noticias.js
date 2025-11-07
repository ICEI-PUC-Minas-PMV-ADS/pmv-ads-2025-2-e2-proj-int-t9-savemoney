document.addEventListener("DOMContentLoaded", function () {
  function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const context = this;
      const later = function () {
        timeout = null;
        func.apply(context, args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }

  let currentPage = 1;
  let currentQuery = "";
  let isLoading = false; 

  const container = document.getElementById("noticias-container");
  const searchInput = document.querySelector(".search-input");
  const btnCarregarMais = document.getElementById("btnCarregarMais"); 

  async function carregarNoticias(query, page, isLoadMore = false) {
    if (isLoading) return; 
    isLoading = true;

    if (isLoadMore) {
      btnCarregarMais.disabled = true;
      btnCarregarMais.textContent = "Carregando...";
    }

    try {
      const baseUrl = "/Noticias/GetNoticias";
      const params = new URLSearchParams();
      params.append("query", query);
      params.append("page", page);
      const apiUrl = `${baseUrl}?${params.toString()}`;

      console.log("Buscando notícias com URL:", apiUrl);

      const response = await fetch(apiUrl);
      if (!response.ok) throw new Error(`Erro na API: ${response.statusText}`);

      const dadosCompletos = await response.json();
      if (!dadosCompletos || !Array.isArray(dadosCompletos.articles)) {
        throw new Error("A resposta da API não continha 'articles'.");
      }
      if (dadosCompletos.status !== "ok") {
        container.innerHTML = '<div class="alert alert-danger text-center">Não foi possível carregar as notícias.</div>';
        return;
      }

      const listaDeNoticias = dadosCompletos.articles;

      if (listaDeNoticias.length === 0) {
        btnCarregarMais.style.display = 'none'; 
        if (isLoadMore) {
          console.log("Não há mais notícias para carregar.");
        } else {
          container.innerHTML = '<div class="alert alert-info text-center">Nenhuma notícia encontrada no momento.</div>';
        }
        return;
      }

      if (!isLoadMore) {
        container.innerHTML = "";
      }

      if (listaDeNoticias.length < 10) {
        btnCarregarMais.style.display = 'none'; 
      } else {
        btnCarregarMais.style.display = 'block'; 
      }

      // Renderiza os cards
      listaDeNoticias.forEach((noticia) => {
        const title = noticia.title || "Sem título";
        const description = noticia.description || "Clique para ler mais na fonte original.";
        const url = noticia.url || "#";
        const imageUrl = noticia.urlToImage || "https://placehold.co/600x400/6a0dad/white?text=Noticia";

        const cardHTML = `
            <a href="${url}" target="_blank" rel="noopener noreferrer" class="meu-card">
                <img src="${imageUrl}" class="meu-card-imagem" alt="${title}">
                <div class="meu-card-conteudo">
                    <h5 class="meu-card-titulo">${title}</h5>
                    <p class="meu-card-descricao">${description}</p>
                </div>
            </a>
        `;
        container.innerHTML += cardHTML;
      });

    } catch (error) {
      console.error("Falha ao carregar notícias:", error);
      container.innerHTML = `<div class="alert alert-danger text-center">Desculpe, não foi possível carregar as notícias.</div>`;
    } finally {
      isLoading = false;
      if (isLoadMore) {
        btnCarregarMais.disabled = false;
        btnCarregarMais.textContent = "Carregar mais notícias";
      }
    }
  }
  
  function handleSearchInput(event) {
    const query = event.target.value.trim();
    currentQuery = query;
    currentPage = 1;

    console.log(`Busca (debounced) iniciada para: "${query}", Página: 1`);
    carregarNoticias(query, 1, false);
    
    
  }

  function handleLoadMore() {
    currentPage++;
    carregarNoticias(currentQuery, currentPage, true);
  }

  
  const debouncedSearchHandler = debounce(handleSearchInput, 500);
  searchInput.addEventListener("input", debouncedSearchHandler);

  btnCarregarMais.addEventListener("click", handleLoadMore);

  carregarNoticias(currentQuery, currentPage);
});