# Registro de Testes de Software

<span style="color:red">Pré-requisitos: <a href="3-Projeto de Interface.md"> Projeto de Interface</a></span>, <a href="8-Plano de Testes de Software.md"> Plano de Testes de Software</a>

Esta seção documenta os resultados da execução dos casos de teste definidos no Plano de Testes de Software. As evidências consistem em **screenshots (imagens .png)**, demonstrando a execução de cada teste e seu resultado.

---

## R1 - [NOME DO REQUISITO 1]

| **Caso de Teste** | **[ID DO TESTE, ex: CT-R1-001]** |
| :--- | :--- |
| **Requisito Associado** | **R1** - [Descrição do Requisito] |
| **Resultado** | **[Aprovado / Reprovado]** |
| **Registro de evidência** | `img/CT-R1-001.png` <br> *(Descrição da evidência)* |

### Relatório Específico do R1
**[PREENCHER COM A ANÁLISE DO R1]**

---

## R2 - Educação Financeira

| **Caso de Teste** | **CT-R2-001 – Front-End - Sucesso Artigos** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A interface de Artigos deve carregar e exibir os conteúdos da fonte externa. |
| **Resultado** | **Aprovado** |
| **Registro de evidência** | `img/CT-R2-001.png` <br> *(A imagem mostra a página de Artigos Acadêmicos carregada com sucesso.)* |

---

| **Caso de Teste** | **CT-R2-002 – Front-End - Sucesso Notícias** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A interface de Notícias deve carregar e exibir os conteúdos da fonte externa. |
| **Resultado** | **Aprovado** |
| **Registro de evidência** | `img/CT-R2-002.png` <br> *(A imagem mostra a página de Notícias do Mercado Financeiro carregada com sucesso.)* |

---

| **Caso de Teste** | **CT-R2-003 – Front-End - Erro (Rota Inexistente)** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - O sistema deve ser resiliente a rotas inválidas (erros de usuário). |
| **Resultado** | **Aprovado** |
| **Registro de evidência** | `img/CT-R2-003.png` <br> *(A imagem mostra o tratamento de erro da aplicação, exibindo uma página 404 Not Found.)* |

---

### Relatório Específico do R2

Durante a execution dos 3 casos de teste definidos para o front-end do requisito R2 (Educação Financeira), a equipe obteve um total de **3** casos aprovados e **0** casos reprovados.

#### Pontos Fortes (R2)
Os testes revelaram que as funcionalidades principais do front-end, o carregamento das páginas de Artigos (`CT-R2-001`) e Notícias (`CT-R2-002`), funcionaram de forma correta e rápida. A aplicação também demonstrou robustez no tratamento de erros de rota, retornando corretamente a página 404 (`CT-R2-003`).

#### Fragilidades e Falhas Detectadas (R2)
Embora os testes definidos tenham sido aprovados, uma falha funcional foi identificada durante a execução manual: a **funcionalidade de Pesquisa (barra de busca)** não está implementada em nenhuma das telas.

**Descrição da Falha:** Ao inserir texto nas barras de pesquisa de Artigos ou Notícias e clicar em "Buscar" (ou "Pesquisar"), a lista de resultados não é filtrada. A página não reage à busca, pois a lógica de front-end e/ou back-end para essa ação ainda não foi desenvolvida (uma falha intencional devido à priorização na entrega).

**Impacto:** O impacto na usabilidade é **Alto**. Sem a busca, o usuário não pode filtrar o conteúdo e depende exclusivamente da rolagem manual para encontrar o que procura, tornando a funcionalidade pouco prática.

#### Estratégias de Correção e Melhorias (R2)
Para corrigir a(s) falha(s) identificada(s), a equipe planeja as seguintes ações:

1.  **Correção da Falha (Prioridade Alta):** **Implementar a lógica de pesquisa.** É necessário desenvolver a funcionalidade no front-end (para capturar o termo de busca) e no back-end (para filtrar os resultados da API externa) em ambas as telas (Artigos e Notícias).
2.  **Melhoria Proposta (Prioridade Média):** **Implementar feedback de busca vazia.** Como parte da correção, garantir que, se uma busca não retornar resultados, a interface exiba uma mensagem amigável (ex: "Nenhum resultado encontrado"), em vez de apenas uma tela em branco.

---

## R3 - [NOME DO REQUISITO 3]

| **Caso de Teste** | **[ID DO TESTE, ex: CT-R3-001]** |
| :--- | :--- |
| **Requisito Associado** | **R3** - [Descrição do Requisito] |
| **Resultado** | **[Aprovado / Reprovado]** |
| **Registro de evidência** | `img/CT-R3-001.png` <br> *(Descrição da evidência)* |

### Relatório Específico do R3
**[PREENCHER COM A ANÁLISE DO R3]**

---

## R4 - [NOME DO REQUISITO 4]
**[A SER PREENCHIDO]**

---

## R5 - [NOME DO REQUISITO 5]
**[A SER PREENCHIDO]**

---

... (E assim por diante para R6 até R16) ...

---

## Relatório Geral de Testes de Software

**[PREENCHER ESTA SEÇÃO APÓS A EXECUÇÃO DE TODOS OS REQUISITOS]**

*Resumo geral dos testes. Ex: Dos 16 requisitos planejados, 2 foram testados, resultando em X casos aprovados e Y reprovados. Os principais bloqueadores encontrados foram...*

> **Links Úteis**:
> - [Ferramentas de Test para Java Script](https://geekflare.com/javascript-unit-testing/)