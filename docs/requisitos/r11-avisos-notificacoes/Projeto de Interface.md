# Projeto de Interface — R11 Avisos e Notificações

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de exibição e interação com avisos e notificações, incluindo visualização, marcação como lida e atualização de indicadores visuais.

```mermaid
graph TD
    A([Usuário acessa o sistema]) --> B[Visualiza painel de notificações]
    B --> C{Há novas notificações?}
    C -- Sim --> D[Exibe alerta visual e mensagem]
    D --> E[Usuário lê notificação]
    E --> F[Marca notificação como lida]
    F --> G[Indicador visual atualizado]
    C -- Não --> H{Deseja ver histórico?}
    H -- Sim --> I[Exibe histórico de notificações]
    H -- Não --> J[Fim da interação]
    G --> B
    I --> B
    J --> K([FIM])
```
