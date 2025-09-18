# Plano de Testes de Software — R15 Ferramentas Interativas

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Ferramentas Interativas. Os testes cobrem a API REST para o gerenciamento de produtos cadastrados pelo usuário, a segurança e a integridade dos dados, e também a lógica de cálculo client-side das calculadoras, assegurando que tanto o backend quanto o frontend funcionem conforme especificado.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R15-001** | API - Cadastrar novo produto | Usuário (perfil PJ, "Maria") está logado. | 1. Realizar uma requisição `POST` para `/api/produtos` com dados válidos (nome: "Café Expresso", preco_venda: 5.00, custo_variavel: 1.00). | A API retorna `201 Created` e o JSON do novo produto. O produto é salvo no banco associado ao `id_usuario` correto. |
| **CT-R15-002** | API - Listar produtos do usuário | Usuário "Maria" está logado e tem 3 produtos cadastrados. Usuário "João" está logado e não tem produtos. | 1. "Maria" realiza `GET` para `/api/produtos`. <br> 2. "João" realiza `GET` para `/api/produtos`. | 1. A API retorna `200 OK` e uma lista com os 3 produtos de Maria. <br> 2. A API retorna `200 OK` e uma lista vazia. |
| **CT-R15-003** | API - Editar um produto existente | Usuário "Maria" está logado. Existe um produto com `id=1` pertencente a ela. | 1. Realizar uma requisição `PUT` para `/api/produtos/1` alterando o `preco_venda` para `6.00`. | A API retorna `200 OK`. O campo `preco_venda` do produto `id=1` é atualizado no banco. |
| **CT-R15-004** | API - Excluir um produto | Usuário "Maria" está logado. Existe um produto com `id=1` pertencente a ela. | 1. Realizar uma requisição `DELETE` para `/api/produtos/1`. | A API retorna `204 No Content` ou `200 OK`. O produto `id=1` é removido da tabela `produtos`. |
| **CT-R15-005** | API - Segurança (Acesso indevido) | Usuário "João" está logado. O produto `id=1` pertence à "Maria". | 1. "João" tenta realizar um `PUT` ou `DELETE` no endpoint `/api/produtos/1`. | A API retorna erro `403 Forbidden` ou `404 Not Found`. Nenhuma alteração é feita no produto da Maria. |
| **CT-R15-006** | API - Validação de dados | Usuário "Maria" está logado. | 1. Realizar `POST` para `/api/produtos` com `preco_venda` com valor negativo. | A API retorna erro `400 Bad Request` com uma mensagem de validação clara. Nenhum produto é salvo. |
| **CT-R15-007** | Frontend - Lógica da Calculadora de Metas | Nenhuma. | 1. Executar a função de cálculo no frontend com os parâmetros: `objetivo=12000`, `prazo=24`. | A função JavaScript deve retornar o resultado `500`. |
| **CT-R15-008** | Frontend - Lógica da Calculadora de Ponto de Equilíbrio | Nenhuma. | 1. Executar a função de cálculo no frontend com: `custosFixos=3000`, `produto={preco_venda: 10, custo_variavel: 4}`. | A função JavaScript deve retornar o resultado `500` (unidades). |

---