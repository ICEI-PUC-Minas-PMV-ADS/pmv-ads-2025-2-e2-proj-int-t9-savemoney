# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas, escolher uma calculadora e interagir com e o fluxograma detalha os dois principais casos de uso do MVP: a "Calculadora de Metas" (com lógica no frontend) e a "Calculadora de Ponto de Equilíbrio", que requer comunicação com o backend para gerenciar uma lista de produtos cadastrados pelo usuário.

```mermaid
graph TD
    A(Usuário clica em 'Ferramentas') --> B[Abre a tela 'Hub de Ferramentas'];
    B --> C{Usuário escolhe a calculadora};

    C -- 'Calculadora de Metas' --> D[Abre a tela da Calculadora de Metas];
    D --> E[Usuário preenche os campos da meta];
    E --> F[Clica em 'Calcular'];
    F --> G[Lógica JavaScript calcula e exibe o resultado];

    C -- 'Ponto de Equilíbrio' --> H[Abre a tela da Calculadora de Ponto de Equilíbrio];
    H --> I[Frontend solicita produtos via GET /api/produtos];
    I --> J[Frontend popula o seletor de produtos];
    J --> K{Precisa gerenciar produtos?};

    K -- Sim --> L[Abre modal de gestão de produtos];
    L --> M[Usuário salva produto via POST /api/produtos];
    M --> I;

    K -- Não --> N[Usuário preenche 'Custos Fixos'];
    N --> O[Usuário seleciona um produto da lista];
    O --> P[Clica em 'Calcular'];
    P --> Q(Lógica JavaScript calcula e exibe o resultado);
```
