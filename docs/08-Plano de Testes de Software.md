# Plano de Testes de Software — R1 Controle Financeiro

## 1. Introdução

Este plano de testes de software tem como objetivo garantir que a funcionalidade de controle financeiro opere conforme a especificação, permitindo o registro, edição, remoção e listagem de receitas e despesas.

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                   | Ações                                                                                 | Resultados Esperados                                                                 |
| ------ | ------------------------------------- | ----------------------------------------------- | ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
| CT-001 | Cadastro de receita                   | Usuário autenticado                             | 1. Acessar tela de cadastro.<br>2. Preencher valor, categoria, tipo "Receita", data e descrição.<br>3. Salvar. | Receita cadastrada e exibida na listagem. Saldo atualizado.                          |
| CT-002 | Cadastro de despesa                   | Usuário autenticado                             | 1. Acessar tela de cadastro.<br>2. Preencher valor, categoria, tipo "Despesa", data e descrição.<br>3. Salvar. | Despesa cadastrada e exibida na listagem. Saldo atualizado.                          |
| CT-003 | Validação de campos obrigatórios      | Usuário autenticado                             | 1. Tentar cadastrar sem preencher valor ou tipo.<br>2. Salvar.                        | Mensagem de erro informando campos obrigatórios.                                     |
| CT-004 | Edição de registro financeiro         | Registro existente                              | 1. Selecionar registro.<br>2. Editar valor, categoria, tipo, data ou descrição.<br>3. Salvar. | Registro atualizado corretamente na listagem.                                        |
| CT-005 | Remoção de registro financeiro        | Registro existente                              | 1. Selecionar registro.<br>2. Remover.                                                | Registro removido da listagem. Saldo atualizado.                                     |
| CT-006 | Listagem de registros                 | Usuário possui registros                        | 1. Acessar tela de listagem.                                                          | Exibição de todos os registros do usuário, com filtros por período, categoria e tipo. |
| CT-007 | Cálculo de saldo                      | Usuário possui receitas e despesas cadastradas   | 1. Visualizar tela de saldo.                                                          | Saldo exibido corretamente (receitas - despesas).                                    |

# Plano de Testes de Software — R2 Educação Financeira

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Educação Financeira. Conforme a arquitetura *stateless* (sem banco de dados), os testes cobrem a capacidade da API de atuar como um *proxy*, buscando conteúdo de fontes externas em tempo real e gerenciando a lógica da newsletter, assegurando que o backend funcione conforme especificado.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R2-001** | API - Listar conteúdos em tempo real | O serviço externo (API de Notícias) está online e funcionando. | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve retornar status `200 OK` e um JSON contendo uma lista de conteúdos buscada da fonte externa. |
| **CT-R2-002** | API - Tratamento de falha do serviço externo | O serviço externo (API de Notícias) está offline ou retorna um erro. | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve tratar o erro de forma graciosa e retornar um status apropriado (ex: `502 Bad Gateway` ou `503 Service Unavailable`) com uma mensagem de erro clara. |
| **CT-R2-003** | Backend - Lógica da Newsletter | O serviço externo (API de Notícias) está online. | 1. Executar o serviço de envio de newsletter. | O serviço deve buscar com sucesso os conteúdos da fonte externa, selecionar os 3 mais recentes e disparar o processo de envio de e-mail, sem falhar. |
| **CT-R2-004** | API - Validação do contrato de dados | O serviço externo retorna dados em um formato inesperado (ex: um campo obrigatório como `titulo` vem nulo). | 1. Realizar uma requisição `GET` para o endpoint `/api/educacao`. | A API deve ser resiliente, podendo filtrar o item malformado ou retornar um erro `500 Internal Server Error` com um log detalhado, em vez de quebrar a aplicação. |

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

# Plano de Testes de Software — R5 Exportação e Compartilhamento

## 1. Introdução
Este plano de testes de software tem como objetivo validar a funcionalidade de **Exportação e Compartilhamento (R5)** no sistema SaveMoney V2.  
Essa funcionalidade deve permitir que usuários exportem relatórios financeiros nos formatos PDF e Excel e compartilhem esses arquivos utilizando as opções nativas do dispositivo (ex: WhatsApp, E-mail, Google Drive).

