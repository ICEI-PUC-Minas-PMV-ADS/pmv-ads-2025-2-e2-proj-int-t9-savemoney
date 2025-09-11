# Projeto de Interface — R3 Conversor de Energia

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

```mermaid
graph TD
    A[Usuário acessa o Conversor] --> B{Informar valor, estado, modalidade, tipo de dispositivo e tempo de uso};
    B --> C[Chamar TarifaService para buscar a tarifa];
    C --> D{Tarifa encontrada?};
    D -- Sim --> E[Calcular conversão];
    D -- Não --> F[Notificar: Tarifa indisponível, usar média?];
    F -- Sim --> G[Usar tarifa média nacional];
    F -- Não --> H[Permitir inserção manual];
    H --> I[Calcular conversão com tarifa manual];
    G --> I;
    E --> J[Gerar Dicas Personalizadas];
    J --> K[Gerar Dados Gráficos Comparativos];
    K --> L[Exibir resultado, dicas e gráficos];
    L --> M[Opcional: Salvar histórico de conversão e gráficos];
    M --> N[Fim];
```
