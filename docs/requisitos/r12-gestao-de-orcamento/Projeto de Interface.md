# Projeto de Interface — R12 Gestão de Orçamento

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de gestão de orçamentos, desde a solicitação do usuário até a criação, consulta e ajuste de orçamentos mensais por categoria.

```mermaid
graph TD
    A([Usuário acessa Gestão de Orçamento]) --> B[Selecionar categoria e período (mês/ano)]
    B --> C{Deseja criar, editar ou consultar?}
    C -- Criar --> D[Informar valor limite e confirmar]
    D --> E[Validar dados de entrada]
    E -- Válido --> F[Salvar orçamento no banco de dados]
    E -- Inválido --> G[Exibir mensagem de erro e solicitar correção]
    F --> H[Exibir confirmação de criação]
    G --> B

    C -- Editar --> I[Selecionar orçamento existente]
    I --> J[Alterar valor limite e confirmar]
    J --> E

    C -- Consultar --> K[Listar orçamentos do período]
    K --> L[Exibir detalhes: categoria, limite, gasto, saldo]
    L --> M{Deseja ajustar orçamento?}
    M -- Sim --> I
    M -- Não --> N([FIM])
    H --> N
```
