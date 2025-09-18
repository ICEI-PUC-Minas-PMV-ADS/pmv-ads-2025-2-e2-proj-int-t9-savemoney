# Registro de Testes de Software — R2 Educação Financeira

<span style="color:red">Pré-requisitos: <a href="3-Projeto de Interface.md"> Projeto de Interface</a></span>, <a href="6-Plano de Testes de Software.md"> Plano de Testes de Software</a>

Esta seção documenta os resultados da execução dos casos de teste definidos no Plano de Testes de Software para o requisito R2. As evidências consistem em vídeos do tipo screencast, demonstrando a execução de cada teste e seu resultado.

---

| **Caso de Teste** | **CT-R2-001 – API - Listar conteúdos agregados** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A API deve ser capaz de retornar a lista de conteúdos que foram previamente agregados e armazenados no banco de dados. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a execução da requisição GET em uma ferramenta como Postman/Insomnia e a resposta JSON recebida com a lista de conteúdos)* |

---

| **Caso de Teste** | **CT-R2-002 – API - Curtir um conteúdo** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A API deve permitir que um usuário autenticado curta um conteúdo, criando o relacionamento no banco de dados. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição POST sendo enviada com sucesso e, se possível, a nova entrada na tabela `conteudos_curtidos`)* |

---

| **Caso de Teste** | **CT-R2-003 – API - Listar conteúdos curtidos** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A API deve retornar a lista de conteúdos específicos que foram curtidos por um usuário autenticado. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição GET para o endpoint de curtidos e a resposta JSON contendo apenas os conteúdos curtidos pelo usuário de teste)* |

---

| **Caso de Teste** | **CT-R2-004 – API - Descurtir um conteúdo** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A API deve permitir que um usuário autenticado remova sua curtida de um conteúdo. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição DELETE sendo enviada e o registro correspondente sendo removido da tabela `conteudos_curtidos`)* |

---

| **Caso de Teste** | **CT-R2-005 – API - Validação (Curtir conteúdo inexistente)** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A API deve tratar corretamente tentativas de curtir um conteúdo que não existe, retornando um erro apropriado. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição POST para um ID inválido e a resposta de erro `404 Not Found`)* |

---

| **Caso de Teste** | **CT-R2-006 – API - Segurança de endpoint** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - Os endpoints que modificam dados devem ser protegidos e exigir autenticação. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar uma tentativa de requisição POST sem o token de autenticação e a resposta de erro `401 Unauthorized`)* |

---

| **Caso de Teste** | **CT-R2-007 – Backend - Prevenção de duplicatas no agregador** |
| :--- | :--- |
| **Requisito Associado** | **R2** - O serviço de agregação não deve inserir conteúdos duplicados no banco de dados. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o estado da tabela `conteudos_agregados`, a execução do serviço, e o estado final da tabela, provando que nenhum registro duplicado foi inserido)* |

---

| **Caso de Teste** | **CT-R2-008 – Backend - Lógica da Newsletter** |
| :--- | :--- |
| **Requisito Associado** | **R2** - O serviço de newsletter deve ser capaz de selecionar os conteúdos corretos e iniciar o processo de envio. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo pode mostrar a execução do serviço e os logs de saída indicando que o processo de envio para N usuários foi iniciado com os 3 conteúdos mais recentes)* |

---

## Relatório de testes de software

**[PREENCHER ESTA SEÇÃO APÓS A EXECUÇÃO DE TODOS OS TESTES]**

Durante a execução dos 8 casos de teste definidos para o requisito R2 (Educação Financeira), a equipe obteve um total de **[X]** casos aprovados e **[Y]** casos reprovados.

### Pontos Fortes
Os testes revelaram que a funcionalidade principal da API REST, incluindo a listagem de conteúdos (`CT-R2-001`) e o sistema de curtidas (`CT-R2-002`, `CT-R2-004`), demonstrou-se robusta e funcional, respondendo conforme o esperado. A segurança dos endpoints (`CT-R2-006`) também se mostrou eficaz, prevenindo o acesso não autorizado.

### Fragilidades e Falhas Detectadas
A principal falha foi identificada no caso de teste **`[ID DO TESTE QUE FALHOU, ex: CT-R2-007]`**. 

**Descrição da Falha:** **[Descrever o problema em detalhe. Exemplo: O serviço de agregação de conteúdo falhou ao tentar analisar um site com uma estrutura HTML complexa, resultando em uma exceção não tratada que interrompeu todo o processo de atualização.]**

**Impacto:** **[Descrever o impacto da falha. Exemplo: O impacto desta falha é crítico, pois impede que novos conteúdos sejam adicionados ao sistema, tornando a funcionalidade obsoleta rapidamente e quebrando a proposta de valor de manter o usuário informado.]**

### Estratégias de Correção e Melhorias
Para corrigir a(s) falha(s) identificada(s), a equipe planeja as seguintes ações:

1.  **Correção da Falha `[ID DA FALHA]`:** **[Descrever a ação específica. Exemplo: Implementar um tratamento de exceções (`try-catch`) mais robusto no serviço de agregação. Se a análise de um site falhar, o serviço irá registrar o erro (log) e continuar para o próximo site da lista, em vez de parar completamente.]**
2.  **Melhoria Proposta:** **[Descrever uma melhoria a partir dos testes. Exemplo: Durante os testes, notou-se que a consulta para listar os conteúdos curtidos (`CT-R2-003`) pode se tornar lenta com muitos usuários. Propomos a criação de um índice composto na tabela `conteudos_curtidos` para otimizar esta consulta.]**

Essas ações serão transformadas em novas *issues* no GitHub e priorizadas para a próxima Sprint, garantindo a evolução e a estabilidade da solução.
