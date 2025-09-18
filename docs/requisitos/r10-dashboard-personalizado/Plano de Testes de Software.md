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
