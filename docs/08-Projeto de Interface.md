# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas, escolher uma calculadora e interagir com e o fluxograma detalha os dois principais casos de uso do MVP: a "Calculadora de Metas" (com lógica no frontend) e a "Calculadora de Ponto de Equilíbrio", que requer comunicação com o backend para gerenciar uma lista de produtos cadastrados pelo usuário.

```mermaid
graph TD
    A(Usuário clica em 'Ferramentas') --> B[Abre a tela 'Hub de Ferramentas'];
    B --> C{Usuário escolhe a calculadora};

    C -- 'Calculadora de Metas' --> D[Abre a tela da Calculadora de Metas];
    D --> E[Usuário preenche os campos da meta];
    E --> F[Clica em 'Calcular'];
    F --> G[Lógica JavaScript calcula e exibe o resultado];

    C -- 'Ponto de Equilíbrio' --> H[Abre a tela da Calculadora de Ponto de Equilíbrio];
    H --> I[Frontend solicita produtos via GET /api/produtos];
    I --> J[Frontend popula o seletor de produtos];
    J --> K{Precisa gerenciar produtos?};

    K -- Sim --> L[Abre modal de gestão de produtos];
    L --> M[Usuário salva produto via POST /api/produtos];
    M --> I;

    K -- Não --> N[Usuário preenche 'Custos Fixos'];
    N --> O[Usuário seleciona um produto da lista];
    O --> P[Clica em 'Calcular'];
    P --> Q(Lógica JavaScript calcula e exibe o resultado);
```

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

# Projeto de Interface — R3 Conversor de Energia

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

```mermaid
graph TD
    A([Usuário acessa o Conversor]) --> B[Informar valor, estado, modalidade, tipo de dispositivo e tempo de uso]
    B --> C{"Dados válidos e completos?"}
    C -- Sim --> D[Chamar TarifaService para buscar a tarifa]
    C -- Não --> E[Exibir mensagem de erro: Dados inválidos ou incompletos]
    E --> B
    D --> F{"Tarifa encontrada?"}
    F -- Sim --> G[Calcular conversão]
    F -- Não --> H[Notificar: Tarifa indisponível, usar média?]
    H -- Sim --> I[Usar tarifa média nacional]
    H -- Não --> J[Permitir inserção manual]
    J --> K[Calcular conversão com tarifa manual]
    I --> K
    G --> L[Gerar Dicas Personalizadas]
    K --> L
    L --> M{"Tipo de gráfico desejado?"}
    M -- "Pizza" --> N1[Gerar Gráfico de Pizza]
    M -- "Barra" --> N2[Gerar Gráfico de Barras]
    M -- "Linha" --> N3[Gerar Gráfico de Linhas]
    N1 --> O[Exibir resultado, dicas e gráfico de pizza]
    N2 --> O[Exibir resultado, dicas e gráfico de barras]
    N3 --> O[Exibir resultado, dicas e gráfico de linhas]
    O --> P[Opcional: Salvar histórico de conversão e gráficos]
    P --> Q([FIM])
```

# Projeto de Interface — R3 Conversor de Energia

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

```mermaid
graph TD
    A[Usuário insere os dados do cadastro] --> B{Verificar tipo de pessoa, validar dados obrigatórios, validar email e senha};
    B --> C{Criar Hash da senha};
    C --> D{Registrar usuário no banco de dados};
    M --> N[Fim];
```

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

