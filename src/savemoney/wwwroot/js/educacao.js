// Garante que o script só roda depois de a página carregar
document.addEventListener('DOMContentLoaded', function() {

    // Função assíncrona para buscar as notícias
    async function carregarNoticias() {
        try {
            // 1. Fazer o pedido para a nossa API interna
            const response = await fetch('/EducacaoFinanceira/GetNoticias');

            // 2. Verificar se o pedido foi bem-sucedido
            if (!response.ok) {
                throw new Error(`Erro na API: ${response.statusText}`);
            }

            // 3. Converter a resposta em JSON
            const noticiasJson = await response.json();

            // 4. Chamar a função que vai desenhar os cartões na tela
            renderizarNoticias(noticiasJson.articles); // A NewsAPI retorna os artigos dentro de uma propriedade "articles"

        } catch (error) {
            console.error("Falha ao carregar notícias:", error);
            // Exibir uma mensagem de erro para o usuário aqui
        }
    }

    // Função para renderizar os cartões (vamos implementar no próximo passo)
    function renderizarNoticias(listaDeNoticias) {
        console.log(listaDeNoticias); // Por enquanto, apenas mostre os dados na consola para confirmar
    }

    // Chamar a função principal para iniciar o processo
    carregarNoticias();
});