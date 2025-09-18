# Arquitetura da Solução — R2 Educação Financeira

## 1. Introdução

Este documento detalha os artefatos de engenharia de software para o requisito de Educação Financeira. O foco da arquitetura é a implementação de um sistema que agrega conteúdo de fontes externas de forma automatizada, o armazena para consulta rápida e permite que o usuário interaja com o conteúdo, salvando seus artigos favoritos ("curtindo").

## 2. Diagrama de Classes

O diagrama de classes a seguir detalha a arquitetura da funcionalidade, incluindo os principais modelos de dados, os serviços de frontend e backend responsáveis pela lógica, e o serviço de agregação de conteúdo.

```mermaid
classDiagram
    class Usuario {
        -int id
        +List~ConteudoCurtido~ obterConteudosCurtidos()
    }

    class ConteudoAgregado {
        -int id
        -string titulo
        -string resumo
        -string url_original
        -string url_imagem
        -string fonte
        -datetime data_publicacao
    }

    class ConteudoCurtido {
        -int id_usuario
        -int id_conteudo
        -datetime data_curtida
    }

    class ServicoFrontend {
        +List~ConteudoAgregado~ obterConteudos()
        +List~ConteudoAgregado~ obterConteudosCurtidos()
        +void curtirConteudo(conteudoId)
        +void descurtirConteudo(conteudoId)
    }

    class ServicoBackend {
        +List~ConteudoAgregado~ obterConteudos()
        +List~ConteudoAgregado~ obterConteudosCurtidosPorUsuario(usuarioId)
        +void salvarCurtida(usuarioId, conteudoId)
        +void removerCurtida(usuarioId, conteudoId)
    }
    
    class ServicoAgregadorConteudo {
        +void buscarEsalvarNovosConteudos()
        +void enviarNewsletterSemanal()
    }

    class BancoDados {
        +void salvarDados(tabela, dados)
        +dados obterDados(tabela, id)
        +void atualizarDados(tabela, dados)
        +void removerDados(tabela, id)
    }

    Usuario "1" -- "0..*" ConteudoCurtido : curte
    ConteudoAgregado "1" -- "0..*" ConteudoCurtido : é curtido por
    ServicoFrontend "1" -- "1" ServicoBackend : usa
    ServicoBackend "1" -- "1" BancoDados : interage
    ServicoAgregadorConteudo "1" -- "1" BancoDados : interage
