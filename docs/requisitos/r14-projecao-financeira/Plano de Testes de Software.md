# Plano de Testes de Software — R14 Projeção Financeira

## 1. Introdução
Este plano de testes de software foca em garantir que a funcionalidade de Projeção Financeira opere conforme a especificação, validando a precisão do algoritmo de cálculo e o tratamento de diferentes cenários de dados históricos.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
|---|---|---|---|---|
| CT-R14-001 | Projeção com fluxo de caixa positivo | Usuário autenticado com histórico financeiro estável onde a média de receitas é maior que a de despesas (ex: +R$1.000/mês). | 1. Solicitar via API uma projeção financeira para os próximos 6 meses. | A API deve retornar 200 OK. O gráfico e o resumo da projeção devem mostrar um crescimento consistente do saldo ao longo dos 6 meses. |
| CT-R14-002 | Projeção com fluxo de caixa negativo | Usuário autenticado com histórico financeiro estável onde a média de despesas é maior que a de receitas (ex: -R$500/mês). | 1. Solicitar via API uma projeção financeira para os próximos 6 meses. | A API deve retornar 200 OK. O gráfico e o resumo da projeção devem mostrar uma queda consistente do saldo ao longo dos 6 meses. |
| CT-R14-003 | Tratamento de dados insuficientes para projeção | Usuário autenticado com menos de 2 meses de histórico de transações. | 1. Solicitar via API uma projeção financeira. | O sistema não deve quebrar. A API deve retornar uma resposta controlada (ex: 200 OK com uma mensagem "Dados insuficientes para uma projeção precisa"). |
| CT-R14-004 | Projeção para novo usuário sem transações | Usuário recém-cadastrado, sem nenhuma receita ou despesa registrada. | 1. Acessar a funcionalidade de projeção financeira. | O sistema deve exibir uma mensagem amigável, informando que é necessário registrar movimentações antes de gerar uma projeção. |
| CT-R14-005 | Validação de diferentes períodos de projeção | Usuário autenticado com histórico financeiro estável. | 1. Gerar uma projeção para 3 meses. <br> 2. Gerar uma projeção para 12 meses. | Ambos os relatórios devem ser gerados com sucesso. A projeção de 12 meses deve ser uma extensão lógica da projeção de 3 meses, seguindo a mesma média. |
| CT-R14-006 | Impacto de transação atípica | Usuário possui histórico estável, mas com um grande ganho único (ex: venda de um carro) no último mês. | 1. Solicitar uma projeção financeira. | O algoritmo de projeção deve ser robusto o suficiente para não superestimar drasticamente os ganhos futuros com base em um evento único e não recorrente. |