# Projeto de Interface — R2 Educação Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para acessar, interagir (curtir) e visualizar o conteúdo educativo, detalhando a comunicação com o backend para buscar artigos agregados e gerenciar os favoritos do usuário.

```mermaid
graph TD
    A[Usuário clica em 'Educação Financeira' no menu] --> B{Solicitar lista de artigos};
    B --> C[GET /api/educacao];
    C --> D[Backend consulta artigos em cache no BD];
    D --> E[Backend retorna lista de artigos];
    E --> F{Frontend exibe a lista de artigos};

    F --> G{Usuário decide a ação};
    
    G -- Clica em um card para ler --> H[App abre o link original do artigo em um navegador/WebView];
    
    G -- Clica no ícone 'Curtir' --> I{Enviar requisição para salvar como favorito};
    I --> J[POST /api/educacao/{id}/curtir];
    J --> K[Backend salva na tabela 'artigos_curtidos'];
    K --> L[UI atualiza o ícone para 'curtido'];

    G -- Clica no filtro 'Meus Curtidos' --> M{Solicitar lista de artigos favoritados};
    M --> N[GET /api/educacao/curtidos];
    N --> O[Backend consulta e retorna a lista de curtidos];
    O --> F;

    H --> Z[Fim da interação];
    L --> Z;