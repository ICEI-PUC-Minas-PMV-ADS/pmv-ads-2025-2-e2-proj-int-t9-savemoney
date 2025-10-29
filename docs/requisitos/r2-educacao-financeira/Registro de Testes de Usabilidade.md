# Registro de Testes de Usabilidade — R2 Educação Financeira

O registro de testes de usabilidade é um documento onde são coletadas e organizadas as informações sobre a experiência dos usuários ao interagir com a funcionalidade de Educação Financeira. Ele inclui dados como tempo de execução de tarefas, taxa de sucesso, dificuldades encontradas e _feedback_ dos usuários para fornecer _insights_ quantitativos e qualitativos para otimizar a experiência do usuário.

## Perfil dos usuários que participaram do teste

Para garantir uma cobertura adequada do nosso público-alvo, os testes foram realizados com usuários que se encaixam em perfis variados de conhecimento tecnológico e necessidades.

- **Usuário 1 (Isadora Patrocinio):** Trabalha no computador todo dia, conhecimento básico de informática, usa para trabalho e para estudos.
- **Usuário 2 (Valéria Delfino):** Trabalha no computador diariamente, conhecimento básico em computadores, uso para trabalho e seu microempreendimento.
- **Usuário 3 (Giulia Faraj):** 28 anos, advogada, trabalha no computador diariamente, conhecimento intermediário em computadores, uso para trabalho, estudos e lazer. Fluente em inglês.

## Registro dos Testes de Usabilidade

### Cenário 1: Descoberta de Artigos (Gestão Financeira)
A tarefa dada ao usuário foi: **"Encontre artigos que te ajudem a entender sobre gestão financeira e organização financeira para aplicar em sua vida e rotina."**

| **Usuário** | **Tempo Total (seg)** | **Quantidade de cliques** | **Tarefa foi concluída?** (Sim/Não) | **Erros Cometidos** (Onde clicou errado, etc.) | **Feedback Qualitativo do Usuário** |
| :--- | :--- | :--- | :--- | :--- | :--- |
| Usuário 1 (Isadora) | **60 segundos** | **3** | **Sim** | **Não** | **"A interface e usabilidade a agradou, mesmo com a dificuldade de ser em inglês, conseguiu se adequar bem à interface."** |
| Usuário 2 (Valéria) | **45 segundos** | **3** | **Não** | **Não** | **"Organizado e fácil compreensão, porém, como está em inglês, não conseguiu entender nada do conteúdo e não finalizou."** |
| Usuário 3 (Giulia) | **30 segundos** | **5** | **Sim** | **Não** | **"A busca não funcionou, dificultando a procura pelo o que precisava com exatidão, porém conseguiu achar o que desejava nos cards."** |

### Cenário 2: Descoberta de Notícias (Cotidiano)
A tarefa dada ao usuário foi: **"Busque notícias que a auxiliem de forma recente e atualizada em sua vida e cotidiano."**

| **Usuário** | **Tempo Total (seg)** | **Quantidade de cliques** | **Tarefa foi concluída?** (Sim/Não) | **Erros Cometidos** (Onde clicou errado, etc.) | **Feedback Qualitativo do Usuário** |
| :--- | :--- | :--- | :--- | :--- | :--- |
| Usuário 1 (Isadora) | **47 segundos** | **2** | **Sim** | **Não** | **"A interface a agradou e achou fácil o uso."** |
| Usuário 2 (Valéria) | **35 segundos** | **3** | **Sim** | **Não** | **"Tudo a agradou."** (Nota: Provavelmente focada na usabilidade, já que o idioma foi uma barreira no teste anterior) |
| Usuário 3 (Giulia) | **70 segundos** | **13** | **Sim** | **Sim** (cliques desnecessários) | **"Filtros e buscas não funcionaram, dificultando e alongando o tempo de uso para conseguir finalizar sua tarefa."** |

---

## Relatório dos testes de usabilidade

