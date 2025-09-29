# Projeto de Interface — R9 Metas Financeiras

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de metas financeiras, desde a criação da meta até o acompanhamento e conclusão.

```mermaid
graph TD
    A([Usuário acessa Metas Financeiras]) --> B[Exibir lista de metas do usuário]
    B --> C{Deseja criar nova meta?}
    C -- Sim --> D[Preencher dados da meta: título, valor objetivo, data limite, descrição]
    D --> E{Dados válidos?}
    E -- Não --> F[Exibir mensagem de erro]
    F --> D
    E -- Sim --> G[Salvar meta]
    G --> B
    C -- Não --> H{Seleciona meta existente?}
    H -- Sim --> I[Exibir detalhes da meta]
    I --> J{Deseja registrar aporte?}
    J -- Sim --> K[Preencher valor do aporte]
    K --> L{Valor válido?}
    L -- Não --> M[Exibir erro de valor]
    M --> K
    L -- Sim --> N[Registrar aporte e atualizar valor atual]
    N --> I
    J -- Não --> O{Deseja editar ou remover meta?}
    O -- Editar --> P[Editar dados da meta]
    P --> E
    O -- Remover --> Q[Remover meta e aportes]
    Q --> B
    O -- Nenhuma --> B
    H -- Não --> B
    I --> R{Meta concluída?}
    R -- Sim --> S[Exibir mensagem de parabéns]
    S --> B
    R -- Não --> B

```

### 1.2 Protótipos de Telas (Sugestão)

- **Tela de Listagem de Metas:**

  - Lista todas as metas do usuário, mostrando título, valor objetivo, valor atual, progresso (%) e status (em andamento/concluída).
  - Botão para criar nova meta.
  - Ações: visualizar detalhes, editar, remover.

- **Tela de Detalhes da Meta:**

  - Exibe informações completas da meta.
  - Lista de aportes realizados (data, valor).
  - Campo para registrar novo aporte.
  - Indicador visual de progresso (barra ou círculo).
  - Botão para editar/remover meta.

- **Tela de Criação/Edição de Meta:**

  - Formulário para inserir/editar título, valor objetivo, data limite, descrição.
  - Validação de campos obrigatórios.

- **Feedback Visual:**
  - Mensagens de sucesso, erro e conclusão de meta.
  - Indicadores de progresso e status.
