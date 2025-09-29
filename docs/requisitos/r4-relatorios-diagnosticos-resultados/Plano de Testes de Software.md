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
