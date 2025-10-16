document.addEventListener('DOMContentLoaded', function () {

    async function carregarArtigos() {
        const container = document.getElementById('artigos-container');
        const spinner = document.getElementById('loading-spinner');
        const apiUrl = '/Artigos/GetArtigos';

        try {
            const response = await fetch(apiUrl);

            if (!response.ok) {
                throw new Error(`Erro no servidor: ${response.statusText} (Status: ${response.status})`);
            }

            const dadosCompletos = await response.json();

            // Remove o spinner assim que os dados chegam
            if (spinner) spinner.remove();

            // Verifica a estrutura padronizada
            if (!dadosCompletos || dadosCompletos.status !== "ok" || !Array.isArray(dadosCompletos.articles)) {
                console.error("Resposta da API inválida ou com erro:", dadosCompletos);
                const mensagem = dadosCompletos.message || "Erro na comunicação com a API de artigos.";
                container.innerHTML = `<div class="alert alert-warning text-center">${mensagem}</div>`;
                return;
            }

            const listaDeArtigos = dadosCompletos.articles;
            
            // Limpa o container (embora o spinner já tenha sido removido, garante que esteja vazio)
            container.innerHTML = '';

            if (listaDeArtigos.length === 0) {
                container.innerHTML = '<div class="alert alert-info text-center">Nenhum artigo encontrado no momento com os termos de busca padrão.</div>';
                return;
            }

            // Renderiza cada artigo
            listaDeArtigos.forEach(artigo => {
                container.innerHTML += criarCardArtigo(artigo);
            });

        } catch (error) {
            console.error("Falha ao carregar artigos:", error);
            // Garante que o spinner seja removido mesmo em caso de erro
            if (spinner) spinner.remove();
            container.innerHTML = `<div class="alert alert-danger text-center">Desculpe, ocorreu um erro inesperado. Verifique a consola (F12) para mais detalhes.</div>`;
        }
    }

    // Função auxiliar para formatar autores no estilo acadêmico
    function formatarAutores(autoresList) {
        if (!autoresList || autoresList.length === 0) {
            return "Autores não disponíveis";
        }
        const maxAutores = 3;
        if (autoresList.length > maxAutores) {
            // Exibe os primeiros e adiciona "et al."
            return `${autoresList.slice(0, maxAutores).join(', ')} et al.`;
        }
        return autoresList.join(', ');
    }

    // Função auxiliar para criar o HTML de cada artigo
    function criarCardArtigo(artigo) {
        const title = artigo.title || "Sem título";
        const url = artigo.url || "#";
        const authorsFormatted = formatarAutores(artigo.authors);
        const source = artigo.source || "Fonte desconhecida";
        const citedByCount = artigo.citedByCount || 0;

        // Formatação da data robusta
        let publicationDate = "Data desconhecida";
        if (artigo.publicationDate) {
            try {
                 // A data vem como YYYY-MM-DD. Adicionar 'T00:00:00' força o JS a interpretar como meia-noite local, 
                 // evitando erros de fuso horário (onde a data poderia voltar um dia se interpretada como UTC).
                 const data = new Date(artigo.publicationDate + 'T00:00:00');
                 if (!isNaN(data.getTime())) {
                    publicationDate = data.toLocaleDateString('pt-BR');
                 }
            } catch (e) {
                console.warn("Erro ao formatar data:", artigo.publicationDate, e);
            }
        }

        // Trunca o abstract para a visualização inicial
        let abstractText = artigo.abstractText || "Abstract (Resumo) não disponível.";
        if (abstractText.length > 500) {
            abstractText = abstractText.substring(0, 500) + '...';
        }

        // Card do Bootstrap adaptado para artigos (focado no texto)
        const cardHTML = `
            <div class="card shadow-sm">
                <div class="card-body">
                    <h5 class="card-title">
                        <a href="${url}" target="_blank" rel="noopener noreferrer" class="text-decoration-none">
                            ${title}
                        </a>
                    </h5>
                    <h6 class="card-subtitle mb-2 text-muted">
                        ${authorsFormatted}
                    </h6>
                    <p class="card-text mt-3" style="font-style: italic; font-size: 0.9rem;">
                        ${abstractText}
                    </p>
                </div>
                <div class="card-footer text-muted d-flex justify-content-between align-items-center">
                    <small>Publicado em: ${publicationDate} | Fonte: ${source}</small>
                    <span class="badge bg-primary" title="Número de citações">
                         Citações: ${citedByCount}
                    </span>
                </div>
            </div>
        `;
        return cardHTML;
    }

    // Inicia o carregamento
    carregarArtigos();
});