
# Metodologia


A metodologia para condução do projeto será <em>Scrum</em>(O Guia do Scrum, 2020), seguindo seus rituais de <em>Periódicosy</em>, <em>Planning</em>, <em>Retrospective</em> e <em>Review</em>. Desenvolvimento das tarefas terão tempo hábil de uma semana no formato de <em>sprint</em>, criando artefatos em seus rituais: <em>Backlog</em> do produto e <em>backlog</em> da <em>sprint</em>

- **Periodic meeting**: Reuniões periódicas com objetivo de acompanhamento das tarefas e verificar impedimentos no desenvolvimento
- **Planning**: Reunião ao iniciar <em>sprints</em>, com objetivo de alinhamento da equipe e criação do artefato: <em>Backlog</em> do produto
- **Retrospective**: Reunião ao terminar ciclos de <em>sprints</em>, com objetivo de avaliar o processo conduzido durante o desenvolvimento e sugestão de melhorias. Criação do artefato <em>Backlog</em> da <em>sprint</em>
- **Review**: Reunião com objetivo de demonstrar as tarefas desenvolvidas, coleta e validação de <em>feedbacks</em>

## Controle de Versão

A ferramenta de controle de versão adotada no projeto foi o
[Git](https://git-scm.com/), sendo que o [Github](https://github.com)
foi utilizado para hospedagem do repositório.

O projeto segue a seguinte convenção para o nome de branches:

- `main`: versão estável já testada do software
- `unstable`: versão já testada do software, porém instável
- `testing`: versão em testes do software
- `dev`: versão de desenvolvimento do software

Quanto à gerência de issues, o projeto adota a seguinte convenção para
etiquetas:

- `documentation`: melhorias ou acréscimos à documentação
- `bug`: uma funcionalidade encontra-se com problemas
- `enhancement`: uma funcionalidade precisa ser melhorada
- `feature`: uma nova funcionalidade precisa ser introduzida

### Gerenciamento de Issues e Pull Requests

O projeto utiliza o sistema de Issues do GitHub para rastrear tarefas, bugs e melhorias. As issues podem ser criadas através do link: [https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/issues](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/issues)

Para contribuições de código, utilizamos Pull Requests que podem ser criados através de: [https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/pulls](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/pulls)

### Comunicação e Discussões

O projeto mantém um fórum de discussões no GitHub para debates sobre funcionalidades, arquitetura e decisões técnicas: [https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/discussions](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e2-proj-int-t9-savemoney/discussions)

Discuta como a configuração do projeto foi feita na ferramenta de versionamento escolhida. Exponha como a gerência de tags, merges, commits e branchs é realizada. Discuta como a gerência de issues foi realizada.

> **Links Úteis**:
> - [Tutorial GitHub](https://guides.github.com/activities/hello-world/)
> - [Git e Github](https://www.youtube.com/playlist?list=PLHz_AreHm4dm7ZULPAmadvNhH6vk9oNZA)
>  - [Comparando fluxos de trabalho](https://www.atlassian.com/br/git/tutorials/comparing-workflows)
> - [Understanding the GitHub flow](https://guides.github.com/introduction/flow/)
> - [The gitflow workflow - in less than 5 mins](https://www.youtube.com/watch?v=1SXpE08hvGs)

## Gerenciamento de Projeto

### Divisão de Papéis

Apresente a divisão de papéis entre os membros do grupo.

> **Links Úteis**:
> - [11 Passos Essenciais para Implantar Scrum no seu 
> Projeto](https://mindmaster.com.br/scrum-11-passos/)
> - [Scrum em 9 minutos](https://www.youtube.com/watch?v=XfvQWnRgxG0)

### Processo

O grupo implementa uma metodologia ágil baseada no Scrum para o desenvolvimento do projeto. Utilizamos o sistema de projetos do GitHub para gerenciar o fluxo de trabalho através de um quadro Kanban.

#### Quadro Kanban

O projeto utiliza um quadro Kanban no GitHub Projects para visualizar e gerenciar o fluxo de trabalho: [https://github.com/orgs/ICEI-PUC-Minas-PMV-ADS/projects/2161](https://github.com/orgs/ICEI-PUC-Minas-PMV-ADS/projects/2161)

O quadro está organizado nas seguintes colunas:
- **Backlog**: Tarefas planejadas para futuras sprints
- **To Do**: Tarefas selecionadas para a sprint atual
- **In Progress**: Tarefas em desenvolvimento
- **Review**: Tarefas em revisão de código
- **Done**: Tarefas concluídas

#### Fluxo de Trabalho

1. **Planejamento**: As tarefas são criadas como Issues no GitHub
2. **Seleção**: As Issues são movidas para o quadro Kanban na coluna "To Do"
3. **Desenvolvimento**: Durante o desenvolvimento, as Issues são movidas para "In Progress"
4. **Revisão**: Após o desenvolvimento, as Issues são movidas para "Review"
5. **Conclusão**: Tarefas aprovadas são movidas para "Done"

O grupo poderá fazer uso de ferramentas on-line para acompanhar o andamento do projeto, a execução das tarefas e o status de desenvolvimento da solução.
 
> **Links Úteis**:
> - [Project management, made simple](https://github.com/features/project-management/)
> - [Sobre quadros de projeto](https://docs.github.com/pt/github/managing-your-work-on-github/about-project-boards)
> - [Como criar Backlogs no Github](https://www.youtube.com/watch?v=RXEy6CFu9Hk)
> - [Tutorial Slack](https://slack.com/intl/en-br/)

### Ferramentas

As ferramentas empregadas no projeto são:

#### Controle de Versão e Colaboração
- **GitHub**: Plataforma principal para controle de versão, gerenciamento de projetos e colaboração
- **Git**: Sistema de controle de versão distribuído para gerenciar o código fonte

#### Gerenciamento de Projeto
- **GitHub Projects**: Quadro Kanban para visualização do fluxo de trabalho e gerenciamento de tarefas
- **GitHub Issues**: Sistema de rastreamento de tarefas, bugs e melhorias
- **GitHub Discussions**: Fórum para discussões técnicas e decisões do projeto
- **GitHub Pull Requests**: Sistema de revisão e merge de código

#### Desenvolvimento
- **Editor de código**: Para desenvolvimento e edição do código fonte
- **Ferramentas de desenho de tela (_wireframing_)**: Para criação de protótipos e diagramas de interface

#### Justificativa das Escolhas

- **GitHub**: Escolhido por sua integração completa entre controle de versão, gerenciamento de projetos e colaboração
- **Git**: Ferramenta padrão da indústria para controle de versão, com excelente integração com GitHub
- **GitHub Projects**: Permite visualização clara do fluxo de trabalho e facilita o acompanhamento do progresso
- **GitHub Issues**: Sistema robusto para rastreamento de tarefas com integração nativa com o repositório
- **GitHub Discussions**: Facilita a comunicação da equipe e documentação de decisões técnicas
- **GitHub Pull Requests**: Sistema eficiente para revisão de código e controle de qualidade
