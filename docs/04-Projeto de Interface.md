
# Projeto de Interface

> **Pré-requisito:** [Documentação de Especificação](2-Especificação%20do%20Projeto.md)

## Visão Geral

Visão geral da interação do usuário pelas telas do sistema. Esta seção apresenta as principais interfaces da plataforma e os fluxogramas que detalham a jornada do usuário e as interações com cada funcionalidade principal do sistema.

## Principais Telas e Funcionalidades

- **Tela de Login e Cadastro**
    - Formulário de autenticação
    - Processo de registro de novos usuários
    - Recuperação de senha

- **Dashboard Principal**
    - Visão geral das finanças
    - Gráficos de receitas vs despesas
    - Resumo mensal e anual
    - Navegação para funcionalidades principais

- **Gestão de Transações**
    - Formulário para adicionar receitas e despesas
    - Lista de transações com filtros
    - Edição e exclusão de transações
    - Categorização automática e manual

- **Relatórios e Análises**
    - Gráficos interativos
    - Filtros por período e categoria
    - Exportação de dados
    - Comparativos mensais/anuais

- **Configurações e Perfil**
    - Dados pessoais do usuário
    - Preferências da aplicação
    - Categorias personalizadas
    - Configurações de notificações

## Diagrama de Fluxo Geral do Save Money v2

O diagrama a seguir representa a jornada completa do usuário dentro do aplicativo, desde o primeiro contato até a utilização das funcionalidades chave. Ele integra os fluxos das duas personas principais, João (Pessoa Física) e Maria (Pessoa Jurídica), em um único mapa de interação para fornecer uma visão holística do sistema.

Snippet de código

```mermaid
graph TD
    subgraph "1. Onboarding e Autenticacao"
        Start([Início]) --> TelaLogin[Tela de Login];
        TelaLogin --> A{Possui conta?};
        A -- Não --> TelaCadastro[Tela de Cadastro];
        TelaCadastro --> B{Seleciona tipo: PF ou PJ};
        B -- "Pessoa Fisica (Joao)" --> PreenchePF["Preenche dados PF"];
        B -- "Pessoa Juridica (Maria)" --> PreenchePJ["Preenche dados PJ"];
        PreenchePF --> ValidaCadastro{Valida Dados};
        PreenchePJ --> ValidaCadastro;
        ValidaCadastro -- Válido --> CriaConta[Cria conta e loga];
        ValidaCadastro -- Inválido --> TelaCadastro;
        
        A -- Sim --> PreencheLogin[Preenche e-mail e senha];
        PreencheLogin --> ValidaLogin{Autentica Usuário};
        ValidaLogin -- Sucesso --> Dashboard;
        ValidaLogin -- Falha --> TelaLogin;
        CriaConta --> MensagemBoasVindas["Exibe 'Boas-Vindas!'"];
        MensagemBoasVindas --> Dashboard;
    end

    subgraph "2. Hub Principal - Dashboard"
        Dashboard[Dashboard Principal] --> Menu{Usuário escolhe funcionalidade};
    end

    subgraph "3. Fluxos de Tarefas Principais"
        Menu -- "Controle Financeiro" --> AddDespesa[Tela de Nova Despesa];
        AddDespesa --> PreencheDespesa["Preenche valor e data"];
        PreencheDespesa --> Categoria{Seleciona Categoria?};
        Categoria -- Sim --> SalvaDespesa[Salva Despesa];
        Categoria -- Não --> AddDespesa;
        SalvaDespesa --> ConfirmaDespesa[Exibe confirmação];
        ConfirmaDespesa --> Dashboard;

        Menu -- "Relatorios" --> TelaRelatorios[Tela de Relatórios];
        TelaRelatorios --> FiltraRelatorio[Seleciona tipo e período];
        FiltraRelatorio --> GeraRelatorio[Gera e exibe relatório];
        GeraRelatorio --> Exporta{Deseja exportar?};
        Exporta -- Sim --> IniciaDownload[Inicia download];
        Exporta -- Não --> TelaRelatorios;
        IniciaDownload --> TelaRelatorios;

        Menu -- "Metas Financeiras" --> TelaMetas[Tela de Metas];
        Menu -- "Ferramentas Interativas" --> TelaFerramentas[Hub de Ferramentas];
        Menu -- "Educacao Financeira" --> TelaEducacao[Feed de Conteúdos];
        Menu -- "Configuracoes" --> TelaConfig[Tela de Configurações];
    end

    TelaMetas --> End([Fim do Fluxo]);
    TelaFerramentas --> End;
    TelaEducacao --> End;
    TelaConfig --> End;
```
Fluxogramas Detalhados por Funcionalidade