---

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                                                 | Ações                                                                                  | Resultados Esperados                                                                                   |
| ------ | ------------------------------------- | ------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| CT-201 | Exportação em PDF                     | Relatório gerado e exibido na tela                                            | 1. Clicar em **Exportar**.<br>2. Selecionar formato PDF.<br>3. Confirmar exportação.   | O sistema deve gerar e baixar o arquivo PDF, mantendo integridade, layout e dados do relatório.         |
| CT-202 | Exportação em Excel                   | Relatório gerado e exibido na tela                                            | 1. Clicar em **Exportar**.<br>2. Selecionar formato Excel.<br>3. Confirmar exportação. | O sistema deve gerar e baixar o arquivo Excel (.xlsx), mantendo integridade, layout e dados do relatório. |
| CT-203 | Compartilhamento de PDF               | Arquivo PDF exportado disponível no dispositivo                               | 1. Clicar em **Compartilhar**.<br>2. Selecionar arquivo PDF.<br>3. Escolher app (ex: WhatsApp). | O sistema deve abrir o menu de compartilhamento nativo e permitir envio do PDF pelo app escolhido.      |
| CT-204 | Compartilhamento de Excel             | Arquivo Excel exportado disponível no dispositivo                             | 1. Clicar em **Compartilhar**.<br>2. Selecionar arquivo Excel.<br>3. Escolher app.     | O sistema deve abrir o menu de compartilhamento nativo e permitir envio do Excel pelo app escolhido.    |
| CT-205 | Mensagem de erro na exportação        | Falha na geração do arquivo (ex: erro de conexão, dados inconsistentes)        | 1. Tentar exportar relatório.<br>2. Simular falha no backend.                          | O sistema deve exibir mensagem de erro clara, informando o motivo da falha e sugerindo ação corretiva.  |
| CT-206 | Restrições de permissão no dispositivo| Dispositivo com restrições de armazenamento ou compartilhamento                | 1. Tentar exportar ou compartilhar relatório.<br>2. Negar permissão de acesso.         | O sistema deve informar ao usuário sobre a restrição e orientar como conceder permissão, se aplicável.  |
| CT-207 | Exportação com filtros aplicados      | Relatório gerado com filtros específicos                                      | 1. Aplicar filtros.<br>2. Exportar relatório em PDF e Excel.                           | Os arquivos exportados devem refletir exatamente os dados filtrados, sem divergências.                  |

# Plano de Testes de Software — R8 Personalização do Tema

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

| ID     | Funcionalidade                   | Pré-condições                                      | Ações                                                                                                 | Resultados Esperados                                               |
| ------ | -------------------------------- | -------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------ |
| CT-001 | Criação de meta financeira       | Usuário autenticado no sistema.                    | 1. Acessar a tela de metas.<br>2. Clicar em "Nova Meta".<br>3. Preencher dados válidos.<br>4. Salvar. | Meta criada com sucesso e exibida na lista.                        |
| CT-002 | Validação de campos obrigatórios | Usuário autenticado.                               | 1. Tentar criar meta sem preencher título ou valor objetivo.<br>2. Salvar.                            | Sistema exibe mensagem de erro e impede o cadastro.                |
| CT-003 | Registro de aporte em meta       | Meta financeira existente.                         | 1. Selecionar meta.<br>2. Informar valor de aporte válido.<br>3. Salvar.                              | Aporte registrado, valor atual da meta atualizado.                 |
| CT-004 | Validação de valor de aporte     | Meta financeira existente.                         | 1. Selecionar meta.<br>2. Informar valor de aporte negativo ou zero.<br>3. Salvar.                    | Sistema exibe mensagem de erro e não registra o aporte.            |
| CT-005 | Edição de meta financeira        | Meta financeira existente.                         | 1. Selecionar meta.<br>2. Editar dados (ex: valor objetivo, data limite).<br>3. Salvar.               | Meta atualizada com sucesso.                                       |
| CT-006 | Remoção de meta financeira       | Meta financeira existente.                         | 1. Selecionar meta.<br>2. Clicar em "Remover".<br>3. Confirmar remoção.                               | Meta removida e aportes associados excluídos.                      |
| CT-007 | Listagem de metas                | Usuário possui metas cadastradas.                  | 1. Acessar tela de metas.                                                                             | Lista exibe todas as metas do usuário, com progresso e status.     |
| CT-008 | Listagem de aportes              | Meta financeira possui aportes registrados.        | 1. Selecionar meta.<br>2. Visualizar lista de aportes.                                                | Lista exibe todos os aportes realizados para a meta.               |
| CT-009 | Conclusão automática de meta     | Meta financeira com valor atual >= valor objetivo. | 1. Registrar aporte que atinja ou ultrapasse o valor objetivo.                                        | Sistema marca meta como concluída e exibe mensagem de parabéns.    |
| CT-010 | Persistência dos dados           | Usuário possui metas e aportes cadastrados.        | 1. Sair e entrar novamente no sistema.<br>2. Acessar tela de metas.                                   | Dados de metas e aportes permanecem salvos e corretos.             |
| CT-011 | Restrições de acesso             | Usuário não autenticado.                           | 1. Tentar acessar tela de metas.                                                                      | Sistema redireciona para login ou exibe mensagem de acesso negado. |

