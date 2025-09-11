# Plano de Testes de Software — R2 Educação Financeira

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Educação Financeira. Os testes cobrem a API REST para gerenciamento de artigos e curtidas, a integridade dos dados no banco e o comportamento do serviço de agregação de conteúdo externo, assegurando que toda a lógica de backend funcione conforme especificado.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R2-001** | API - Listar artigos agregados | O serviço de agregação já populou a tabela `artigos_agregados` com pelo menos 5 artigos. | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve retornar status `200 OK` e um JSON contendo a lista de artigos. |
| **CT-R2-002** | API - Curtir um artigo | Usuário está logado. <br> Existe um artigo com `id=1`. <br> O usuário ainda não curtiu o artigo `id=1`. | 1. Realizar uma requisição `POST` para `/api/educacao/1/curtir`. | A API retorna `201 Created` ou `200 OK`. <br> Um registro associando o usuário ao artigo `id=1` é criado na tabela `artigos_curtidos`. |
| **CT-R2-003** | API - Listar artigos curtidos | Usuário está logado e já curtiu 2 artigos específicos. | 1. Realizar uma requisição `GET` para `/api/educacao/curtidos`. | A API retorna `200 OK` e um JSON contendo a lista com os dados completos dos 2 artigos curtidos. |
| **CT-R2-004** | API - Descurtir um artigo | Usuário está logado e já curtiu o artigo com `id=1`. | 1. Realizar uma requisição `DELETE` para `/api/educacao/1/descurtir`. | A API retorna `200 OK` ou `204 No Content`. <br> O registro correspondente é removido da tabela `artigos_curtidos`. |
| **CT-R2-005** | API - Validação de dados (Curtir artigo inexistente) | Usuário está logado. | 1. Realizar uma requisição `POST` para `/api/educacao/999/curtir`, onde o `id=999` não existe. | A API retorna erro `404 Not Found`. Nenhum registro é criado no banco de dados. |
| **CT-R2-006** | API - Segurança de endpoint | Nenhuma. | 1. Realizar uma requisição `POST` para `/api/educacao/1/curtir` sem um token de autenticação válido. | A API retorna erro `401 Unauthorized`. |
| **CT-R2-007** | Backend - Prevenção de duplicatas no agregador | A tabela `artigos_agregados` já contém um artigo com a `url_original` "https://portal.com/noticia-A". | 1. Executar o serviço de agregação de conteúdo, que tenta buscar a mesma "noticia-A". | O serviço finaliza a execução sem erros. A "noticia-A" **não** é inserida novamente. A tabela continua com uma única entrada para essa URL. |
| **CT-R2-008** | Backend - Lógica da Newsletter | A tabela `artigos_agregados` contém pelo menos 10 artigos. | 1. Executar o serviço de envio de newsletter. | O serviço seleciona os 3 artigos mais recentes e dispara o processo de envio de e-mail, sem falhar. |

---