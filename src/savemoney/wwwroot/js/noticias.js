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
  let currentQuery = ""; // Seletores do DOM

  const container = document.getElementById("noticias-container");
  const searchInput = document.querySelector(".search-input");
  const loadMoreButton = document.querySelector(".carregar-mais-button button");
  const searchButton = document.querySelector(".action-search-button");
  const suggestionsContainer = document.getElementById("search-suggestions");

  async function carregarNoticias(query, page, isLoadMore = false) {
    try {
      const baseUrl = "/Noticias/GetNoticias";
      const params = new URLSearchParams();
      params.append("query", query);
      params.append("page", page);
      const apiUrl = `${baseUrl}?${params.toString()}`;

      console.log("Buscando notícias com URL:", apiUrl);

      const response = await fetch(apiUrl);

      if (!response.ok) {
        throw new Error(
          `Erro na API: ${response.statusText} (Status: ${response.status})`
        );
      }

      const dadosCompletos = await response.json();

      if (!dadosCompletos || !Array.isArray(dadosCompletos.articles)) {
        throw new Error(
          "A resposta da API não continha a propriedade 'articles' esperada."
        );
      }

      if (dadosCompletos.status !== "ok") {
        container.innerHTML =
          '<div class="alert alert-danger text-center">Não foi possível carregar as notícias no momento.</div>';
        return;
      }

      const listaDeNoticias = dadosCompletos.articles;

      if (listaDeNoticias.length === 0) {
        if (isLoadMore) {
          console.log("Não há mais notícias para carregar.");
          loadMoreButton.style.display = "none"; // Esconde o botão
          return;
        } else {
          container.innerHTML =
            '<div class="alert alert-info text-center">Nenhuma notícia encontrada no momento.</div>';
          return;
        }
      } else {
        // Garante que o botão reapareça se houver mais notícias
        loadMoreButton.style.display = "block";
      }

      if (!isLoadMore) {
        container.innerHTML = "";
      }

      listaDeNoticias.forEach((noticia) => {
        const title = noticia.title || "Sem título";
        const description =
          noticia.description || "Clique para ler mais na fonte original.";
        const url = noticia.url || "#";
        const imageUrl =
          noticia.urlToImage ||
          "https://placehold.co/600x400/6a0dad/white?text=Noticia";

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
      container.innerHTML = `<div class="alert alert-danger text-center">Desculpe, não foi possível carregar as notícias. Verifique a consola (F12) para mais detalhes do erro.</div>`;
    }
  } // --- LÓGICA DE SUGESTÕES (PASSO 6) ---

  async function fetchSuggestions(query) {
    if (query.length < 2) {
      // Otimização: não buscar por 1 letra
      clearSuggestions();
      return;
    }
    try {
      // ATENÇÃO: /Noticias/GetSugestoes deve ser criado no seu Controller
      const response = await fetch(
        `/Noticias/GetSugestoes?query=${encodeURIComponent(query)}`
      );
      if (!response.ok) throw new Error("API de sugestões falhou");
      const suggestions = await response.json(); // Espera um array: ["sugestao1", "sugestao2"]
      renderSuggestions(suggestions);
    } catch (error) {
      console.error("Erro ao buscar sugestões:", error);
      clearSuggestions();
    }
  }

  function renderSuggestions(suggestions) {
    if (suggestions.length === 0) {
      clearSuggestions();
      return;
    } // Usa 'suggestion-item' para CSS e para o clique
    suggestionsContainer.innerHTML = suggestions
      .map((s) => `<div class="suggestion-item">${s}</div>`)
      .join("");
  }

  function clearSuggestions() {
    suggestionsContainer.innerHTML = "";
  } // --- MANIPULADORES DE EVENTOS (PASSOS 4, 5, 7) ---

  function handleSearchInput(event) {
    const query = event.target.value.trim();
    currentQuery = query;
    currentPage = 1;

    console.log(`Busca (debounced) iniciada para: "${query}", Página: 1`);
    carregarNoticias(query, 1, false);
    fetchSuggestions(query); // Busca sugestões ao mesmo tempo
  }

  function handleLoadMore() {
    currentPage++;
    console.log(`Carregando mais: "${currentQuery}", Página: ${currentPage}`);
    carregarNoticias(currentQuery, currentPage, true);
  }

  // Manipulador para o clique no botão "Buscar" e "Enter"
  function handleExplicitSearch() {
    const query = searchInput.value.trim();
    currentQuery = query;
    currentPage = 1;

    console.log(`Busca EXPLÍCITA para: "${currentQuery}", Página: 1`);
    carregarNoticias(currentQuery, currentPage, false);
    clearSuggestions(); // Limpa sugestões após busca explícita
  } // --- CONEXÃO DOS EVENT LISTENERS --- // "Live Search" (debounced)

  const debouncedSearchHandler = debounce(handleSearchInput, 500);
  searchInput.addEventListener("input", debouncedSearchHandler); // "Carregar Mais"

  loadMoreButton.addEventListener("click", handleLoadMore); // "Busca Explícita" (Botão)

  searchButton.addEventListener("click", handleExplicitSearch); // "Busca Explícita" (Tecla Enter)

  searchInput.addEventListener("keyup", function (event) {
    if (event.key === "Enter") {
      handleExplicitSearch();
    }
  }); // Clique na Sugestão (Event Delegation)

  suggestionsContainer.addEventListener("click", function (event) {
    if (event.target && event.target.classList.contains("suggestion-item")) {
      const clickedSuggestion = event.target.textContent;
      searchInput.value = clickedSuggestion; // Coloca o texto na barra
      handleExplicitSearch(); // Executa a busca imediatamente
    }
  }); // Carga Inicial

  carregarNoticias(currentQuery, currentPage);
});
