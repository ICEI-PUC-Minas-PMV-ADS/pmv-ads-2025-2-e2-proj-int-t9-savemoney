# Projeto de Interface — R8 Personalização do Tema

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para personalizar e salvar o tema, detalhando a comunicação com o backend e a persistência.

```mermaid
graph TD
    A[Usuário acessa as Configurações] --> B{Solicitar preferências do backend};
    B --> C[GET /api/preferences];
    C --> D[Backend consulta Banco de Dados];
    D --> E[Backend retorna preferências salvas ou padrão];
    E --> F{Frontend aplica o tema};
    F --> G[Usuário customiza tema em tempo real];
    G --> H{Salvar?};
    H -- Sim --> I[POST/PUT /api/preferences];
    I --> J[Backend salva/atualiza no Banco de Dados];
    J --> K[Confirmação de sucesso];
    H -- Não --> L[Fim];
    K --> L;
```

---
