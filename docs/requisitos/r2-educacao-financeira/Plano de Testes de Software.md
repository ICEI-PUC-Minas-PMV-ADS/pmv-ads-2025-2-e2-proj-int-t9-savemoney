# Plano de Testes de Software — Save Money (R2 Educação Financeira)

## 1. Introdução

O plano de testes foca em garantir a funcionalidade do **front-end** do módulo de Educação Financeira. Os testes cobrem a capacidade da interface de usuário (UI) de carregar corretamente o conteúdo (Artigos e Notícias) e de tratar de forma robusta os fluxos de exceção, como rotas inexistentes.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R2-001** | Front-End - Sucesso Artigos | A aplicação está rodando no ambiente de desenvolvimento (Codespace). | 1. Acessar o endpoint `/Artigos` pelo navegador. | A API deve carregar a página de "Artigos Acadêmicos" com sucesso, exibindo a interface de busca e os cards de conteúdo. |
| **CT-R2-002** | Front-End - Sucesso Notícias | A aplicação está rodando no ambiente de desenvolvimento (Codespace). | 1. Acessar o endpoint `/Noticias` pelo navegador. | A API deve carregar a página de "Notícias do Mercado Financeiro" com sucesso, exibindo a interface de busca e os cards de conteúdo. |
| **CT-R2-003** | Front-End - Erro (Rota Inexistente) | A aplicação está rodando no ambiente de desenvolvimento (Codespace). | 1. Acessar um endpoint inválido (ex: `/PaginaQueNaoExiste123`). | A aplicação deve tratar o erro graciosamente, retornando uma página `HTTP ERROR 404` (Não Encontrado), sem "quebrar". |