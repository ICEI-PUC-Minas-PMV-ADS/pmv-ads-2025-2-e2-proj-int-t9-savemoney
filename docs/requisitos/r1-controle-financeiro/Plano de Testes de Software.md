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
