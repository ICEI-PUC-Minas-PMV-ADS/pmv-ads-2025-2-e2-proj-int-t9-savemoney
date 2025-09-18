# Plano de Testes de Software — R15 Ferramentas Interativas

## 1. Introdução

O plano de testes foca em garantir a funcionalidade completa do módulo de Ferramentas Interativas. Conforme a arquitetura 100% client-side, os testes se concentram na validação da lógica de cálculo implementada em JavaScript e no tratamento de entradas de dados do usuário diretamente no frontend, assegurando que as calculadoras sejam precisas e robustas.

## 2. Casos de Teste

| ID | Funcionalidade | Pré-condições | Ações | Resultados Esperados |
| :--- | :--- | :--- | :--- | :--- |
| **CT-R15-001** | Frontend - Lógica da Calculadora de Metas | Nenhuma. | 1. Na Calculadora de Metas, inserir `Valor do Objetivo = 12000` e `Prazo = 24 meses`. <br> 2. Clicar em "Calcular". | A interface deve exibir o resultado "R$ 500,00 por mês". O cálculo JavaScript deve retornar `500`. |
| **CT-R15-002** | Frontend - Lógica da Calculadora de Ponto de Equilíbrio | Nenhuma. | 1. Na Calculadora de Ponto de Equilíbrio, inserir `Custos Fixos = 3000`, `Preço de Venda = 10`, `Custo Variável = 4`. <br> 2. Clicar em "Calcular". | A interface deve exibir o resultado "500 unidades". O cálculo JavaScript deve retornar `500`. |
| **CT-R15-003** | Frontend - Validação de Entrada (Calculadora de Metas) | Nenhuma. | 1. Na Calculadora de Metas, deixar o campo `Prazo` em branco ou com valor `0`. <br> 2. Clicar em "Calcular". | O sistema não deve quebrar. Uma mensagem de erro amigável deve ser exibida ao usuário (ex: "O prazo deve ser maior que zero"). |
| **CT-R15-004** | Frontend - Validação de Entrada (Calculadora de Ponto de Equilíbrio) | Nenhuma. | 1. Na Calculadora de Ponto de Equilíbrio, inserir um `Preço de Venda` menor que o `Custo Variável`. <br> 2. Clicar em "Calcular". | O sistema não deve quebrar. Uma mensagem de erro deve ser exibida (ex: "O preço de venda deve ser maior que o custo"). |
| **CT-R15-005** | Frontend - Tratamento de Divisão por Zero | Nenhuma. | 1. Na Calculadora de Ponto de Equilíbrio, inserir `Preço de Venda` igual ao `Custo Variável` (ex: 10 e 10). <br> 2. Clicar em "Calcular". | O sistema não deve retornar "Infinity" ou quebrar. Uma mensagem de erro clara deve ser exibida ao usuário. |