R1 — Controle Financeiro

Este diagrama representa o fluxo de execução para a funcionalidade de registro de receitas e despesas, desde a entrada de dados do usuário até a confirmação do registro e atualização do saldo.

Snippet de código

```mermaid
graph TD
    A([Usuário acessa o Controle Financeiro]) --> B[Informar valor, categoria, tipo: receita ou despesa, data e descrição]
    B --> C{"Dados válidos e completos?"}
    C -- Sim --> D[Registrar no sistema]
    C -- Não --> E[Exibir mensagem de erro: Dados inválidos ou incompletos]
    E --> B
    D --> F[Atualizar saldo e histórico do usuário]
    F --> G[Exibir confirmação e novo saldo]
    G --> H([FIM])
```
Protótipos de Telas

### Protótipos de Telas

- **Tela de Cadastro de Receita/Despesa:**
    - Campos para valor, categoria, tipo, data e descrição
    - Botão de salvar

- **Tela de Listagem:**
    - Exibe histórico de registros financeiros
    - Filtros por período, categoria e tipo

- **Tela de Saldo:**
    - Mostra saldo atual, total de receitas e despesas

#### Navegação
- O usuário pode acessar o cadastro a partir do menu principal.
- Após o registro, retorna à tela de listagem com atualização automática.
- Opção de editar ou remover registros existentes.

O diagrama a seguir representa o fluxo de interação do usuário para acessar e visualizar o conteúdo educativo.

### R2 — Educação Financeira

O diagrama a seguir representa o fluxo de interação do usuário para acessar e visualizar o conteúdo educativo.

Snippet de código

```mermaid
graph TD
    A([Usuário clica em 'Educação Financeira']) --> B[Frontend solicita lista de conteúdos via API];
    B --> C[Backend busca conteúdo de fontes externas];
    C --> D[Backend retorna a lista para o Frontend];
    D --> E[Frontend exibe a lista de conteúdos];
    E --> F[Usuário clica em um conteúdo para ler];
    F --> G([App abre o link original do conteúdo em um navegador/WebView]);
```

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

### R3 — Conversor de Energia

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

Snippet de código

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

Este diagrama representa o fluxo de execução para a funcionalidade de geração de relatórios, desde a solicitação do usuário até a exibição do relatório detalhado.

### R4 — Relatórios, Diagnósticos e Resultados

Este diagrama representa o fluxo de execução para a funcionalidade de geração de relatórios, desde a solicitação do usuário até a exibição do relatório detalhado.

Snippet de código

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
Este diagrama representa o fluxo de cadastro e uso do sistema por usuários Pessoa Física e Pessoa Jurídica.

### R6/R7 — Cadastro Pessoa Física/Jurídica

Este diagrama representa o fluxo de cadastro e uso do sistema por usuários Pessoa Física e Pessoa Jurídica.

Snippet de código

```mermaid
graph TD
    A([Usuário acessa tela de cadastro]) --> B{Seleciona tipo de usuário: PF ou PJ}
    B -- Pessoa Física --> C[Preencher nome, e-mail, senha, CPF]
    B -- Pessoa Jurídica --> D[Preencher razão social, e-mail, senha, CNPJ]
    C --> E[Validar dados e senha]
    D --> E
    E -- Dados válidos --> F[Registrar usuário no sistema]
    E -- Dados inválidos --> G[Exibir mensagem de erro]
    F --> H[Usuário acessa painel conforme tipo]
    G --> B
    H --> I([FIM])
```
Protótipos de Telas

