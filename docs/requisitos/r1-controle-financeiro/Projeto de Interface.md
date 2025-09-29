# Projeto de Interface — R1 Controle Financeiro

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de registro de receitas e despesas, desde a entrada de dados do usuário até a confirmação do registro e atualização do saldo.

```mermaid
graph TD
    A([Usuário acessa o Controle Financeiro]) --> B[Informar valor, categoria, tipo: receita ou despesa, data e descrição]
    B --> C{"Dados válidos e completos?"}
    C -- Sim --> D[Registrar no sistema]
    C -- Não --> E[Exibir mensagem de erro: Dados inválidos ou incompletos]
    E --> B
    D --> F[Atualizar saldo e histórico do usuário]
    F --> G[Exibir confirmação e novo saldo]
    G --> H([FIM])
```

## 2. Protótipos de Telas

- Tela de Cadastro de Receita/Despesa: Campos para valor, categoria, tipo, data e descrição, botão de salvar.
- Tela de Listagem: Exibe histórico de registros financeiros, filtros por período, categoria e tipo.
- Tela de Saldo: Mostra saldo atual, total de receitas e despesas.

## 3. Navegação

- O usuário pode acessar o cadastro a partir do menu principal.
- Após o registro, retorna à tela de listagem com atualização automática.
- Opção de editar ou remover registros existentes.
