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