### Protótipos de Telas

- **Tela de Cadastro:**
    - Seleção de tipo (PF/PJ)
    - Campos dinâmicos para CPF ou CNPJ, nome ou razão social, e-mail, senha

- **Tela de Login:**
    - E-mail e senha

- **Tela de Perfil:**
    - Exibe dados do usuário, tipo de conta, opção de editar informações

- **Painel Pessoa Física:**
    - Funcionalidades voltadas para controle financeiro pessoal, metas, histórico

- **Painel Pessoa Jurídica:**
    - Funcionalidades para gestão financeira empresarial, relatórios, fluxo de caixa

#### Navegação
- O usuário pode alternar entre cadastro e login.
- Após cadastro/login, é direcionado ao painel correspondente ao tipo de usuário.
- Opção de editar perfil e trocar tipo de conta (se permitido).
- Funcionalidades e menus adaptados conforme PF ou PJ.

O diagrama a seguir representa o fluxo de interação do usuário para personalizar e salvar o tema.

### R8 — Personalização do Tema

O diagrama a seguir representa o fluxo de interação do usuário para personalizar e salvar o tema.

Snippet de código

```mermaid
graph TD
    A(Usuário acessa as Configurações) --> B{Solicitar preferências do backend};
    B --> C[GET /api/preferences];
    C --> D[Backend consulta Banco de Dados];
    D --> E[Backend retorna preferências salvas ou padrão];
    E --> F{Frontend aplica o tema};
    F --> G[Usuário customiza tema em tempo real];
    G --> H{Salvar?};
    H -- Sim --> I[POST/PUT /api/preferences];
    I --> J[Backend salva/atualiza no Banco de Dados];
    J --> K[Confirmação de sucesso];
    H -- Não --> L(Fim);
    K --> L;
```
Este diagrama representa o fluxo de execução para a funcionalidade de metas financeiras, desde a criação da meta até o acompanhamento e conclusão.

### R9 — Metas Financeiras

Este diagrama representa o fluxo de execução para a funcionalidade de metas financeiras, desde a criação da meta até o acompanhamento e conclusão.

Snippet de código

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
Este diagrama representa o fluxo de customização do dashboard financeiro.

### R10 — Dashboard Personalizado

Este diagrama representa o fluxo de customização do dashboard financeiro.

Snippet de código

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
Este diagrama representa o fluxo de exibição e interação com avisos e notificações.

### R11 — Avisos e Notificações

Este diagrama representa o fluxo de exibição e interação com avisos e notificações.

Snippet de código

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
O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas e utilizar as calculadoras.

### R15 — Ferramentas Interativas

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas e utilizar as calculadoras.

Snippet de código

```mermaid
graph TD
    A([Usuário clica em 'Ferramentas' no menu]) --> B[Abre a tela 'Hub de Ferramentas'];
    B --> C{Usuário escolhe a calculadora};
    
    C -- 'Calculadora de Metas' --> D[Abre a tela da Calculadora de Metas];
    D --> E[Usuário preenche os campos da meta];
    E --> F[Clica em 'Calcular'];
    F --> G([Lógica JavaScript calcula e exibe o resultado]);
    
    C -- 'Ponto de Equilíbrio' --> H[Abre a tela da Calculadora de Ponto de Equilíbrio];
    H --> I[Usuário preenche Custos Fixos, Preço de Venda e Custo Variável];
    I --> J[Clica em 'Calcular'];
    J --> K([Lógica JavaScript calcula e exibe o resultado]);
```
Este diagrama representa o fluxo de visualização do histórico financeiro.

### R16 — Histórico Financeiro

Este diagrama representa o fluxo de visualização do histórico financeiro.

Snippet de código

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