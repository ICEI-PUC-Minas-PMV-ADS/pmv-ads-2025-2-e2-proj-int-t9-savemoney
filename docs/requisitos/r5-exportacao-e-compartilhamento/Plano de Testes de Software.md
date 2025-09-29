# Plano de Testes de Software — R4 Relatórios, Diagnósticos e Resultados

## 1. Introdução
Este plano de testes de software tem como objetivo validar a funcionalidade de **Relatórios, Diagnósticos e Resultados (R4)** no sistema SaveMoney V2.  
Essa funcionalidade deve permitir que usuários, tanto pessoa física (ex: João, estudante) quanto pessoa jurídica (ex: Maria, empreendedora), gerem relatórios detalhados sobre suas finanças, recebam diagnósticos automáticos e tenham acesso a resultados claros e organizados que auxiliem na tomada de decisões.

---

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                                                 | Ações                                                                                  | Resultados Esperados                                                                                   |
| ------ | ------------------------------------- | ------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| CT-101 | Geração de relatório detalhado        | Usuário autenticado (PF ou PJ) com dados financeiros cadastrados               | 1. Solicitar relatório detalhado para um período específico.                           | O sistema deve gerar e exibir o relatório com dados corretos, completos e organizados em formato visual.|
| CT-102 | Aplicação de filtros                  | Usuário autenticado com múltiplos registros financeiros                        | 1. Selecionar filtros (ex: categoria, período, tipo de transação).<br>2. Gerar relatório. | O relatório deve considerar apenas os dados filtrados, refletindo exatamente a seleção do usuário.     |
| CT-103 | Diagnóstico automático                | Usuário autenticado com histórico financeiro suficiente                        | 1. Solicitar diagnóstico financeiro.<br>2. Confirmar geração.                         | O sistema deve exibir um diagnóstico com análises automáticas e recomendações personalizadas.           |
| CT-104 | Exportação de relatório               | Relatório gerado e exibido na tela                                             | 1. Clicar em **Exportar**.<br>2. Selecionar formato (PDF, Excel).                     | O sistema deve exportar o relatório no formato escolhido, mantendo integridade, layout e dados.        |
| CT-105 | Relatórios para Pessoa Jurídica       | Usuário autenticado como PJ, com dados de fluxo de caixa cadastrados           | 1. Gerar relatório mensal de fluxo de caixa.<br>2. Visualizar diagnóstico.             | O sistema deve gerar relatório empresarial claro, com visão gerencial simplificada (DRE), pronto para envio a contador. |
| CT-106 | Mensagem de erro para dados inválidos | Usuário não preenche filtros obrigatórios ou insere período inexistente        | 1. Tentar gerar relatório sem preencher campos obrigatórios.<br>2. Requisitar relatório. | O sistema deve exibir mensagem de erro clara, informando os campos obrigatórios que faltam ou inconsistências. |
| CT-107 | Atualização de relatório              | Relatório já gerado anteriormente                                              | 1. Alterar filtros ou parâmetros.<br>2. Requisitar novo relatório.                     | O sistema deve atualizar o relatório conforme os novos parâmetros e exibir o resultado atualizado.      |

---
