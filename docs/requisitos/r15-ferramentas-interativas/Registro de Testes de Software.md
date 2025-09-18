# Registro de Testes de Software — R15 Ferramentas Interativas

<span style="color:red">Pré-requisitos: <a href="3-Projeto de Interface.md"> Projeto de Interface</a></span>, <a href="6-Plano de Testes de Software.md"> Plano de Testes de Software</a>

Esta seção documenta os resultados da execução dos casos de teste definidos no Plano de Testes de Software para o requisito R15. As evidências consistem em vídeos do tipo screencast, demonstrando a execução de cada teste e seu resultado.

---

| **Caso de Teste** | **CT-R15-001 – Frontend - Lógica da Calculadora de Metas** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A lógica de cálculo da ferramenta de metas no frontend deve retornar o valor matematicamente correto. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o preenchimento dos campos na interface, o clique no botão "Calcular", e a exibição do resultado correto)* |

---

| **Caso de Teste** | **CT-R15-002 – Frontend - Lógica da Calculadora de Ponto de Equilíbrio** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A lógica de cálculo da ferramenta de Ponto de Equilíbrio no frontend deve retornar o valor matematicamente correto. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o preenchimento dos campos na interface, o clique no botão "Calcular", e a exibição do resultado correto)* |

---

| **Caso de Teste** | **CT-R15-003 – Frontend - Validação de Entrada (Calculadora de Metas)** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A interface deve validar as entradas do usuário para prevenir cálculos inválidos (ex: divisão por zero). |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o usuário tentando calcular com o campo "Prazo" zerado e a exibição de uma mensagem de erro amigável)* |

---

| **Caso de Teste** | **CT-R15-004 – Frontend - Validação de Entrada (Calculadora de Ponto de Equilíbrio)** |
| :--- | :--- |
| **Requisito Associado** | **R15** - A interface deve validar as entradas do usuário para prevenir cálculos inválidos (ex: preço de venda menor que o custo). |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o usuário tentando calcular com um preço de venda menor que o custo e a exibição de uma mensagem de erro)* |

---

| **Caso de Teste** | **CT-R15-005 – Frontend - Tratamento de Divisão por Zero** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - O sistema deve ser robusto e tratar exceções matemáticas para não quebrar a experiência do usuário. |
| **Resultado** | **[PREENCHER: Aprovado / Reprovado]** |
| **Registro de evidência** | **[INSERIR AQUI O LINK PARA O VÍDEO DO SCREENCAST]** <br> *(O vídeo deve mostrar o usuário inserindo preço de venda igual ao custo variável e o sistema exibindo uma mensagem de erro controlada, em vez de "Infinity" ou de travar)* |

---

## Relatório de testes de software

**[PREENCHER ESTA SEÇÃO APÓS A EXECUÇÃO DE TODOS OS TESTES]**

Durante a execução dos 5 casos de teste definidos para a arquitetura client-side do requisito R15 (Ferramentas Interativas), a equipe obteve um total de **[X]** casos aprovados e **[Y]** casos reprovados.

### Pontos Fortes
Os testes revelaram que a lógica de cálculo principal para ambas as calculadoras (`CT-R15-001`, `CT-R15-002`) está funcionando de forma precisa e instantânea, o que valida a abordagem client-side para uma boa experiência do usuário. A performance das ferramentas foi considerada excelente.

### Fragilidades e Falhas Detectadas
A principal fragilidade foi observada durante o teste **`[ID DO TESTE QUE FALHOU, ex: CT-R15-004]`**.

**Descrição da Falha:** **[Descrever o problema em detalhe. Exemplo: A validação de entrada na Calculadora de Ponto de Equilíbrio não impedia que o usuário inserisse texto (letras) nos campos numéricos. Ao clicar em "Calcular", isso resultava em "NaN" (Not a Number) sendo exibido como resultado, o que é confuso para o usuário.]**

**Impacto:** **[Descrever o impacto da falha. Exemplo: O impacto é moderado. Embora não quebre a aplicação, ele gera um resultado ininteligível e passa uma imagem de falta de polimento, podendo diminuir a confiança do usuário na ferramenta.]**

### Estratégias de Correção e Melhorias
Para corrigir a(s) falha(s) identificada(s), a equipe planeja as seguintes ações:

1.  **Correção da Falha `[ID DA FALHA]`:** **[Descrever a ação específica. Exemplo: Implementar uma validação mais robusta nos campos de entrada das calculadoras. Utilizar o atributo `type="number"` nos inputs HTML e adicionar uma verificação em JavaScript para garantir que apenas valores numéricos sejam processados antes de realizar o cálculo.]**
2.  **Melhoria Proposta:** **[Descrever uma melhoria a partir dos testes. Exemplo: Para tornar as ferramentas mais intuitivas, propomos adicionar ícones de "ajuda" (`tooltips`) ao lado de cada campo das calculadoras, explicando termos como "Custos Fixos" ou "Ponto de Equilíbrio".]**

Essas ações serão transformadas em novas *issues* no GitHub e priorizadas para a próxima Sprint, garantindo a evolução e a estabilidade da solução.