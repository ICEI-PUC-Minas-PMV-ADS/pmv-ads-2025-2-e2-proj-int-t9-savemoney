# Projeto de Interface

## Principais Telas e Funcionalidades

1. **Tela Inicial Pública / Sobre Nós**
  - Apresenta a missão do sistema, opções de login, cadastro, funcionalidades e newsletters

2. **Tela de Login**
  - Permite acessar o sistema informando e-mail e senha

3. **Tela de Cadastro**
  - Permite criar uma conta, selecionando o tipo de pessoa e preenchendo os dados obrigatórios

4. **Dashboard Principal**
  - Exibe navegação para todas as principais funcionalidades do sistema

5. **Receitas**
  - Visualizar, cadastrar, editar e excluir receitas, com informações detalhadas e filtros

6. **Despesas**
  - Visualizar, cadastrar, editar e excluir despesas, com informações detalhadas e filtros

7. **Investimentos**
  - Gerenciar investimentos, exibir status, rentabilidade e histórico

8. **Relatórios**
  - Exibe gráficos e resumos financeiros para análise do usuário

9. **Perfil do Usuário**
  - Visualizar e editar informações pessoais, contato e configurações da conta

10. **Contato**
  - Entrar em contato com a equipe de suporte por meio de formulário

11. **Configurações**
  - Editar dados pessoais, contato, endereço e personalizar o tema

12. **Conversor de Moedas**
  - Ferramenta para conversão de moedas, com histórico e dicas

13. **Conversor de Energia**
  - Ferramenta para simulação e cálculo de consumo de energia

14. **Dicas de Energia**
  - Exibe dicas personalizadas relacionadas ao consumo de energia


## Wireframes

Abaixo estão os wireframes que ilustram o template padrão da aplicação, com seus respectivos títulos e descrições.

### Tela Inicial Pública / Sobre Nós

Apresenta a missão do sistema, opções de login, cadastro, funcionalidades e newsletters.

![Tela Sobre Nós](img/wireframes/home-publica.png)

---

### Tela Dashboard

Exibe navegação para todas as principais funcionalidades do sistema.

![Tela Dashboard](img/wireframes/dashboard.png)

---

### Tela de Login

Permite acessar o sistema informando e-mail e senha.

![Tela de Login](img/wireframes/login.png)

---

### Tela de Cadastro

Permite criar uma conta, selecionando o tipo de pessoa e preenchendo os dados obrigatórios.

![Tela de Cadastro](img/wireframes/registrar.png)

---

### Tela de Receitas

Permite visualizar, cadastrar, editar e excluir receitas, com informações detalhadas e filtros.

![Tela de Receitas](img/wireframes/receitas.png)

---

### Tela de Despesas

Permite visualizar, cadastrar, editar e excluir despesas, com informações detalhadas e filtros.

![Tela de Despesas](img/wireframes/despesas.png)

---

### Tela de Investimentos

Permite gerenciar investimentos, exibir status, rentabilidade e histórico.

![Tela de Investimentos](img/wireframes/investimentos.png)

---

### Tela de Relatórios

Exibe gráficos e resumos financeiros para análise do usuário.

![Tela de Relatórios](img/wireframes/relatorios.png)

---

### Tela de Perfil do Usuário

Permite visualizar e editar informações pessoais, contato e configurações da conta.

![Tela de Perfil do Usuário](img/wireframes/perfil.png)

---

### Tela de Contato

Permite entrar em contato com a equipe de suporte por meio de formulário.

![Tela de Contato](img/wireframes/contato.png)

---

### Tela de Configurações

Permite editar dados pessoais, contato, endereço e personalizar o tema.

![Tela de Configurações](img/wireframes/configuracao.png)

---

### Tela Conversor de Moedas

Ferramenta para conversão de moedas, com histórico e dicas.

![Tela Conversor de Moedas](img/wireframes/conversor-moedas.png)

---

### Tela Conversor de Energia

Ferramenta para simulação e cálculo de consumo de energia.

![Tela Conversor de Energia](img/wireframes/conversor-energia.png)

---

### Tela Dicas de Energia

Exibe dicas personalizadas relacionadas ao consumo de energia.

![Tela Dicas de Energia](img/wireframes/conversor-energia-dicas.png)

---

