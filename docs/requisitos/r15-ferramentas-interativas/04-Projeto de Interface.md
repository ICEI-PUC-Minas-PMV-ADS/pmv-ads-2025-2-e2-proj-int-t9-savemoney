# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas, escolher uma calculadora e interagir com ela. O fluxograma detalha os dois principais casos de uso do MVP: a "Calculadora de Metas" (com lógica no frontend) e a "Calculadora de Ponto de Equilíbrio", que requer comunicação com o backend para gerenciar uma lista de produtos cadastrados pelo usuário.

```mermaid
graph TD
    A[1. Usuário clica em 'Ferramentas' no menu] --> B[2. Abre a tela 'Hub de Ferramentas'];
    B --> C{3. Usuário escolhe a calculadora};
    
    C -- "Calculadora de Metas" --> D_Metas;
    C -- "Ponto de Equilíbrio" --> E_PE;

    subgraph Fluxo 1: Calculadora de Metas (Persona: João)
        D_Metas[4a. Abre a tela da Calculadora de Metas] --> F_Metas[5a. Usuário preenche campos da meta];
        F_Metas --> G_Metas[6a. Clica em 'Calcular'];
        G_Metas --> H_Metas[7a. Lógica JavaScript executa o cálculo];
        H_Metas --> I_Metas[8a. Exibe resultado com texto e gráfico];
    end

    subgraph Fluxo 2: Calculadora de Ponto de Equilíbrio (Persona: Maria)
        E_PE[4b. Abre a tela da Calculadora de Ponto de Equilíbrio] --> F_PE[5b. Frontend solicita produtos via GET /api/produtos];
        F_PE --> G_PE[6b. Frontend popula o seletor de produtos];
        
        G_PE --> H_PE{7b. Precisa gerenciar produtos?};
        
        H_PE -- Sim --> I_PE[8b. Abre modal de gestão de produtos];
        I_PE --> J_PE[9b. Usuário salva um produto via POST /api/produtos];
        J_PE --> F_PE;

        H_PE -- Não --> K_PE[8c. Usuário preenche 'Custos Fixos'];
        K_PE --> L_PE[9c. Usuário seleciona um produto da lista];
        L_PE --> M_PE[10c. Clica em 'Calcular'];
        M_PE --> N_PE[11c. Lógica JavaScript executa o cálculo];
        N_PE --> O_PE[12c. Exibe resultado com texto e gráfico];
    end