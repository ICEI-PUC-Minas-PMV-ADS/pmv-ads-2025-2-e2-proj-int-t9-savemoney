# Projeto de Interface — R10 Dashboard Personalizado

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de customização do dashboard financeiro, incluindo adição, remoção, configuração e reordenação (drag and drop) de widgets.

```mermaid
graph TD
    A([Usuário acessa o Dashboard]) --> B[Visualiza painel com widgets]
    B --> C{Deseja adicionar widget?}
    C -- Sim --> D[Seleciona tipo e configura widget]
    D --> E[Widget adicionado ao painel]
    C -- Não --> F{Deseja remover widget?}
    F -- Sim --> G[Remove widget do painel]
    F -- Não --> H{Deseja reordenar widgets?}
    H -- Sim --> I[Arrasta e solta widget na nova posição]
    I --> J[Ordem dos widgets atualizada]
    H -- Não --> K{Deseja editar configuração de um widget?}
    K -- Sim --> L[Edita e salva configuração do widget]
    L --> M[Widget atualizado]
    K -- Não --> N[Fim da customização]
    E --> B
    G --> B
    J --> B
    M --> B
    N --> O([FIM])
```
