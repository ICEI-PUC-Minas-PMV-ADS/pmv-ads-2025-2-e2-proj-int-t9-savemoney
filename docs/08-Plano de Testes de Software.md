# Plano de Testes de Software — R15 Ferramentas Interativas

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Ferramentas Interativas. Os testes cobrem a API REST para o gerenciamento de produtos cadastrados pelo usuário, a segurança e a integridade dos dados, e também a lógica de cálculo client-side das calculadoras, assegurando que tanto o backend quanto o frontend funcionem conforme especificado.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R15-001** | API - Cadastrar novo produto | Usuário (perfil PJ, "Maria") está logado. | 1. Realizar uma requisição `POST` para `/api/produtos` com dados válidos (nome: "Café Expresso", preco_venda: 5.00, custo_variavel: 1.00). | A API retorna `201 Created` e o JSON do novo produto. O produto é salvo no banco associado ao `id_usuario` correto. |
| **CT-R15-002** | API - Listar produtos do usuário | Usuário "Maria" está logado e tem 3 produtos cadastrados. Usuário "João" está logado e não tem produtos. | 1. "Maria" realiza `GET` para `/api/produtos`. <br> 2. "João" realiza `GET` para `/api/produtos`. | 1. A API retorna `200 OK` e uma lista com os 3 produtos de Maria. <br> 2. A API retorna `200 OK` e uma lista vazia. |
| **CT-R15-003** | API - Editar um produto existente | Usuário "Maria" está logado. Existe um produto com `id=1` pertencente a ela. | 1. Realizar uma requisição `PUT` para `/api/produtos/1` alterando o `preco_venda` para `6.00`. | A API retorna `200 OK`. O campo `preco_venda` do produto `id=1` é atualizado no banco. |
| **CT-R15-004** | API - Excluir um produto | Usuário "Maria" está logado. Existe um produto com `id=1` pertencente a ela. | 1. Realizar uma requisição `DELETE` para `/api/produtos/1`. | A API retorna `204 No Content` ou `200 OK`. O produto `id=1` é removido da tabela `produtos`. |
| **CT-R15-005** | API - Segurança (Acesso indevido) | Usuário "João" está logado. O produto `id=1` pertence à "Maria". | 1. "João" tenta realizar um `PUT` ou `DELETE` no endpoint `/api/produtos/1`. | A API retorna erro `403 Forbidden` ou `404 Not Found`. Nenhuma alteração é feita no produto da Maria. |
| **CT-R15-006** | API - Validação de dados | Usuário "Maria" está logado. | 1. Realizar `POST` para `/api/produtos` com `preco_venda` com valor negativo. | A API retorna erro `400 Bad Request` com uma mensagem de validação clara. Nenhum produto é salvo. |
| **CT-R15-007** | Frontend - Lógica da Calculadora de Metas | Nenhuma. | 1. Executar a função de cálculo no frontend com os parâmetros: `objetivo=12000`, `prazo=24`. | A função JavaScript deve retornar o resultado `500`. |
| **CT-R15-008** | Frontend - Lógica da Calculadora de Ponto de Equilíbrio | Nenhuma. | 1. Executar a função de cálculo no frontend com: `custosFixos=3000`, `produto={preco_venda: 10, custo_variavel: 4}`. | A função JavaScript deve retornar o resultado `500` (unidades). |

---

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

# Plano de Testes de Software — R3 Conversor de Energia

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de conversão de energia opere conforme a especificação.

## 2. Casos de Teste

