# Projeto de Interface — R3 Conversor de Energia

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

```mermaid
graph TD
    A([Usuário acessa o Conversor]) --> B[Informar valor, estado, modalidade, tipo de dispositivo e tempo de uso]
    B --> C{"Dados válidos e completos?"}
    C -- Sim --> D[Chamar TarifaService para buscar a tarifa]
    C -- Não --> E[Exibir mensagem de erro: Dados inválidos ou incompletos]
    E --> B
    D --> F{"Tarifa encontrada?"}
    F -- Sim --> G[Calcular conversão]
    F -- Não --> H[Notificar: Tarifa indisponível, usar média?]
    H -- Sim --> I[Usar tarifa média nacional]
    H -- Não --> J[Permitir inserção manual]
    J --> K[Calcular conversão com tarifa manual]
    I --> K
    G --> L[Gerar Dicas Personalizadas]
    K --> L
    L --> M{"Tipo de gráfico desejado?"}
    M -- "Pizza" --> N1[Gerar Gráfico de Pizza]
    M -- "Barra" --> N2[Gerar Gráfico de Barras]
    M -- "Linha" --> N3[Gerar Gráfico de Linhas]
    N1 --> O[Exibir resultado, dicas e gráfico de pizza]
    N2 --> O[Exibir resultado, dicas e gráfico de barras]
    N3 --> O[Exibir resultado, dicas e gráfico de linhas]
    O --> P[Opcional: Salvar histórico de conversão e gráficos]
    P --> Q([FIM])
```
