# Projeto de Interface — R5 Exportação e Compartilhamento
## 1. Modelos Funcionais
### 1.1 Diagrama de Fluxo (Fluxograma)
Este diagrama representa o fluxo de interação do usuário para as funcionalidades de exportação e compartilhamento de um relatório. O fluxo se inicia a partir do momento em que um relatório (requisito R4) já está sendo exibido na tela, detalhando os passos para a geração do arquivo no backend e a subsequente ação de compartilhamento no frontend.

```mermaid
graph TD
    A[Usuário visualiza um relatório na tela] --> B[Clica no botão 'Exportar'];
    B --> C{Escolhe o formato do arquivo};
    C -- PDF --> D["Frontend envia requisição para API (formato=PDF)"];
    C -- Excel --> E["Frontend envia requisição para API (formato=Excel)"];

    subgraph Processamento no Backend
        D --> F[Backend gera o arquivo .pdf];
        E --> G[Backend gera o arquivo .xlsx];
    end

    F --> H[Backend retorna o arquivo para download];
    G --> H;

    H --> I[Navegador inicia o download do arquivo];
    I --> J{Deseja compartilhar o arquivo?};
    J -- Sim --> K[Clica no botão 'Compartilhar'];
    K --> L[Frontend aciona a API de compartilhamento nativa do dispositivo];
    L --> M[Usuário seleciona app (WhatsApp, E-mail, etc.) e envia];

    J -- Não --> N([Fim da Interação]);
    M --> N;
```
