document.addEventListener("DOMContentLoaded", function () {
  async function carregarNoticias() {
    const container = document.getElementById("noticias-container");

    try {
      const apiUrl = "/Noticias/GetNoticias";

      const response = await fetch(apiUrl);

      if (!response.ok) {
        throw new Error(
          `Erro na API: ${response.statusText} (Status: ${response.status})`
        );
      }

      const dadosCompletos = await response.json();

      // Garante que a estrutura esperada está presente
      if (!dadosCompletos || !Array.isArray(dadosCompletos.articles)) {
        throw new Error(
          "A resposta da API não continha a propriedade 'articles' esperada."
        );
      }

      // Se status for error, mostra mensagem amigável
      if (dadosCompletos.status !== "ok") {
        container.innerHTML =
          '<div class="alert alert-danger text-center">Não foi possível carregar as notícias no momento.</div>';
        return;
      }

      const listaDeNoticias = dadosCompletos.articles;

      container.innerHTML = "";

      if (listaDeNoticias.length === 0) {
        container.innerHTML =
          '<div class="alert alert-info text-center">Nenhuma notícia encontrada no momento.</div>';
        return;
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
  }

  carregarNoticias();
});
