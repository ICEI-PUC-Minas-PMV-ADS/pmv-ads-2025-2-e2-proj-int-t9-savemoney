# Projeto de Interface — R2 Educação Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para acessar, interagir (curtir) e visualizar o conteúdo educativo, detalhando a comunicação com o backend para buscar artigos agregados e gerenciar os favoritos do usuário.

```mermaid
graph TD
    subgraph "Inicio e Carregamento"
        A[1. Usuário clica em 'Educação Financeira'] --> B[2. Frontend solicita lista de artigos];
        B --> C[3. GET /api/educacao];
        C --> D[4. Backend retorna lista de artigos];
        D --> E[5. Frontend exibe a lista de artigos];
    end

    subgraph "Interacao do Usuario"
        E --> F{6. Usuário decide a ação};
        F -- Leitura --> G[7a. App abre o link original do artigo];
        F -- Interação --> H{7b. Clica no ícone 'Curtir'};
        H --> I[8b. POST /api/educacao/{id}/curtir];
        I --> J[9b. Backend salva o favorito];
        J --> K[10b. UI atualiza o ícone];
        F -- Navegação --> L{7c. Clica no filtro 'Meus Curtidos'};
        L --> M[8c. GET /api/educacao/curtidos];
        M --> N[9c. Backend retorna a lista de favoritos];
        N --> E;
    end