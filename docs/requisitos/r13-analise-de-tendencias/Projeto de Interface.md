# Projeto de Interface — R4 Relatórios, Diagnósticos e Resultados

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de geração de relatórios, diagnósticos e resultados, desde a solicitação do usuário até a exibição do relatório detalhado.

```mermaid
graph TD
    A([Usuário acessa Relatórios]) --> B[Selecionar tipo de relatório, período e filtros]
    B --> C{"Dados válidos e completos?"}
    C -- Sim --> D[Chamar ServicoRelatorio para gerar relatório]
    C -- Não --> E[Exibir mensagem de erro: Dados inválidos ou incompletos]
    E --> B
    D --> F{"Relatório gerado com sucesso?"}
    F -- Sim --> G[Exibir relatório, diagnóstico e resultados]
    F -- Não --> H[Exibir mensagem de erro: Falha na geração]
    G --> I{Deseja exportar ou salvar?}
    I -- Sim --> J[Exportar/Salvar relatório]
    I -- Não --> K([FIM])
    J --> K
```
