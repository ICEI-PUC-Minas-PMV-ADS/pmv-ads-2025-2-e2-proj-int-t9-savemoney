# Metodologia

A condução do projeto adota o framework ágil **Scrum**, conforme descrito no "Guia do Scrum" (2020). O desenvolvimento será organizado em **Sprints**, que são ciclos de trabalho com duração fixa de **uma semana**, com o objetivo de entregar um incremento de valor ao produto ao final de cada ciclo.

Para garantir o alinhamento e a melhoria contínua, os seguintes rituais do Scrum serão praticados:

* **Sprint Planning (Planejamento da Sprint)**: Reunião realizada no início de cada Sprint para selecionar os itens do *Product Backlog* que serão desenvolvidos. O resultado é a criação do *Sprint Backlog*, que representa o plano de trabalho para o ciclo.
* **Daily Scrum (Reunião Diária)**: Encontros rápidos diários para sincronizar o trabalho da equipe, discutir o progresso e identificar quaisquer impedimentos.
* **Sprint Review (Revisão da Sprint)**: Ao final da Sprint, a equipe apresenta o trabalho concluído às partes interessadas (stakeholders) para coletar feedback e validar as entregas.
* **Sprint Retrospective (Retrospectiva da Sprint)**: Reunião interna da equipe para refletir sobre o processo de trabalho da última Sprint, identificando pontos positivos e áreas para melhoria no próximo ciclo.

## Controle de Versão

A ferramenta de controle de versão adotada é o **[Git](https://git-scm.com/)**, com o repositório central hospedado na plataforma **[Github](https://github.com)**.

O projeto segue um modelo de branches simplificado para organizar o fluxo de desenvolvimento:

* `main`: Contém a versão estável e de produção do software. É a branch principal e protegida.
* `dev`: Branch de desenvolvimento que serve como base para a criação de novas funcionalidades e correções.
* **Branches de Funcionalidade/Tarefa**: Ramificações temporárias criadas a partir da `dev` para trabalhar em uma *issue* específica (ex: `apresentacaoEtapa1`, `feature/R8-tema`).

Para a organização das *issues*, o projeto adota as seguintes etiquetas padrão:

* `documentation`: Para tarefas relacionadas à melhoria ou criação de documentação.
* `bug`: Identifica um comportamento inesperado ou erro em uma funcionalidade.
* `enhancement`: Refere-se a uma melhoria em uma funcionalidade já existente.
* `feature`: Designa a implementação de uma nova funcionalidade.

### Gerenciamento de Issues e Pull Requests

O projeto utiliza o sistema de Issues do GitHub para rastrear tarefas, bugs e melhorias. As issues podem ser criadas através do link: [https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/issues](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/issues)

Para contribuições de código, utilizamos Pull Requests, que podem ser criados através de: [https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/pulls](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/pulls)

### Comunicação e Discussões

O projeto mantém um fórum de discussões no GitHub para debates sobre funcionalidades, arquitetura e decisões técnicas: [https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/discussions](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/discussions)

### Discussão sobre o Processo de Versionamento

O fluxo de trabalho da equipe segue um padrão claro para garantir a organização e a qualidade do código.

* ***Commits:*** Adotamos um padrão de commits convencionais para manter o histórico claro e legível. Cada commit segue o formato `<tipo>: <descrição>`, como `fix: corrige link para video` ou `feat: adiciona tela de login`, o que facilita a automação e o entendimento das mudanças.
* ***Branches:*** Para cada nova `feature` ou `bug` vinda de uma *issue*, uma nova branch é criada a partir da `dev` (ex: `feature/R8-personalizacao-tema`).
* ***Merges e Pull Requests (PRs):*** Após a conclusão do desenvolvimento na branch de funcionalidade, um Pull Request é aberto para mesclar o código de volta na branch `dev`. É necessária a revisão e aprovação de pelo menos um outro membro da equipe para que o merge seja autorizado e realizado. Periodicamente, a branch `dev`, contendo um conjunto estável de funcionalidades testadas, é mesclada na `main` para uma nova release.
* ***Tags:*** Versões estáveis na branch `main` são marcadas com tags semânticas (ex: `v1.0.0`, `v1.1.0`) para facilitar o controle de releases.

## Gerenciamento de Projeto

### Divisão de Papéis

A equipe está organizada da seguinte forma, com base nos papéis do Scrum:

* **Product Owner (PO):**
    * Lucas Ferreira Lima - Responsável por gerenciar o Product Backlog, definir as prioridades e garantir que o desenvolvimento agregue valor ao negócio.
* **Scrum Master:**
    * Maicon Theodoro - Responsável por garantir que a equipe siga os processos do Scrum, remover impedimentos e facilitar os rituais.
* **Development Team (Equipe de Desenvolvimento):**
    * Rafael Matos
    * Jean Patricio
    * Samuel Alves
    * Matheus Carlos
    - Responsáveis por desenvolver, testar e entregar os incrementos do produto ao final de cada Sprint.

### Processo

A gestão das tarefas e do fluxo de trabalho é realizada através do **GitHub Projects**, que funciona como um quadro Kanban virtual, proporcionando visibilidade sobre o andamento do projeto.

#### Quadro Kanban

O projeto utiliza um quadro Kanban no GitHub Projects para visualizar e gerenciar o fluxo de trabalho, acessível em: [https://github.com/orgs/ICEI-PUC-Minas-PMV-ADS/projects/2161](https://github.com/orgs/ICEI-PUC-Minas-PMV-ADS/projects/2161)

O quadro está organizado nas seguintes colunas:
- **Backlog**: Tarefas planejadas para futuras sprints.
- **Prontas para Iniciar**: Tarefas selecionadas para a sprint atual.
- **Em desenvolvimento**: Tarefas em desenvolvimento ativo.
- **Em revisão**: Tarefas em revisão de código.
- **Concluídas**: Tarefas finalizadas e aprovadas.

### Ferramentas

| Categoria                   | Ferramenta                                      | Justificativa                                                                                                                         |
| :-------------------------- | :---------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------------ |
| **Controle de Versão** | Git & GitHub                                    | Padrão da indústria, integração nativa com o gerenciamento de projeto e excelente suporte para colaboração e revisão de código.        |
| **Gerenciamento de Projeto**| GitHub Projects, Issues, Discussions, PRs       | Ecossistema integrado que centraliza o planejamento, execução, comunicação e controle de qualidade do projeto no mesmo local do código. |
| **Comunicação da Equipe** | Microsoft Teams                                 | Ferramenta oficial para comunicação síncrona e assíncrona, facilitando a troca rápida de informações e a organização de reuniões.   |
| **Desenvolvimento** | Visual Studio Code                              | Editor de código leve, extensível e com forte suporte para as tecnologias do projeto, sendo o padrão utilizado pela equipe.          |
| **Design de Interface** | Figma                                           | Ferramenta colaborativa líder de mercado para a criação de diagramas de fluxo, wireframes e protótipos interativos, essencial para a UX. |