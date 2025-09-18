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

# Plano de Testes de Software — R4 Relatórios, Diagnósticos e Resultados

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de geração de relatórios, diagnósticos e resultados opere conforme a especificação.

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                              | Ações                                                                                                   | Resultados Esperados                                                                                      |
| ------ | ------------------------------------- | --------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------- |
| CT-101 | Geração de relatório detalhado        | Usuário autenticado e dados financeiros cadastrados        | 1. Solicitar relatório detalhado para um período específico.                                            | O sistema deve gerar e exibir o relatório com dados corretos e completos.                                 |
| CT-102 | Filtros de relatório                  | Usuário autenticado e múltiplos registros financeiros      | 1. Selecionar filtros (ex: categoria, período).<br>2. Gerar relatório.                                 | O relatório deve considerar apenas os dados filtrados conforme seleção do usuário.                       |
| CT-103 | Diagnóstico automático                | Usuário autenticado com histórico financeiro suficiente    | 1. Solicitar diagnóstico financeiro.<br>2. Confirmar geração.                                          | O sistema deve exibir diagnóstico com análise e recomendações baseadas nos dados do usuário.              |
| CT-104 | Exportação de relatório               | Relatório gerado e exibido na tela                         | 1. Clicar em "Exportar".<br>2. Selecionar formato (PDF, CSV).                                         | O sistema deve exportar o relatório no formato escolhido, mantendo a integridade dos dados.               |
| CT-105 | Mensagem de erro para dados inválidos | Usuário não preenche filtros obrigatórios                  | 1. Tentar gerar relatório sem preencher campos obrigatórios.                                            | O sistema deve exibir mensagem de erro clara, informando os campos obrigatórios que faltam ser preenchidos.|
| CT-106 | Atualização de relatório              | Relatório já existente                                     | 1. Editar filtros ou parâmetros.<br>2. Gerar novamente.                                                | O sistema deve atualizar o relatório conforme os novos parâmetros e exibir o resultado atualizado.         |

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

# Plano de Testes de Software — R9 Metas Financeiras

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de criação e gerenciamento de metas financeiras opere conforme a especificação.

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                              | Ações                                                                                                 | Resultados Esperados                                                                                 |
| ------ | ------------------------------------- | --------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| CT-001 | Criação de meta financeira            | Usuário autenticado no sistema.                            | 1. Acessar a tela de metas.<br>2. Clicar em "Nova Meta".<br>3. Preencher dados válidos.<br>4. Salvar. | Meta criada com sucesso e exibida na lista.                                                         |
| CT-002 | Validação de campos obrigatórios      | Usuário autenticado.                                      | 1. Tentar criar meta sem preencher título ou valor objetivo.<br>2. Salvar.                            | Sistema exibe mensagem de erro e impede o cadastro.                                                  |
| CT-003 | Registro de aporte em meta            | Meta financeira existente.                                 | 1. Selecionar meta.<br>2. Informar valor de aporte válido.<br>3. Salvar.                              | Aporte registrado, valor atual da meta atualizado.                                                   |
| CT-004 | Validação de valor de aporte          | Meta financeira existente.                                 | 1. Selecionar meta.<br>2. Informar valor de aporte negativo ou zero.<br>3. Salvar.                    | Sistema exibe mensagem de erro e não registra o aporte.                                              |
| CT-005 | Edição de meta financeira             | Meta financeira existente.                                 | 1. Selecionar meta.<br>2. Editar dados (ex: valor objetivo, data limite).<br>3. Salvar.               | Meta atualizada com sucesso.                                                                         |
| CT-006 | Remoção de meta financeira            | Meta financeira existente.                                 | 1. Selecionar meta.<br>2. Clicar em "Remover".<br>3. Confirmar remoção.                              | Meta removida e aportes associados excluídos.                                                        |
| CT-007 | Listagem de metas                     | Usuário possui metas cadastradas.                          | 1. Acessar tela de metas.                                                                             | Lista exibe todas as metas do usuário, com progresso e status.                                       |
| CT-008 | Listagem de aportes                   | Meta financeira possui aportes registrados.                | 1. Selecionar meta.<br>2. Visualizar lista de aportes.                                               | Lista exibe todos os aportes realizados para a meta.                                                 |
| CT-009 | Conclusão automática de meta          | Meta financeira com valor atual >= valor objetivo.         | 1. Registrar aporte que atinja ou ultrapasse o valor objetivo.                                       | Sistema marca meta como concluída e exibe mensagem de parabéns.                                      |
| CT-010 | Persistência dos dados                | Usuário possui metas e aportes cadastrados.                | 1. Sair e entrar novamente no sistema.<br>2. Acessar tela de metas.                                  | Dados de metas e aportes permanecem salvos e corretos.                                               |
| CT-011 | Restrições de acesso                  | Usuário não autenticado.                                   | 1. Tentar acessar tela de metas.                                                                     | Sistema redireciona para login ou exibe mensagem de acesso negado.                                   |

---

