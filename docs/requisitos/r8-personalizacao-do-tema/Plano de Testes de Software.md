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
