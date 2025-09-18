# Projeto de Interface — R2 Educação Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para acessar, interagir (curtir) e visualizar o conteúdo educativo, detalhando a comunicação com o backend para buscar artigos agregados e gerenciar os favoritos do usuário.

```mermaid
graph TD
    A(Usuário clica em 'Educação Financeira') --> B[Frontend solicita lista de conteúdos];
    B --> C[Backend retorna a lista];
    C --> D[Frontend exibe os conteúdos];
    D --> E{Usuário decide a ação};

    E -- Leitura --> F[App abre o link original do conteúdo];

    E -- Curtir --> G[Clica no ícone 'Curtir'];
    G --> H[Frontend envia POST de Curtir Conteúdo];
    H --> I[Backend salva a curtida];
    I --> J[UI atualiza o ícone para 'curtido'];

    E -- Ver Curtidos --> K[Clica no filtro 'Meus Curtidos'];
    K --> L[Frontend solicita conteúdos curtidos];
    L --> M(Backend retorna a lista de curtidos);
    M --> D;
```
