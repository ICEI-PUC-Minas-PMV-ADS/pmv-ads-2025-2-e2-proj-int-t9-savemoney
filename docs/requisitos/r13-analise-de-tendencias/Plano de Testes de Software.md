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