# Plano de Testes de Software — R10 Dashboard Personalizado

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de dashboard personalizado opere conforme a especificação.

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                      | Ações                                                                                      | Resultados Esperados                                                                                 |
| ------ | ------------------------------------- | -------------------------------------------------- | ------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------- |
| CT-001 | Adicionar widget ao dashboard         | Usuário autenticado e dashboard existente           | 1. Acessar dashboard.<br>2. Clicar em "Adicionar Widget".<br>3. Selecionar tipo e salvar.  | Widget é exibido no painel na posição definida.                                                      |
| CT-002 | Remover widget do dashboard           | Dashboard com pelo menos um widget                  | 1. Clicar em "Remover" no widget desejado.                                                | Widget é removido do painel e configuração é atualizada.                                             |
| CT-003 | Reordenar widgets (drag and drop)     | Dashboard com dois ou mais widgets                  | 1. Arrastar um widget para nova posição.<br>2. Soltar.                                     | Ordem dos widgets é atualizada e persistida.                                                         |
| CT-004 | Editar configuração de um widget      | Dashboard com widget configurável                   | 1. Clicar em "Editar" no widget.<br>2. Alterar configuração.<br>3. Salvar.                | Widget reflete as novas configurações imediatamente.                                                 |
| CT-005 | Salvar e restaurar layout personalizado| Layout do dashboard alterado pelo usuário           | 1. Alterar disposição dos widgets.<br>2. Salvar layout.<br>3. Recarregar dashboard.        | Layout salvo é restaurado corretamente, mantendo ordem e configurações dos widgets.                  |
| CT-006 | Persistência após logout/login        | Layout personalizado salvo                          | 1. Fazer logout.<br>2. Fazer login novamente.<br>3. Acessar dashboard.                     | Layout e widgets permanecem conforme última configuração salva.                                      |

# Plano de Testes de Software — R11 Avisos e Notificações

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de avisos e notificações opere conforme a especificação.

## 2. Casos de Teste

| ID     | Funcionalidade                          | Pré-condições                                 | Ações                                                                                 | Resultados Esperados                                                                 |
| ------ | --------------------------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
| CT-001 | Exibir notificação ao usuário           | Usuário autenticado e evento gerador ocorrido | 1. Realizar ação que gera notificação.<br>2. Verificar painel de notificações.        | Notificação é exibida ao usuário com mensagem e indicador visual correspondente.      |
| CT-002 | Marcar notificação como lida            | Notificação não lida disponível               | 1. Clicar em "Marcar como lida" na notificação.                                     | Notificação é marcada como lida e indicador visual é atualizado.                     |
| CT-003 | Exibir histórico de notificações        | Notificações já recebidas                     | 1. Acessar histórico de notificações.                                                | Histórico exibe todas as notificações anteriores, lidas e não lidas.                 |
| CT-004 | Atualizar indicador visual              | Indicador visual disponível                   | 1. Realizar ação que altera status (ex: progresso).                                  | Indicador visual é atualizado conforme novo status.                                  |
| CT-005 | Persistência após logout/login          | Notificações e indicadores salvos             | 1. Fazer logout.<br>2. Fazer login novamente.<br>3. Acessar painel de notificações.   | Notificações e indicadores permanecem conforme última configuração salva.            |

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

# Plano de Testes de Software — R16 Histórico Financeiro

## 1. Introdução

Este plano de testes de software visa garantir que a funcionalidade de histórico financeiro opere conforme a especificação, permitindo ao usuário visualizar corretamente saldos e movimentações.

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                 | Ações                                                                                 | Resultados Esperados                                                                                 |
| ------ | ------------------------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| CT-001 | Visualização de histórico             | Usuário autenticado com movimentações salvas  | 1. Acessar histórico financeiro.                                                     | Exibir lista de movimentações e saldo atual.                                                         |
| CT-002 | Filtro por período                    | Usuário autenticado com movimentações salvas  | 1. Selecionar período (ex: mês atual).                                               | Exibir apenas movimentações do período selecionado.                                                   |
| CT-003 | Filtro por categoria                  | Usuário autenticado com movimentações salvas  | 1. Selecionar categoria (ex: Alimentação).                                           | Exibir apenas movimentações da categoria selecionada.                                                 |
| CT-004 | Gráfico de evolução do saldo          | Usuário autenticado com movimentações salvas  | 1. Visualizar gráfico de saldo.                                                      | Exibir gráfico mostrando evolução do saldo ao longo do tempo.                                        |
| CT-005 | Exportação de relatório               | Usuário autenticado com movimentações salvas  | 1. Clicar em exportar relatório.                                                     | Gerar arquivo PDF/Excel com dados do histórico financeiro.                                            |
| CT-006 | Mensagem para histórico vazio         | Usuário autenticado sem movimentações salvas  | 1. Acessar histórico financeiro.                                                     | Exibir mensagem: "Nenhuma movimentação encontrada para o período selecionado."                      |
| CT-007 | Atualização de saldo após movimentação| Usuário autenticado                          | 1. Adicionar nova movimentação (receita ou despesa).                                 | Saldo atualizado corretamente após a inclusão da movimentação.                                       |

---

