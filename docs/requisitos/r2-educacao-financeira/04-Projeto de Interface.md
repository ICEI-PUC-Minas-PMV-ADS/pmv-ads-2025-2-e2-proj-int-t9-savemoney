# Projeto de Interface — R2 Educação Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para acessar, interagir (curtir) e visualizar o conteúdo educativo, detalhando a comunicação com o backend para buscar artigos agregados e gerenciar os favoritos do usuário.

```mermaid
graph TD
    A[1. Usuário clica em 'Educação Financeira'] --> B[2. Frontend solicita lista de artigos];
    B --> C[3. Backend retorna a lista];
    C --> D[4. Frontend exibe os artigos];
    D --> E{5. Usuário decide a ação};
    
    E -- Leitura --> F[6a. App abre o link original do artigo];
    
    E -- Curtir --> G[6b. Clica no ícone 'Curtir'];
    G --> H[7b. Frontend envia POST de Curtir Artigo];
    H --> I[8b. Backend salva o favorito];
    I --> J[9b. UI atualiza o ícone para 'curtido'];
    
    E -- Ver Favoritos --> K[6c. Clica no filtro 'Meus Curtidos'];
    K --> L[7c. Frontend solicita artigos curtidos];
    L --> M[8c. Backend retorna a lista de favoritos];
    M --> D;
