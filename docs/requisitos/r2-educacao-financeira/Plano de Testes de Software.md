# Plano de Testes de Software — R2 Educação Financeira Save Money com JavaScript

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Educação Financeira. Os testes cobrem a API REST para gerenciamento de conteúdo e curtidas, a integridade dos dados no banco e o comportamento do serviço de agregação de conteúdo externo, assegurando que toda a lógica de backend funcione conforme especificado.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R2-001** | API - Listar conteúdos agregados teste testestes | O serviço de agregação já populou a tabela `conteudos_agregados` com pelo menos 5 itens. | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve retornar status `200 OK` e um JSON contendo a lista de conteúdos. |
| **CT-R2-002** | API - Curtir um conteúdo | Usuário está logado. <br> Existe um conteúdo com `id=1`. <br> O usuário ainda não curtiu o conteúdo `id=1`. | 1. Realizar uma requisição `POST` para `/api/educacao/1/curtir`. | A API retorna `201 Created` ou `200 OK`. <br> Um registro associando o usuário ao conteúdo `id=1` é criado na tabela `conteudos_curtidos`. |
| **CT-R2-003** | API - Listar conteúdos curtidos | Usuário está logado e já curtiu 2 conteúdos específicos. | 1. Realizar uma requisição `GET` para `/api/educacao/curtidos`. | A API retorna `200 OK` e um JSON contendo a lista com os dados completos dos 2 conteúdos curtidos. |
| **CT-R2-004** | API - Descurtir um conteúdo | Usuário está logado e já curtiu o conteúdo com `id=1`. | 1. Realizar uma requisição `DELETE` para `/api/educacao/1/descurtir`. | A API retorna `200 OK` ou `204 No Content`. <br> O registro correspondente é removido da tabela `conteudos_curtidos`. |
| **CT-R2-005** | API - Validação (Curtir conteúdo inexistente) | Usuário está logado. | 1. Realizar uma requisição `POST` para `/api/educacao/999/curtir`, onde o `id=999` não existe. | A API retorna erro `404 Not Found`. Nenhum registro é criado no banco de dados. |
| **CT-R2-006** | API - Segurança de endpoint | Nenhuma. | 1. Realizar uma requisição `POST` para `/api/educacao/1/curtir` sem um token de autenticação válido. | A API retorna erro `401 Unauthorized`. |
| **CT-R2-007** | Backend - Prevenção de duplicatas no agregador | A tabela `conteudos_agregados` já contém um item com a `url_original` "https://portal.com/noticia-A". | 1. Executar o serviço de agregação de conteúdo, que tenta buscar a mesma "noticia-A". | O serviço finaliza a execução sem erros. A "noticia-A" **não** é inserida novamente. A tabela continua com uma única entrada para essa URL. |
| **CT-R2-008** | Backend - Lógica da Newsletter | A tabela `conteudos_agregados` contém pelo menos 10 itens. | 1. Executar o serviço de envio de newsletter. | O serviço seleciona os 3 conteúdos mais recentes e dispara o processo de envio de e-mail, sem falhar. |
