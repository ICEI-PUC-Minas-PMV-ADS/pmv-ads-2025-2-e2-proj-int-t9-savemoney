# Projeto de Interface — R2 Educação Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para acessar, interagir (curtir) e visualizar o conteúdo educativo, detalhando a comunicação com o backend para buscar artigos agregados e gerenciar os favoritos do usuário.

```mermaid
graph TD
    A[Usuário clica em 'Educação Financeira'] --> B[Frontend solicita lista de conteúdos via API];
    B --> C[Backend busca conteúdo de fontes externas];
    C --> D[Backend retorna a lista para o Frontend];
    D --> E[Frontend exibe a lista de conteúdos];
    E --> F[Usuário clica em um conteúdo para ler];
    F --> G[App abre o link original do conteúdo em um navegador/WebView];
```