# Projeto de Interface — R1 Controle Financeiro

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de registro de receitas e despesas, desde a entrada de dados do usuário até a confirmação do registro e atualização do saldo.

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

## 2. Protótipos de Telas

- Tela de Cadastro de Receita/Despesa: Campos para valor, categoria, tipo, data e descrição, botão de salvar.
- Tela de Listagem: Exibe histórico de registros financeiros, filtros por período, categoria e tipo.
- Tela de Saldo: Mostra saldo atual, total de receitas e despesas.

## 3. Navegação

- O usuário pode acessar o cadastro a partir do menu principal.
- Após o registro, retorna à tela de listagem com atualização automática.
- Opção de editar ou remover registros existentes.

# Projeto de Interface — R2 Educação Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para acessar, interagir (curtir) e visualizar o conteúdo educativo, detalhando a comunicação com o backend para buscar artigos agregados e gerenciar os favoritos do usuário.

```mermaid
graph TD
    A([Usuário clica em 'Educação Financeira']) --> B[Frontend solicita lista de conteúdos via API];
    B --> C[Backend busca conteúdo de fontes externas];
    C --> D[Backend retorna a lista para o Frontend];
    D --> E[Frontend exibe a lista de conteúdos];
    E --> F[Usuário clica em um conteúdo para ler];
    F --> G([App abre o link original do conteúdo em um navegador/WebView]);
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

# Projeto de Interface — R6/R7 Pessoa Física/Jurídica

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de cadastro e uso do sistema por usuários Pessoa Física e Pessoa Jurídica, desde a entrada dos dados até a diferenciação de funcionalidades.

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

## 2. Protótipos de Telas

- Tela de Cadastro: Seleção de tipo (PF/PJ), campos dinâmicos para CPF ou CNPJ, nome ou razão social, e-mail, senha.
- Tela de Login: E-mail e senha.
- Tela de Perfil: Exibe dados do usuário, tipo de conta, opção de editar informações.
- Painel Pessoa Física: Funcionalidades voltadas para controle financeiro pessoal, metas, histórico.
- Painel Pessoa Jurídica: Funcionalidades para gestão financeira empresarial, relatórios, fluxo de caixa.

## 3. Navegação

- O usuário pode alternar entre cadastro e login.
- Após cadastro/login, é direcionado ao painel correspondente ao tipo de usuário.
- Opção de editar perfil e trocar tipo de conta (se permitido).
- Funcionalidades e menus adaptados conforme PF ou PJ.

# Projeto de Interface — R8 Personalização do Tema

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa o fluxo de interação do usuário para personalizar e salvar o tema, detalhando a comunicação com o backend e a persistência.

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

# Projeto de Interface — R12 Gestão de Orçamento

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de gestão de orçamentos, desde a solicitação do usuário até a criação, consulta e ajuste de orçamentos mensais por categoria.

```mermaid
graph TD
    A([Usuário acessa Gestão de Orçamento]) --> B[Selecionar categoria e período (mês/ano)]
    B --> C{Deseja criar, editar ou consultar?}
    C -- Criar --> D[Informar valor limite e confirmar]
    D --> E[Validar dados de entrada]
    E -- Válido --> F[Salvar orçamento no banco de dados]
    E -- Inválido --> G[Exibir mensagem de erro e solicitar correção]
    F --> H[Exibir confirmação de criação]
    G --> B

    C -- Editar --> I[Selecionar orçamento existente]
    I --> J[Alterar valor limite e confirmar]
    J --> E

    C -- Consultar --> K[Listar orçamentos do período]
    K --> L[Exibir detalhes: categoria, limite, gasto, saldo]
    L --> M{Deseja ajustar orçamento?}
    M -- Sim --> I
    M -- Não --> N([FIM])
    H --> N
```

# Projeto de Interface — R13 Análise de Tendências

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de análise de tendências financeiras, desde a solicitação do usuário até a exibição dos resultados e recomendações.

```mermaid
graph TD
    A([Usuário acessa Análise de Tendências]) --> B[Selecionar categoria, período e filtros]
    B --> C{"Dados suficientes para análise?"}
    C -- Sim --> D[Chamar ServicoAnaliseTendencia para gerar análise]
    C -- Não --> E[Exibir mensagem: Dados insuficientes]
    E --> B
    D --> F{"Análise gerada com sucesso?"}
    F -- Sim --> G[Exibir tendência, gráfico e recomendações]
    F -- Não --> H[Exibir mensagem de erro: Falha na análise]
    G --> I{Deseja exportar ou salvar?}
    I -- Sim --> J[Exportar/Salvar análise]
    I -- Não --> K([FIM])
    J --> K