---

# Plano de Testes de Software — R10 Dashboard Personalizado

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de dashboard personalizado opere conforme a especificação.

## 2. Casos de Teste

| ID     | Funcionalidade                          | Pré-condições                             | Ações                                                                                     | Resultados Esperados                                                                |
| ------ | --------------------------------------- | ----------------------------------------- | ----------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| CT-001 | Adicionar widget ao dashboard           | Usuário autenticado e dashboard existente | 1. Acessar dashboard.<br>2. Clicar em "Adicionar Widget".<br>3. Selecionar tipo e salvar. | Widget é exibido no painel na posição definida.                                     |
| CT-002 | Remover widget do dashboard             | Dashboard com pelo menos um widget        | 1. Clicar em "Remover" no widget desejado.                                                | Widget é removido do painel e configuração é atualizada.                            |
| CT-003 | Reordenar widgets (drag and drop)       | Dashboard com dois ou mais widgets        | 1. Arrastar um widget para nova posição.<br>2. Soltar.                                    | Ordem dos widgets é atualizada e persistida.                                        |
| CT-004 | Editar configuração de um widget        | Dashboard com widget configurável         | 1. Clicar em "Editar" no widget.<br>2. Alterar configuração.<br>3. Salvar.                | Widget reflete as novas configurações imediatamente.                                |
| CT-005 | Salvar e restaurar layout personalizado | Layout do dashboard alterado pelo usuário | 1. Alterar disposição dos widgets.<br>2. Salvar layout.<br>3. Recarregar dashboard.       | Layout salvo é restaurado corretamente, mantendo ordem e configurações dos widgets. |
| CT-006 | Persistência após logout/login          | Layout personalizado salvo                | 1. Fazer logout.<br>2. Fazer login novamente.<br>3. Acessar dashboard.                    | Layout e widgets permanecem conforme última configuração salva.                     |

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

# Plano de Testes de Software — R12 Gestão de Orçamento

## 1. Introdução

Este plano de testes de software foca em garantir que a funcionalidade de Gestão de Orçamento opere conforme a especificação, validando a criação, atualização, remoção e consulta de orçamentos mensais por categoria.

## 2. Casos de Teste

| ID         | Funcionalidade                   | Pré-condições                                                                              | Ações                                                                                                          | Resultados Esperados                                                                                                                      |
| ---------- | -------------------------------- | ------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| CT-R12-001 | Criar um novo orçamento          | Usuário autenticado. Categoria "Lazer" existe.                                             | 1. Enviar via API os dados para um novo orçamento: categoriaId="Lazer", valor_limite=500.00, mes=10, ano=2025. | A API deve retornar 201 Created. Um novo registro de orçamento deve ser criado no banco de dados com os valores corretos.                 |
| CT-R12-002 | Listar orçamentos do mês         | Usuário autenticado possui 2 orçamentos criados para o mês 10/2025.                        | 1. Solicitar via API a lista de orçamentos para o período de 10/2025.                                          | A API deve retornar 200 OK e uma lista contendo exatamente os 2 orçamentos criados para aquele mês.                                       |
| CT-R12-003 | Atualizar um orçamento existente | Usuário autenticado. Existe um orçamento para "Lazer" com valor_limite=500.00.             | 1. Enviar uma requisição PUT para atualizar o orçamento, alterando o valor_limite para 600.00.                 | A API deve retornar 200 OK. O registro do orçamento no banco de dados deve ser atualizado com o novo valor.                               |
| CT-R12-004 | Excluir um orçamento             | Usuário autenticado. Existe um orçamento para a categoria "Lazer".                         | 1. Enviar uma requisição DELETE para remover o orçamento da categoria "Lazer".                                 | A API deve retornar 204 No Content ou 200 OK. O registro do orçamento deve ser removido do banco de dados.                                |
| CT-R12-005 | Prevenir orçamento duplicado     | Já existe um orçamento para "Lazer" no mês 10/2025.                                        | 1. Tentar criar um segundo orçamento para "Lazer" no mesmo mês e ano (10/2025).                                | A API deve retornar um erro 409 Conflict, informando que o orçamento já existe. Nenhum novo registro deve ser criado.                     |
| CT-R12-006 | Validação de dados de entrada    | Usuário autenticado.                                                                       | 1. Tentar criar um orçamento com valor_limite negativo (-100).                                                 | A API deve retornar um erro 400 Bad Request com uma mensagem de validação clara. Nenhum orçamento deve ser salvo.                         |
| CT-R12-007 | Verificar status de um orçamento | Usuário tem um orçamento de R$ 500 para "Lazer" e já gastou R$ 350 nessa categoria no mês. | 1. Solicitar via API o status do orçamento de "Lazer".                                                         | A API deve retornar 200 OK e um objeto contendo o limite, o valor gasto e o saldo restante (ex: { limite: 500, gasto: 350, saldo: 150 }). |