**[RELATÓRIO PREENCHIDO COM BASE NOS DADOS COLETADOS]**

### Análise Quantitativa
- **Taxa de sucesso (Cenário 1 - Artigos):** **66,7%** (2 de 3 usuários concluíram a tarefa). A falha foi de uma usuária (Valéria) devido à barreira de idioma.
- **Taxa de sucesso (Cenário 2 - Notícias):** **100%** (3 de 3 usuários concluíram a tarefa).
- **Tempo médio para completar Cenário 1:** **45 segundos** ((60+45+30)/3).
- **Tempo médio para completar Cenário 2:** **50,7 segundos** ((47+35+70)/3). O tempo foi inflado pela falha na busca (Usuária 3).
- **Número médio de erros cometidos:** **0** (erros de clique). No entanto, a Usuária 3 teve um número excessivo de cliques (13) no Cenário 2, classificado como "erros de fluxo" devido à falha da busca.

### Análise Qualitativa e Padrões Identificados
A partir do feedback dos usuários e da observação, foram identificados os seguintes padrões:

- **Principais dificuldades (Críticas):**
    1.  **Barreira de Idioma (Bloqueador):** O problema mais grave. O conteúdo estar em inglês tornou a funcionalidade inutilizável para a Usuária 2 (Valéria), que não pôde concluir a tarefa 1. A Usuária 1 (Isadora) também relatou dificuldade, mas conseguiu avançar.
    2.  **Busca e Filtros Quebrados:** A usuária avançada (Giulia) reportou em *ambos* os testes que as funcionalidades de busca e filtro não funcionaram. Isso a forçou a usar os cards (no Cenário 1) e aumentou drasticamente seu tempo e cliques (70s, 13 cliques) no Cenário 2.

- **Pontos positivos:**
    1.  **Interface e Usabilidade:** A interface foi universalmente elogiada como "agradável", "organizada" e "fácil de usar", mesmo por usuárias com conhecimento básico.
    2.  **Descoberta por Cards:** Mesmo com a busca quebrada, a navegação visual pelos cards foi eficaz o suficiente para permitir que a Usuária 3 (Giulia) encontrasse o que precisava no Cenário 1.

- **Sugestões dos usuários (Implícitas):**
    1.  O conteúdo precisa estar em português.
    2.  A funcionalidade de busca precisa ser corrigida.

### Problemas Identificados e Priorização

1.  **Crítico (Bloqueador):** **Conteúdo em Inglês.** O produto não atende aos perfis de usuário (Isadora, Valéria) que não são fluentes, resultando em falha direta na conclusão da tarefa (Valéria, Cenário 1).
2.  **Crítico (Funcional):** **Busca e Filtros inoperantes.** Uma funcionalidade essencial de descoberta está quebrada, frustrando a usuária avançada (Giulia) e inflando o tempo de tarefa (Cenário 2).
3.  **Leve:** **Ausência de Feedback da Busca.** A busca não funcional não parece ter retornado um erro claro, levando a usuária (Giulia) a clicar mais vezes (13) para tentar fazer funcionar.

### Plano de Ação e Melhorias Propostas
Com base na análise, a equipe propõe as seguintes ações a serem priorizadas em futuras Sprints:

- **Ação 1 (Melhoria - Prioridade CRÍTICA):** **Localização de Conteúdo.** Garantir que os artigos e notícias exibidos estejam em Português-BR para atender ao público-alvo principal.
- **Ação 2 (Correção - Prioridade CRÍTICA):** **Reparo da Funcionalidade de Busca e Filtro.** Investigar e corrigir o bug que impede o funcionamento da busca e dos filtros.
- **Ação 3 (Melhoria - Prioridade Baixa):** **Feedback de Erro na Busca.** Implementar uma mensagem clara (ex: "Nenhum resultado encontrado") caso a busca não retorne resultados, para evitar cliques repetitivos.