# Registro de Testes de Software — Save Money (R2 Educação Financeira)

<span style="color:red">Pré-requisitos: <a href="3-Projeto de Interface.md"> Projeto de Interface</a></span>, <a href="6-Plano de Testes de Software.md"> Plano de Testes de Software</a>

Esta seção documenta os resultados da execução dos casos de teste definidos no Plano de Testes de Software para o requisito R2. As evidências consistem em **screenshots (imagens .png)**, demonstrando a execução de cada teste e seu resultado.

---

| **Caso de Teste** | **CT-R2-001 – Front-End - Sucesso Artigos** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A interface de Artigos deve carregar e exibir os conteúdos da fonte externa. |
| **Resultado** | **Aprovado** |
| **Registro de evidência** | `../img/CT-R2-001.png` <br> *(A imagem mostra a página de Artigos Acadêmicos carregada com sucesso.)* |

---

| **Caso de Teste** | **CT-R2-002 – Front-End - Sucesso Notícias** |
| :--- | :--- |
| **Requisito Associado** | **R2** - A interface de Notícias deve carregar e exibir os conteúdos da fonte externa. |
| **Resultado** | **Aprovado** |
| **Registro de evidência** | `../img/CT-R2-002.png` <br> *(A imagem mostra a página de Notícias do Mercado Financeiro carregada com sucesso.)* |

---

| **Caso de Teste** | **CT-R2-003 – Front-End - Erro (Rota Inexistente)** |
| :--- | :--- |
| **Requisito Associado** | **RNF** - O sistema deve ser resiliente a rotas inválidas (erros de usuário). |
| **Resultado** | **Aprovado** |
| **Registro de evidência** | `../img/CT-R2-003.png` <br> *(A imagem mostra o tratamento de erro da aplicação, exibindo uma página 404 Not Found.)* |

---

## Relatório de testes de software

Durante a execução dos 3 casos de teste definidos para o front-end do requisito R2 (Educação Financeira), a equipe obteve um total de **3** casos aprovados e **0** casos reprovados.

### Pontos Fortes
Os testes revelaram que as funcionalidades principais do front-end, o carregamento das páginas de Artigos (`CT-R2-001`) e Notícias (`CT-R2-002`), funcionaram de forma correta e rápida. A aplicação também demonstrou robustez no tratamento de erros de rota, retornando corretamente a página 404 (`CT-R2-003`).

### Fragilidades e Falhas Detectadas
Embora os testes definidos tenham sido aprovados, uma falha funcional foi identificada durante a execução manual (conforme visto nas evidências dos testes de "busca vazia" não incluídos): a **funcionalidade de Pesquisa (barra de busca)** não está implementada em nenhuma das telas.

**Descrição da Falha:** Ao inserir texto nas barras de pesquisa de Artigos ou Notícias e clicar em "Buscar", a lista de resultados não é filtrada. A página simplesmente recarrega (ou não faz nada), pois a lógica de front-end e/ou back-end para essa ação ainda não foi desenvolvida.

**Impacto:** O impacto na usabilidade é **Alto**. Sem a busca, o usuário não pode filtrar o conteúdo e depende exclusivamente da rolagem manual para encontrar o que procura, tornando a funcionalidade pouco prática.

### Estratégias de Correção e Melhorias
Para corrigir a(s) falha(s) identificada(s), a equipe planeja as seguintes ações:

1.  **Correção da Falha (Prioridade Alta):** **Implementar a lógica de pesquisa.** É necessário desenvolver a funcionalidade no front-end (para capturar o termo de busca) e no back-end (para filtrar os resultados da API externa) em ambas as telas (Artigos e Notícias).
2.  **Melhoria Proposta (Prioridade Média):** **Implementar feedback de busca vazia.** Como parte da correção, garantir que, se uma busca não retornar resultados, a interface exiba uma mensagem amigável (ex: "Nenhum resultado encontrado para 'termo'"), em vez de apenas uma tela em branco.