# Projeto de Interface — R16 Histórico Financeiro

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de visualização do histórico financeiro, desde a seleção do período até a exibição dos saldos e movimentações.

```mermaid
graph TD
    A([Usuário acessa Histórico Financeiro]) --> B[Seleciona período de consulta]
    B --> C[Buscar movimentações e saldos]
    C --> D{Movimentações encontradas?}
    D -- Sim --> E[Exibir lista de movimentações e gráfico de saldo]
    D -- Não --> F[Exibir mensagem: Nenhuma movimentação encontrada]
    E --> G[Permitir exportação do relatório]
    G --> H([FIM])
    F --> H
```

## 2. Protótipos de Telas

- Tela de seleção de período
- Lista de movimentações (com filtros por categoria, tipo, valor)
- Gráfico de evolução do saldo
- Botão para exportar relatório (PDF/Excel)

## 3. Requisitos de Interface

- Interface clara e intuitiva
- Gráficos responsivos
- Filtros avançados para busca de movimentações
- Opção de exportação de dados
