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
