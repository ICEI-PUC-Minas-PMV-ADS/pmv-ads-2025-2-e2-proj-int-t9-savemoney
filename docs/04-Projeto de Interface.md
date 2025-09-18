# Projeto de Interface

#### Principais Telas e Funcionalidades

1. **Tela de Login e Cadastro**
  - Formulário de autenticação
  - Processo de registro de novos usuários
  - Recuperação de senha

2. **Dashboard Principal**
  - Visão geral das finanças
  - Gráficos de receitas vs despesas
  - Resumo mensal e anual
  - Navegação para funcionalidades principais

3. **Gestão de Transações**
  - Formulário para adicionar receitas e despesas
  - Lista de transações com filtros
  - Edição e exclusão de transações
  - Categorização automática e manual

4. **Relatórios e Análises**
  - Gráficos interativos
  - Filtros por período e categoria
  - Exportação de dados
  - Comparativos mensais/anuais

5. **Configurações e Perfil**
  - Dados pessoais do usuário
  - Preferências da aplicação
  - Categorias personalizadas
  - Configurações de notificações


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
    F -- Não --> H{Notificar: Tarifa indisponível, usar média?}
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

# Projeto de Interface — R4 Relatórios, Diagnósticos e Resultados

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de geração de relatórios, diagnósticos e resultados, desde a solicitação do usuário até a exibição do relatório detalhado.

```mermaid
graph TD
    A([Usuário acessa Relatórios]) --> B[Selecionar tipo de relatório, período e filtros]
    B --> C{"Dados válidos e completos?"}
    C -- Sim --> D[Chamar ServicoRelatorio para gerar relatório]
    C -- Não --> E[Exibir mensagem de erro: Dados inválidos ou incompletos]
    E --> B
    D --> F{"Relatório gerado com sucesso?"}
    F -- Sim --> G[Exibir relatório, diagnóstico e resultados]
    F -- Não --> H[Exibir mensagem de erro: Falha na geração]
    G --> I{Deseja exportar ou salvar?}
    I -- Sim --> J[Exportar/Salvar relatório]
    I -- Não --> K([FIM])
    J --> K
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

# Projeto de Interface — R9 Metas Financeiras

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de metas financeiras, desde a criação da meta até o acompanhamento e conclusão.

```mermaid
graph TD
        A([Usuário acessa Metas Financeiras]) --> B[Exibir lista de metas do usuário]
        B --> C{Deseja criar nova meta?}
        C -- Sim --> D[Preencher dados da meta: título, valor objetivo, data limite, descrição]
        D --> E{Dados válidos?}
        E -- Não --> F[Exibir mensagem de erro]
        F --> D
        E -- Sim --> G[Salvar meta]
        G --> B
        C -- Não --> H{Seleciona meta existente?}
        H -- Sim --> I[Exibir detalhes da meta]
        I --> J{Deseja registrar aporte?}
        J -- Sim --> K[Preencher valor do aporte]
        K --> L{Valor válido?}
        L -- Não --> M[Exibir erro de valor]
        M --> K
        L -- Sim --> N[Registrar aporte e atualizar valor atual]
        N --> I
        J -- Não --> O{Deseja editar ou remover meta?}
        O -- Editar --> P[Editar dados da meta]
        P --> E
        O -- Remover --> Q[Remover meta e aportes]
        Q --> B
        O -- Nenhuma --> B
        H -- Não --> B
        I --> R{Meta concluída?}
        R -- Sim --> S[Exibir mensagem de parabéns]
        S --> B
        R -- Não --> B
    
```

### 1.2 Protótipos de Telas (Sugestão)

- **Tela de Listagem de Metas:**
    - Lista todas as metas do usuário, mostrando título, valor objetivo, valor atual, progresso (%) e status (em andamento/concluída).
    - Botão para criar nova meta.
    - Ações: visualizar detalhes, editar, remover.

- **Tela de Detalhes da Meta:**
    - Exibe informações completas da meta.
    - Lista de aportes realizados (data, valor).
    - Campo para registrar novo aporte.
    - Indicador visual de progresso (barra ou círculo).
    - Botão para editar/remover meta.

- **Tela de Criação/Edição de Meta:**
    - Formulário para inserir/editar título, valor objetivo, data limite, descrição.
    - Validação de campos obrigatórios.

- **Feedback Visual:**
    - Mensagens de sucesso, erro e conclusão de meta.
    - Indicadores de progresso e status.

# Projeto de Interface — R10 Dashboard Personalizado

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de customização do dashboard financeiro, incluindo adição, remoção, configuração e reordenação (drag and drop) de widgets.

```mermaid
graph TD
    A([Usuário acessa o Dashboard]) --> B[Visualiza painel com widgets]
    B --> C{Deseja adicionar widget?}
    C -- Sim --> D[Seleciona tipo e configura widget]
    D --> E[Widget adicionado ao painel]
    C -- Não --> F{Deseja remover widget?}
    F -- Sim --> G[Remove widget do painel]
    F -- Não --> H{Deseja reordenar widgets?}
    H -- Sim --> I[Arrasta e solta widget na nova posição]
    I --> J[Ordem dos widgets atualizada]
    H -- Não --> K{Deseja editar configuração de um widget?}
    K -- Sim --> L[Edita e salva configuração do widget]
    L --> M[Widget atualizado]
    K -- Não --> N[Fim da customização]
    E --> B
    G --> B
    J --> B
    M --> B
    N --> O([FIM])
```

# Projeto de Interface — R11 Avisos e Notificações

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de exibição e interação com avisos e notificações, incluindo visualização, marcação como lida e atualização de indicadores visuais.

```mermaid
graph TD
    A([Usuário acessa o sistema]) --> B[Visualiza painel de notificações]
    B --> C{Há novas notificações?}
    C -- Sim --> D[Exibe alerta visual e mensagem]
    D --> E[Usuário lê notificação]
    E --> F[Marca notificação como lida]
    F --> G[Indicador visual atualizado]
    C -- Não --> H{Deseja ver histórico?}
    H -- Sim --> I[Exibe histórico de notificações]
    H -- Não --> J[Fim da interação]
    G --> B
    I --> B
    J --> K([FIM])
```

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

# Projeto de Interface — R16 Histórico Financeiro

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de visualização do histórico financeiro, desde a seleção do período até a exibição dos saldos e movimentações.

```mermaid
graph TD
    A([Usuário acessa Histórico Financeiro]) --> B[Seleciona período de consulta]
    B --> C[Buscar movimentações e saldos]
    C --> D{Movimentações encontradas?}
    D -- Sim --> E[Exibir lista de movimentações e gráfico de saldo]
    D -- Não --> F[Exibir mensagem: Nenhuma movimentação encontrada]
    E --> G[Permitir exportação do relatório]
    G --> H([FIM])
    F --> H
```

## 2. Protótipos de Telas

- Tela de seleção de período
- Lista de movimentações (com filtros por categoria, tipo, valor)
- Gráfico de evolução do saldo
- Botão para exportar relatório (PDF/Excel)

## 3. Requisitos de Interface

- Interface clara e intuitiva
- Gráficos responsivos
- Filtros avançados para busca de movimentações
- Opção de exportação de dados

