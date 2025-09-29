# Projeto de Interface — R4 Relatórios, Diagnósticos e Resultados

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de Projeção Financeira, desde a solicitação do usuário, passando pelo processamento dos dados históricos, até a exibição da projeção futura de seu saldo.

```mermaid
graph TD
    A([Usuário acessa Projeção Financeira]) --> B[Seleciona o período para a projeção (ex: 6, 12 meses)];
    B --> C{"Existem dados históricos suficientes?"};
    C -- Sim --> D[Backend analisa o histórico e calcula a projeção];
    C -- Não --> E[Exibir mensagem de erro: 'Dados insuficientes para projetar'];
    D --> F{"Projeção gerada com sucesso?"};
    F -- Sim --> G[Exibir projeção na tela (resumo em texto + gráfico de linhas)];
    F -- Não --> H[Exibir mensagem de erro: 'Falha ao gerar projeção'];
    G --> I([FIM]);
    E --> I;
    H --> I;

```
