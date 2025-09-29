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
