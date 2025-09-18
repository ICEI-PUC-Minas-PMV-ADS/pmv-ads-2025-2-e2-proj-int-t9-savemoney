# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas e utilizar as calculadoras. Conforme a decisão de arquitetura, ambas as ferramentas operam inteiramente no frontend (client-side), com os cálculos sendo executados em tempo real a partir dos dados inseridos manualmente pelo usuário.

```mermaid
graph TD
    A([Usuário clica em 'Ferramentas' no menu]) --> B[Abre a tela 'Hub de Ferramentas'];
    B --> C{Usuário escolhe a calculadora};
    
    C -- 'Calculadora de Metas' --> D[Abre a tela da Calculadora de Metas];
    D --> E[Usuário preenche os campos da meta];
    E --> F[Clica em 'Calcular'];
    F --> G([Lógica JavaScript calcula e exibe o resultado]);
    
    C -- 'Ponto de Equilíbrio' --> H[Abre a tela da Calculadora de Ponto de Equilíbrio];
    H --> I[Usuário preenche Custos Fixos, Preço de Venda e Custo Variável];
    I --> J[Clica em 'Calcular'];
    J --> K([Lógica JavaScript calcula e exibe o resultado]);
```