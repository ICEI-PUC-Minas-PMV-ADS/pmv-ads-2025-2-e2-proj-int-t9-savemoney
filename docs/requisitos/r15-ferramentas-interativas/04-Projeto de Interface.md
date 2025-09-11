# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas, escolher uma calculadora e interagir com ela. O fluxograma detalha os dois principais casos de uso do MVP: a "Calculadora de Metas" (com lógica no frontend) e a "Calculadora de Ponto de Equilíbrio", que requer comunicação com o backend para gerenciar uma lista de produtos cadastrados pelo usuário.

```mermaid
graph TD
    A[1. Usuário clica em 'Ferramentas'] --> B[2. Abre a tela 'Hub de Ferramentas'];
    B --> C{3. Usuário escolhe a calculadora};
    
    C -- 'Calculadora de Metas' --> D[4a. Abre a tela da Calculadora de Metas];
    D --> E[5a. Usuário preenche os campos da meta];
    E --> F[6a. Clica em 'Calcular'];
    F --> G[7a. Lógica JavaScript calcula e exibe o resultado];
    
    C -- 'Ponto de Equilíbrio' --> H[4b. Abre a tela da Calculadora de Ponto de Equilíbrio];
    H --> I[5b. Frontend solicita produtos via GET /api/produtos];
    I --> J[6b. Frontend popula o seletor de produtos];
    J --> K{7b. Precisa gerenciar produtos?};
    
    K -- Sim --> L[8b. Abre modal de gestão de produtos];
    L --> M[9b. Usuário salva produto via POST /api/produtos];
    M --> I;
    
    K -- Não --> N[8c. Usuário preenche 'Custos Fixos'];
    N --> O[9c. Usuário seleciona um produto da lista];
    O --> P[10c. Clica em 'Calcular'];
    P --> Q[11c. Lógica JavaScript calcula e exibe o resultado];