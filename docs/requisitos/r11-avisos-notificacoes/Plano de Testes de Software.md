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