# Plano de Testes de Software — R13 Análise de Tendências

## 1. Introdução
Este plano de testes de software foca em garantir que a funcionalidade de Análise de Tendências opere conforme a especificação, validando a corretude do algoritmo de análise e o tratamento de diferentes cenários de dados históricos do usuário.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
|---|---|---|---|---|
| CT-R13-001 | Identificar Tendência de Alta | Usuário autenticado possui despesas na categoria "Lazer" com aumento consistente nos últimos 3 meses (ex: R$100, R$200, R$400). | 1. Solicitar via API a análise de tendência para a categoria "Lazer" no período de 3 meses. | A API deve retornar 200 OK e um objeto de análise indicando `tipoTendencia: "Alta"` e uma descrição textual correspondente. |
| CT-R13-002 | Identificar Tendência de Baixa | Usuário autenticado possui despesas na categoria "Transporte" com queda consistente nos últimos 3 meses (ex: R$300, R$150, R$50). | 1. Solicitar via API a análise de tendência para a categoria "Transporte". | A API deve retornar 200 OK e um objeto de análise indicando `tipoTendencia: "Baixa"` e uma descrição textual correspondente. |
| CT-R13-003 | Identificar Tendência Estável | Usuário autenticado possui despesas na categoria "Alimentação" com valores relativamente constantes (ex: R$500, R$510, R$495). | 1. Solicitar via API a análise de tendência para a categoria "Alimentação". | A API deve retornar 200 OK e um objeto de análise indicando `tipoTendencia: "Estável"`. |
| CT-R13-004 | Tratamento de Dados Insuficientes | Usuário autenticado possui transações em apenas um único mês. | 1. Solicitar via API a análise de tendência. | O sistema não deve quebrar. A API deve retornar uma resposta controlada (ex: 200 OK com uma mensagem "Dados insuficientes para análise"). |
| CT-R13-005 | Análise com Períodos Diferentes | Usuário possui 6 meses de dados, com uma tendência de alta nos últimos 3 meses, mas uma média estável nos 6 meses. | 1. Solicitar a análise para o período de "3 meses". <br> 2. Solicitar a análise para o período de "6 meses". | Os resultados devem ser consistentes com cada período. A primeira análise deve retornar "Alta", e a segunda deve retornar "Estável". |
| CT-R13-006 | Análise sem dados no período | Usuário autenticado não possui nenhuma despesa registrada no período solicitado (ex: últimos 30 dias). | 1. Solicitar via API a análise de tendência para o período sem dados. | A API deve retornar uma resposta controlada, informando que não há transações para analisar naquele período. |

# Plano de Testes de Software — R14 Projeção Financeira

