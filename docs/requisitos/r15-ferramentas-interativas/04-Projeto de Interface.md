# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas, escolher uma calculadora e interagir com ela. O fluxograma detalha os dois principais casos de uso do MVP: a "Calculadora de Metas" (com lógica no frontend) e a "Calculadora de Ponto de Equilíbrio", que requer comunicação com o backend para gerenciar uma lista de produtos cadastrados pelo usuário.

```mermaid
graph TD
    A[Usuário clica em 'Ferramentas' no menu principal] --> B{Abre a tela 'Hub de Ferramentas'};
    B --> C{Usuário visualiza as opções disponíveis};
    C --> D[Opção 1: Calculadora de Metas];
    C --> E[Opção 2: Calculadora de Ponto de Equilíbrio];
    
    subgraph Fluxo da Calculadora de Metas (Client-Side)
        D -- Seleciona --> F[Abre a tela da Calculadora de Metas];
        F --> G[Usuário preenche campos: <br> Valor do Objetivo, Prazo, etc.];
        G --> H[Clica em 'Calcular'];
        H --> I{Lógica JavaScript executa o cálculo};
        I --> J[Exibe resultado com texto e gráfico de projeção];
    end

    subgraph Fluxo da Calculadora de Ponto de Equilíbrio (com Backend)
        E -- Seleciona --> K[Abre a tela da Calculadora de Ponto de Equilíbrio];
        
        subgraph Carregamento e Gestão de Dados
            K --> L{Solicitar lista de produtos do usuário};
            L --> M[GET /api/produtos];
            M --> N[Backend retorna a lista de produtos da Maria];
            N --> O{Frontend popula o seletor 'Escolha um Produto'};
            
            O --> P[Usuário clica em 'Gerenciar Produtos'];
            P --> Q{Abre modal de Cadastro/Edição de Produtos};
            Q --> R[Usuário adiciona/edita um produto];
            R --> S[POST /api/produtos];
            S --> T[Backend salva o produto e retorna sucesso];
            T --> L;
        end

        subgraph Cálculo
            O --> U[Usuário preenche campo 'Custos Fixos Mensais'];
            U --> V[Usuário seleciona um produto já cadastrado];
            V --> W[Clica em 'Calcular'];
            W --> X{Lógica JavaScript calcula o ponto de equilíbrio};
            X --> Y[Exibe resultado com texto e gráfico de break-even];
        end
    end

    J --> Z[Fim da interação];
    Y --> Z;