```

### 1.2 Telas Principais

#### Tela: Dashboard de Tendências

- **Componentes:**
  - Filtros de período (ex: últimos 3, 6, 12 meses)
  - Filtro de categoria (dropdown)
  - Botão "Analisar"
  - Lista de tendências por categoria (exibe tipo da tendência: Alta, Baixa, Estável)
  - Gráfico de linha com evolução dos gastos no período selecionado
  - Recomendações e descrição textual da análise
  - Botão "Exportar" (PDF/CSV)

#### Tela: Detalhe da Tendência

- **Componentes:**
  - Nome da categoria analisada
  - Gráfico detalhado dos valores mensais
  - Texto explicativo da tendência identificada
  - Recomendações personalizadas
  - Botão "Voltar" para o dashboard

### 1.3 Comportamento Esperado

- Ao acessar a Análise de Tendências, o usuário pode selecionar uma categoria e um período.
- O sistema exibe o resultado da análise, incluindo:
  - Tipo de tendência (Alta, Baixa, Estável)
  - Gráfico de evolução dos gastos
  - Recomendações e explicações em linguagem simples
- Caso não haja dados suficientes, uma mensagem informativa é exibida.
- O usuário pode exportar o resultado da análise.

## 2. Observações

- A interface deve ser responsiva e acessível.
- Os gráficos devem ser claros, com legendas e destaques para mudanças significativas.
- Recomendações devem ser objetivas e contextualizadas conforme o padrão identificado.

# Projeto de Interface — R14 Projeção Financeira

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de Projeção Financeira, desde a solicitação do usuário até a exibição da projeção e exportação dos resultados.

```mermaid
graph TD
    A([Usuário acessa Projeção Financeira]) --> B[Seleciona o período para a projeção (ex: 6, 12 meses)]
    B --> C{"Existem dados históricos suficientes?"}
    C -- Sim --> D[Backend calcula a projeção]
    C -- Não --> E[Exibir mensagem: Dados insuficientes para projetar]
    E --> B
    D --> F{"Projeção gerada com sucesso?"}
    F -- Sim --> G[Exibir projeção: gráfico e resumo]
    F -- Não --> H[Exibir mensagem de erro: Falha ao gerar projeção]
    G --> I{Deseja exportar ou salvar?}
    I -- Sim --> J[Exportar/Salvar projeção]
    I -- Não --> K([FIM])
    J --> K
```

### 1.2 Telas Principais

#### Tela: Projeção Financeira

- **Componentes:**
  - Filtro de período para projeção (ex: dropdown com 3, 6, 12 meses)
  - Botão "Gerar Projeção"
  - Gráfico de linhas mostrando a evolução do saldo projetado mês a mês
  - Resumo textual da projeção (ex: "Seu saldo projetado para 6 meses é R$ X.XXX,XX")
  - Mensagens de erro ou alerta caso não haja dados suficientes
  - Botão "Exportar" (PDF/CSV)

#### Tela: Detalhe da Projeção

- **Componentes:**
  - Gráfico detalhado com valores de saldo projetado por mês
  - Tabela opcional com os valores mês a mês
  - Texto explicativo sobre a metodologia da projeção
  - Botão "Voltar" para a tela principal

### 1.3 Comportamento Esperado

- O usuário seleciona o período desejado e solicita a projeção.
- O sistema exibe o gráfico de saldo projetado e um resumo textual.
- Caso não haja dados suficientes, uma mensagem informativa é exibida.
- O usuário pode exportar a projeção em PDF ou CSV.

## 2. Observações

- A interface deve ser responsiva e acessível.
- Os gráficos devem ser claros, com legendas e destaques para variações relevantes.
- O resumo textual deve ser objetivo e fácil de entender.

# Projeto de Interface — R15 Ferramentas Interativas

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

O diagrama a seguir representa a jornada do usuário para acessar o hub de ferramentas e utilizar as calculadoras. Conforme a decisão de arquitetura, ambas as ferramentas operam inteiramente no frontend (client-side), com os cálculos sendo executados em tempo real a partir dos dados inseridos manualmente pelo usuário.

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