| ID     | Funcionalidade                          | Pré-condições                                                                | Ações                                                                                                                                 | Resultados Esperados                                                                                                                       |
| ------ | --------------------------------------- | ---------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| CT-006 | Busca de tarifa via TarifaService       | API da ANEEL e API da concessionária "CEMIG" estão registradas e ativas.     | 1. Tentar buscar uma tarifa para Minas Gerais.                                                                                        | O sistema deve obter a tarifa da CEMIG, priorizando-a sobre a tarifa da ANEEL, se aplicável.                                               |
| CT-007 | Falha em API externa                    | API da ANEEL está com falha de conexão.                                      | 1. Tentar buscar uma tarifa para um estado sem tarifa em outra API.<br>2. Sistema deve tentar a API da ANEEL.                         | O sistema deve informar que a busca falhou e sugerir a tarifa manual.                                                                      |
| CT-008 | Geração e exibição de gráfico           | Dados de consumo mensal do usuário preenchidos.                              | 1. Realizar uma conversão de kWh para R$.<br>2. Após a conversão, verificar a área de exibição de gráficos.                           | Um gráfico comparativo deve ser exibido, mostrando o custo atual vs. o custo estimado com energia solar, com rótulos e valores claros.     |
| CT-009 | Validação de entrada (números)          | Nenhuma pré-condição.                                                        | 1. Inserir o valor "abc" no campo valorBase.<br>2. Clicar em "Converter".                                                             | O sistema deve exibir uma mensagem de erro, como "Por favor, insira um valor numérico válido".                                             |
| CT-010 | Conversão para Veículo Elétrico e Dicas | Configurações para veículos elétricos (como consumo por km) pré-definidas.   | 1. Acessar o conversor.<br>2. Inserir valor em kWh.<br>3. Selecionar tipoDispositivo "Veiculo Eletrico".<br>4. Clicar em "Converter". | O sistema deve exibir o valor convertido e dicas específicas para veículos elétricos (ex.: "Custo por km: X R$").                          |
| CT-011 | Conversão para Ar Condicionado e Dicas  | Informações sobre o consumo de Ar Condicionado pré-definidas.                | 1. Acessar o conversor.<br>2. Inserir valor em kWh.<br>3. Selecionar tipoDispositivo "Ar Condicionado".<br>4. Clicar em "Converter".  | O sistema deve exibir o valor convertido e dicas específicas para ar condicionado (ex.: "Mantenha a temperatura em 24ºC para economizar"). |
| CT-012 | Conversão com tempoUso (horas para dia) | Tarifa para o estado "SP" está disponível.                                   | 1. Acessar o conversor.<br>2. Inserir 10 kWh.<br>3. Selecionar tempoUso "por hora".<br>4. Clicar em "Converter".                      | O sistema deve converter 10 kWh/h para o custo diário, exibindo o resultado para 24 horas.                                                 |
| CT-013 | Conversão com tempoUso (km para custo)  | Consumo de referência de veículo elétrico (ex: 0.15 kWh/km) está disponível. | 1. Acessar o conversor.<br>2. Inserir 100 km.<br>3. Selecionar tipoDispositivo "Veículo Elétrico".<br>4. Clicar em "Converter".       | O sistema deve converter 100 km para o valor em kWh e depois para o custo em R$.                                                           |

---

# Plano de Testes de Software — R8 Personalização do Tema Dark e Light

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa de personalização de tema, incluindo a comunicação com o backend e a persistência dos dados.

## 2. Casos de Teste

| ID     | Funcionalidade                    | Pré-condições                      | Ações                                                                                                | Resultados Esperados                                                                                                  |
| ------ | --------------------------------- | ---------------------------------- | ---------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| CT-001 | Salvar tema personalizado via API | Usuário logado                     | 1. Selecionar tema "Customizado".<br>2. Mudar cor primária para `#ff5722`.<br>3. Clicar em "Salvar". | Requisição `POST /api/preferences` enviada com dados corretos. API retorna `201 Created`. Tema aplicado na interface. |
| CT-002 | Atualizar tema salvo              | Usuário tem tema customizado salvo | 1. Acessar personalização.<br>2. Alterar cor primária para `#2196f3`.<br>3. Clicar em "Salvar".      | Requisição `PUT /api/preferences` enviada. API retorna `200 OK`. Tema atualizado na interface.                        |
| CT-003 | Persistência após recarga         | Tema customizado salvo no banco    | 1. Fazer logout e login novamente.<br>2. Alternativamente, recarregar a página.                      | Frontend envia `GET /api/preferences` e aplica automaticamente o tema salvo.                                          |
| CT-004 | Restauração de tema padrão        | Usuário com tema customizado ativo | 1. Acessar personalização.<br>2. Clicar em "Restaurar Padrão".<br>3. Clicar em "Salvar".             | Requisição `DELETE /api/preferences`. Tema volta ao padrão "Claro". Preferência removida do banco.                    |
| CT-005 | Validação de dados da API         | Nenhuma                            | 1. Enviar `POST /api/preferences` com dados inválidos (ex: `corPrimaria: "abc"`).                    | API retorna erro `400 Bad Request`. Nenhuma preferência é salva.                                                      |

---

