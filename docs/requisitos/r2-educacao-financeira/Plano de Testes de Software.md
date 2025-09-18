# Plano de Testes de Software — R2 Educação Financeira Save Money com JavaScript

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Educação Financeira. Conforme a arquitetura *stateless* (sem banco de dados), os testes cobrem a capacidade da API de atuar como um *proxy*, buscando conteúdo de fontes externas em tempo real e gerenciando a lógica da newsletter, assegurando que o backend funcione conforme especificado.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R2-001** | API - Listar conteúdos em tempo real | O serviço externo (API de Notícias) está online e funcionando. | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve retornar status `200 OK` e um JSON contendo uma lista de conteúdos buscada da fonte externa. |
| **CT-R2-002** | API - Tratamento de falha do serviço externo | O serviço externo (API de Notícias) está offline ou retorna um erro. | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve tratar o erro de forma graciosa e retornar um status apropriado (ex: `502 Bad Gateway` ou `503 Service Unavailable`) com uma mensagem de erro clara. |
| **CT-R2-003** | Backend - Lógica da Newsletter | O serviço externo (API de Notícias) está online. | 1. Executar o serviço de envio de newsletter. | O serviço deve buscar com sucesso os conteúdos da fonte externa, selecionar os 3 mais recentes e disparar o processo de envio de e-mail, sem falhar. |
| **CT-R2-004** | API - Validação do contrato de dados | O serviço externo retorna dados em um formato inesperado (ex: um campo obrigatório como `titulo` vem nulo). | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve ser resiliente, podendo filtrar o item malformado ou retornar um erro `500 Internal Server Error` com um log detalhado, em vez de quebrar a aplicação. |