## 1. Introdução
Este plano de testes de software foca em garantir que a funcionalidade de Projeção Financeira opere conforme a especificação, validando a precisão do algoritmo de cálculo e o tratamento de diferentes cenários de dados históricos.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
|---|---|---|---|---|
| CT-R14-001 | Projeção com fluxo de caixa positivo | Usuário autenticado com histórico financeiro estável onde a média de receitas é maior que a de despesas (ex: +R$1.000/mês). | 1. Solicitar via API uma projeção financeira para os próximos 6 meses. | A API deve retornar 200 OK. O gráfico e o resumo da projeção devem mostrar um crescimento consistente do saldo ao longo dos 6 meses. |
| CT-R14-002 | Projeção com fluxo de caixa negativo | Usuário autenticado com histórico financeiro estável onde a média de despesas é maior que a de receitas (ex: -R$500/mês). | 1. Solicitar via API uma projeção financeira para os próximos 6 meses. | A API deve retornar 200 OK. O gráfico e o resumo da projeção devem mostrar uma queda consistente do saldo ao longo dos 6 meses. |
| CT-R14-003 | Tratamento de dados insuficientes para projeção | Usuário autenticado com menos de 2 meses de histórico de transações. | 1. Solicitar via API uma projeção financeira. | O sistema não deve quebrar. A API deve retornar uma resposta controlada (ex: 200 OK com uma mensagem "Dados insuficientes para uma projeção precisa"). |
| CT-R14-004 | Projeção para novo usuário sem transações | Usuário recém-cadastrado, sem nenhuma receita ou despesa registrada. | 1. Acessar a funcionalidade de projeção financeira. | O sistema deve exibir uma mensagem amigável, informando que é necessário registrar movimentações antes de gerar uma projeção. |
| CT-R14-005 | Validação de diferentes períodos de projeção | Usuário autenticado com histórico financeiro estável. | 1. Gerar uma projeção para 3 meses. <br> 2. Gerar uma projeção para 12 meses. | Ambos os relatórios devem ser gerados com sucesso. A projeção de 12 meses deve ser uma extensão lógica da projeção de 3 meses, seguindo a mesma média. |
| CT-R14-006 | Impacto de transação atípica | Usuário possui histórico estável, mas com um grande ganho único (ex: venda de um carro) no último mês. | 1. Solicitar uma projeção financeira. | O algoritmo de projeção deve ser robusto o suficiente para não superestimar drasticamente os ganhos futuros com base em um evento único e não recorrente. |

# Plano de Testes de Software — R15 Ferramentas Interativas

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Ferramentas Interativas. Conforme a arquitetura 100% client-side, os testes se concentram na validação da lógica de cálculo implementada em JavaScript e no tratamento de entradas de dados do usuário diretamente no frontend, assegurando que as calculadoras sejam precisas e robustas.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R15-001** | Frontend - Lógica da Calculadora de Metas | Nenhuma. | 1. Na Calculadora de Metas, inserir `Valor do Objetivo = 12000` e `Prazo = 24 meses`. <br> 2. Clicar em "Calcular". | A interface deve exibir o resultado "R$ 500,00 por mês". O cálculo JavaScript deve retornar `500`. |
| **CT-R15-002** | Frontend - Lógica da Calculadora de Ponto de Equilíbrio | Nenhuma. | 1. Na Calculadora de Ponto de Equilíbrio, inserir `Custos Fixos = 3000`, `Preço de Venda = 10`, `Custo Variável = 4`. <br> 2. Clicar em "Calcular". | A interface deve exibir o resultado "500 unidades". O cálculo JavaScript deve retornar `500`. |
| **CT-R15-003** | Frontend - Validação de Entrada (Calculadora de Metas) | Nenhuma. | 1. Na Calculadora de Metas, deixar o campo `Prazo` em branco ou com valor `0`. <br> 2. Clicar em "Calcular". | O sistema não deve quebrar. Uma mensagem de erro amigável deve ser exibida ao usuário (ex: "O prazo deve ser maior que zero"). |
| **CT-R15-004** | Frontend - Validação de Entrada (Calculadora de Ponto de Equilíbrio) | Nenhuma. | 1. Na Calculadora de Ponto de Equilíbrio, inserir um `Preço de Venda` menor que o `Custo Variável`. <br> 2. Clicar em "Calcular". | O sistema não deve quebrar. Uma mensagem de erro deve ser exibida (ex: "O preço de venda deve ser maior que o custo"). |
| **CT-R15-005** | Frontend - Tratamento de Divisão por Zero | Nenhuma. | 1. Na Calculadora de Ponto de Equilíbrio, inserir `Preço de Venda` igual ao `Custo Variável` (ex: 10 e 10). <br> 2. Clicar em "Calcular". | O sistema não deve retornar "Infinity" ou quebrar. Uma mensagem de erro clara deve ser exibida ao usuário. |

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

