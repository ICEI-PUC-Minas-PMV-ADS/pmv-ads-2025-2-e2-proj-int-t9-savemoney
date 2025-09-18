# Registro de Testes de Software — R15 Ferramentas Interativas

<span style="color:red">Pré-requisitos: <a href="3-Projeto de Interface.md"> Projeto de Interface</a></span>, <a href="6-Plano de Testes de Software.md"> Plano de Testes de Software</a>

Esta seção documenta os resultados da execução dos casos de teste definidos no Plano de Testes de Software para o requisito R15. As evidências consistem em vídeos do tipo screencast, demonstrando a execução de cada teste e seu resultado.

---

| **Caso de Teste** | **CT-R15-001 – API - Cadastrar novo produto** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A API deve permitir que um usuário PJ autenticado cadastre um novo produto, persistindo os dados no banco. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição POST no Postman/Insomnia, a resposta `201 Created`, e o novo registro na tabela `produtos`)* |

---

| **Caso de Teste** | **CT-R15-002 – API - Listar produtos do usuário** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A API deve retornar apenas os produtos que pertencem ao usuário autenticado. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição GET sendo feita por um usuário dono dos produtos (retornando a lista) e por outro usuário (retornando uma lista vazia))* |

---

| **Caso de Teste** | **CT-R15-003 – API - Editar um produto existente** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A API deve permitir que um usuário edite as informações de um produto que ele cadastrou. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição PUT, a resposta `200 OK`, e a alteração dos dados no banco)* |

---

| **Caso de Teste** | **CT-R15-004 – API - Excluir um produto** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A API deve permitir que um usuário exclua um produto que ele cadastrou. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar a requisição DELETE, a resposta `204 No Content`, e o registro sendo removido do banco)* |

---

| **Caso de Teste** | **CT-R15-005 – API - Segurança (Acesso indevido)** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - Um usuário não deve poder visualizar ou modificar os dados (produtos) de outro usuário. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar um usuário tentando dar PUT/DELETE no produto de outro e recebendo uma resposta `403 Forbidden` ou `404 Not Found`)* |

---

| **Caso de Teste** | **CT-R15-006 – API - Validação de dados** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A API não deve aceitar dados inválidos para o cadastro de produtos (ex: valores negativos). |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar uma requisição POST com dados inválidos e a resposta de erro `400 Bad Request`)* |

---

| **Caso de Teste** | **CT-R15-007 – Frontend - Lógica da Calculadora de Metas** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A lógica de cálculo da ferramenta de metas no frontend deve retornar o valor matematicamente correto. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo pode mostrar a execução da função em um console de navegador ou em um teste unitário, provando que a saída corresponde ao resultado esperado)* |

---

| **Caso de Teste** | **CT-R15-008 – Frontend - Lógica da Calculadora de Ponto de Equilíbrio** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A lógica de cálculo da ferramenta de Ponto de Equilíbrio no frontend deve retornar o valor matematicamente correto. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo pode mostrar a execução da função em um console de navegador ou teste unitário, provando que a saída corresponde ao resultado esperado)* |

---

## Relatório de testes de software

**[PREENCHER ESTA SEÇÃO APÓS A EXECUÇÃO DE TODOS OS TESTES]**

Durante a execução dos 8 casos de teste definidos para o requisito R15 (Ferramentas Interativas), a equipe obteve um total de **[X]** casos aprovados e **[Y]** casos reprovados.

### Pontos Fortes
Os testes da API de gerenciamento de produtos (`CT-R15-001` a `CT-R15-004`) se mostraram extremamente sólidos. A segurança que impede um usuário de acessar os dados de outro (`CT-R15-005`) também funcionou perfeitamente, o que é um ponto crucial para a confiança na aplicação.

### Fragilidades e Falhas Detectadas
A principal fragilidade foi observada durante o teste **`[ID DO TESTE QUE FALHOU, ex: CT-R15-008]`**.

**Descrição da Falha:** **[Descrever o problema em detalhe. Exemplo: A função de cálculo do Ponto de Equilíbrio no frontend não tratava a divisão por zero. Se o usuário inserisse um produto cujo `preco_venda` era igual ao `custo_variavel`, a aplicação quebrava ou retornava 'Infinity'.]**

**Impacto:** **[Descrever o impacto da falha. Exemplo: O impacto é moderado. Embora seja um caso de uso incomum, ele gera uma experiência de usuário ruim e pode fazer com que a ferramenta pareça pouco confiável, travando a tela do usuário sem uma mensagem de erro clara.]**

### Estratégias de Correção e Melhorias
Para corrigir a(s) falha(s) identificada(s), a equipe planeja as seguintes ações:

1.  **Correção da Falha `[ID DA FALHA]`:** **[Descrever a ação específica. Exemplo: Implementar uma validação na função JavaScript da Calculadora de Ponto de Equilíbrio. Antes de dividir, o código irá verificar se `(preco_venda - custo_variavel)` é maior que zero. Caso não seja, exibirá uma mensagem amigável ao usuário, como "O preço de venda deve ser maior que o custo por unidade".]**
2.  **Melhoria Proposta:** **[Descrever uma melhoria a partir dos testes. Exemplo: Para tornar as ferramentas mais intuitivas, propomos adicionar ícones de "ajuda" (`tooltips`) ao lado de cada campo das calculadoras, explicando termos como "Custos Fixos" ou "Ponto de Equilíbrio".]**

Essas ações serão transformadas em novas *issues* no GitHub e priorizadas para a próxima Sprint, garantindo a evolução e a estabilidade da solução.