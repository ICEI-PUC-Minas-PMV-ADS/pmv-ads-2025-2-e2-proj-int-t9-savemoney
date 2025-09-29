# Projeto de Interface — R14 Projeção Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de Projeção Financeira, desde a solicitação do usuário até a exibição da projeção e exportação dos resultados.

```mermaid
graph TD
    A([Usuário acessa Projeção Financeira]) --> B[Seleciona o período para a projeção (ex: 6, 12 meses)]
    B --> C{"Existem dados históricos suficientes?"}
    C -- Sim --> D[Backend calcula a projeção]
    C -- Não --> E[Exibir mensagem: Dados insuficientes para projetar]
    E --> B
    D --> F{"Projeção gerada com sucesso?"}
    F -- Sim --> G[Exibir projeção: gráfico e resumo]
    F -- Não --> H[Exibir mensagem de erro: Falha ao gerar projeção]
    G --> I{Deseja exportar ou salvar?}
    I -- Sim --> J[Exportar/Salvar projeção]
    I -- Não --> K([FIM])
    J --> K
```

### 1.2 Telas Principais

#### Tela: Projeção Financeira

- **Componentes:**
  - Filtro de período para projeção (ex: dropdown com 3, 6, 12 meses)
  - Botão "Gerar Projeção"
  - Gráfico de linhas mostrando a evolução do saldo projetado mês a mês
  - Resumo textual da projeção (ex: "Seu saldo projetado para 6 meses é R$ X.XXX,XX")
  - Mensagens de erro ou alerta caso não haja dados suficientes
  - Botão "Exportar" (PDF/CSV)

#### Tela: Detalhe da Projeção

- **Componentes:**
  - Gráfico detalhado com valores de saldo projetado por mês
  - Tabela opcional com os valores mês a mês
  - Texto explicativo sobre a metodologia da projeção
  - Botão "Voltar" para a tela principal

### 1.3 Comportamento Esperado

- O usuário seleciona o período desejado e solicita a projeção.
- O sistema exibe o gráfico de saldo projetado e um resumo textual.
- Caso não haja dados suficientes, uma mensagem informativa é exibida.
- O usuário pode exportar a projeção em PDF ou CSV.

## 2. Observações

- A interface deve ser responsiva e acessível.
- Os gráficos devem ser claros, com legendas e destaques para variações relevantes.
- O resumo textual deve ser objetivo e fácil de entender.
