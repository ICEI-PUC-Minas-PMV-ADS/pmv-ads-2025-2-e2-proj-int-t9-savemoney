document.addEventListener('DOMContentLoaded', function () {

    async function carregarNoticias() {
        const container = document.getElementById('noticias-container');

        try {
            const apiUrl = '/EducacaoFinanceira/GetNoticias';

            const response = await fetch(apiUrl);

            if (!response.ok) {
                throw new Error(`Erro na API: ${response.statusText} (Status: ${response.status})`);
            }

            const dadosCompletos = await response.json();

            // Garante que a estrutura esperada est� presente
            if (!dadosCompletos || !Array.isArray(dadosCompletos.articles)) {
                throw new Error("A resposta da API n�o continha a propriedade 'articles' esperada.");
            }

            // Se status for error, mostra mensagem amig�vel
            if (dadosCompletos.status !== "ok") {
                container.innerHTML = '<div class="alert alert-danger text-center">N�o foi poss�vel carregar as not�cias no momento.</div>';
                return;
            }

            const listaDeNoticias = dadosCompletos.articles;

            container.innerHTML = '';

            if (listaDeNoticias.length === 0) {
                container.innerHTML = '<div class="alert alert-info text-center">Nenhuma not�cia encontrada no momento.</div>';
                return;
            }

            listaDeNoticias.forEach(noticia => {
                // Garante valores padr�o para todos os campos
                const title = noticia.title || "Sem t�tulo";
                const description = noticia.description || "Clique para ler mais na fonte original.";
                const url = noticia.url || "#";
                const imageUrl = noticia.urlToImage || 'https://placehold.co/600x400/6a0dad/white?text=Noticia';

                const cardHTML = `
                        <div class="col-12 col-sm-10 col-md-6 col-lg-4 d-flex align-items-stretch mx-auto">
                            <a href="${url}" target="_blank" rel="noopener noreferrer" class="card-link w-100">
                                <div class="card news-card h-100">
                                    <img src="${imageUrl}" class="card-img-top" alt="${title}">
                                    <div class="card-body d-flex flex-column">
                                        <h5 class="card-title">${title}</h5>
                                        <p class="card-text flex-grow-1">${description}</p>
                                    </div>
                                </div>
                            </a>
                        </div>
                    `;
                container.innerHTML += cardHTML;
            });

        } catch (error) {
            console.error("Falha ao carregar not�cias:", error);
            container.innerHTML = `<div class="alert alert-danger text-center">Desculpe, n�o foi poss�vel carregar as not�cias. Verifique a consola (F12) para mais detalhes do erro.</div>`;
        }
    }

    carregarNoticias();
});