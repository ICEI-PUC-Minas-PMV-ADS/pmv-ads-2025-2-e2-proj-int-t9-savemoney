# Registro de Testes de Software — R2 Educação Financeira

<span style="color:red">Pré-requisitos: <a href="3-Projeto de Interface.md"> Projeto de Interface</a></span>, <a href="6-Plano de Testes de Software.md"> Plano de Testes de Software</a>

Esta seção documenta os resultados da execução dos casos de teste definidos no Plano de Testes de Software para o requisito R2. As evidências consistem em vídeos do tipo screencast, demonstrando a execução de cada teste e seu resultado.

---

| **Caso de Teste** | **CT-R2-001 – API - Listar conteúdos em tempo real** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A API deve atuar como um proxy, buscando e retornando com sucesso uma lista de conteúdos de uma fonte externa. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a execução da requisição GET em uma ferramenta como Postman/Insomnia e a resposta JSON recebida com a lista de conteúdos)* |

---

| **Caso de Teste** | **CT-R2-002 – API - Tratamento de falha do serviço externo** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - O sistema deve ser resiliente a falhas em serviços externos dos quais depende. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve simular a API de notícias externa estando offline e mostrar o nosso backend retornando um erro controlado, como `503 Service Unavailable`)* |

---

| **Caso de Teste** | **CT-R2-003 – Backend - Lógica da Newsletter** |
| :--- | :--- |
| **Requisito Associado** | **R2** - O serviço de newsletter deve ser capaz de buscar os conteúdos mais recentes da fonte externa e iniciar o processo de envio. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo pode mostrar a execução do serviço e os logs de saída indicando que o processo de envio para N usuários foi iniciado com os 3 conteúdos mais recentes)* |

---

| **Caso de Teste** | **CT-R2-004 – API - Validação do contrato de dados** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - O sistema deve ser robusto a dados malformados vindos de fontes externas. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o sistema tratando corretamente uma resposta da API externa que venha com um campo essencial faltando, sem quebrar a aplicação)* |

---

## Relatório de testes de software

**[PREENCHER ESTA SEÇÃO APÓS A EXECUÇÃO DE TODOS OS TESTES]**

Durante a execução dos 4 casos de teste definidos para a arquitetura *stateless* do requisito R2 (Educação Financeira), a equipe obteve um total de **[X]** casos aprovados e **[Y]** casos reprovados.

### Pontos Fortes
Os testes revelaram que a funcionalidade principal da API, atuando como um proxy para buscar e repassar o conteúdo (`CT-R2-001`), funcionou de forma consistente e rápida. A lógica do serviço de newsletter (`CT-R2-003`) também se mostrou correta na seleção dos conteúdos mais recentes para envio.

### Fragilidades e Falhas Detectadas
A principal falha foi identificada no caso de teste **`[ID DO TESTE QUE FALHOU, ex: CT-R2-002]`**. 

**Descrição da Falha:** **[Descrever o problema em detalhe. Exemplo: Ao simular a indisponibilidade da API de notícias externa, o nosso backend demorou 30 segundos para responder (timeout), em vez de retornar um erro imediatamente. Isso deixou a interface do usuário travada, esperando por uma resposta.]**

**Impacto:** **[Descrever o impacto da falha. Exemplo: O impacto desta falha na experiência do usuário é alto. Em caso de instabilidade do serviço externo, a seção de Educação Financeira do nosso aplicativo ficaria inutilizável e passaria a impressão de que o nosso próprio sistema está com problemas.]**

### Estratégias de Correção e Melhorias
Para corrigir a(s) falha(s) identificada(s), a equipe planeja as seguintes ações:

1.  **Correção da Falha `[ID DA FALHA]`:** **[Descrever a ação específica. Exemplo: Implementar um timeout mais curto (ex: 5 segundos) para a chamada HTTP ao serviço externo. Se a resposta não chegar neste tempo, a nossa API irá imediatamente retornar um erro `503 Service Unavailable`, permitindo que o frontend exiba uma mensagem amigável para o usuário.]**
2.  **Melhoria Proposta:** **[Descrever uma melhoria a partir dos testes. Exemplo: Para melhorar a resiliência, propomos implementar um cache de curta duração (ex: 10 minutos) em memória no backend. Se a API externa falhar, o sistema pode servir os últimos dados buscados com sucesso, garantindo que o usuário quase sempre veja algum conteúdo, mesmo durante instabilidades.]**

Essas ações serão transformadas em novas *issues* no GitHub e priorizadas para a próxima Sprint, garantindo a evolução e a estabilidade da solução.