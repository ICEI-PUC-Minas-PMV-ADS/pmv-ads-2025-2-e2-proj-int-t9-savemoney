# Arquitetura da Solução — R15 Ferramentas Interativas

## 1. Introdução

Este documento detalha os artefatos de engenharia de software para o requisito de Ferramentas Interativas. O foco da arquitetura é a implementação de uma suíte de calculadoras de planejamento financeiro, incluindo a **Calculadora de Metas** e a **Calculadora de Ponto de Equilíbrio**. A arquitetura dá suporte a esta última através de um módulo de **cadastro de produtos**, permitindo que o usuário de perfil de negócios salve e gerencie os dados de suas mercadorias (preço de venda, custo) de forma persistente.

## 2. Diagrama de Classes

O diagrama de classes a seguir detalha a arquitetura da funcionalidade, com destaque para a nova classe `Produto` e os serviços de backend necessários para a sua gestão, que darão suporte à Calculadora de Ponto de Equilíbrio.

```mermaid
classDiagram
    class Usuario {
        -int id
        +List~Produto~ obterProdutosCadastrados()
    }

    class Produto {
        -int id
        -int id_usuario
        -string nome
        -decimal preco_venda
        -decimal custo_variavel
    }

    class ServicoFrontend {
        +void calcularMeta(objetivo, prazo)
        +void calcularPontoEquilibrio(custosFixos, produto)
        +List~Produto~ obterProdutos()
        +void salvarProduto(produto)
        +void deletarProduto(produtoId)
    }

    class ServicoBackend {
        +List~Produto~ obterProdutosPorUsuario(usuarioId)
        +Produto criarProduto(dadosProduto)
        +Produto atualizarProduto(produtoId, dadosProduto)
        +void deletarProduto(produtoId)
    }

    class BancoDados {
        +void salvarDados(tabela, dados)
        +dados obterDados(tabela, id)
        +void atualizarDados(tabela, dados)
        +void removerDados(tabela, id)
    }

    Usuario "1" -- "0..*" Produto : cadastra
    ServicoFrontend "1" -- "1" ServicoBackend : usa
    ServicoBackend "1" -- "1" BancoDados : interage