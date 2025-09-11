# Arquitetura da Solução — R2 Educação Financeira

## 1. Introdução

Este documento detalha os artefatos de engenharia de software para o requisito de Educação Financeira. O foco da arquitetura é a implementação de um sistema que agrega conteúdo de fontes externas de forma automatizada, o armazena para consulta rápida e permite que o usuário interaja com o conteúdo, salvando seus artigos favoritos ("curtindo").

## 2. Diagrama de Classes

O diagrama de classes a seguir detalha a arquitetura da funcionalidade, incluindo os principais modelos de dados, os serviços de frontend e backend responsáveis pela lógica, e o serviço de agregação de conteúdo.

```mermaid
classDiagram
    class Usuario {
        -int id
        +List~ArtigoCurtido~ obterArtigosCurtidos()
    }

    class ArtigoAgregado {
        -int id
        -string titulo
        -string resumo
        -string url_original
        -string url_imagem
        -string fonte
        -datetime data_publicacao
    }

    class ArtigoCurtido {
        -int id_usuario
        -int id_artigo
        -datetime data_curtida
    }

    class ServicoFrontend {
        +List~ArtigoAgregado~ obterArtigos()
        +List~ArtigoAgregado~ obterArtigosCurtidos()
        +void curtirArtigo(artigoId)
        +void descurtirArtigo(artigoId)
    }

    class ServicoBackend {
        +List~ArtigoAgregado~ obterArtigos()
        +List~ArtigoAgregado~ obterArtigosCurtidosPorUsuario(usuarioId)
        +void salvarCurtida(usuarioId, artigoId)
        +void removerCurtida(usuarioId, artigoId)
    }
    
    class ServicoAgregadorConteudo {
        +void buscarEsalvarNovosArtigos()
        +void enviarNewsletterSemanal()
    }

    class BancoDados {
        +void salvarDados(tabela, dados)
        +dados obterDados(tabela, id)
        +void atualizarDados(tabela, dados)
        +void removerDados(tabela, id)
    }

    Usuario "1" -- "0..*" ArtigoCurtido : curte
    ArtigoAgregado "1" -- "0..*" ArtigoCurtido : é curtido por
    ServicoFrontend "1" -- "1" ServicoBackend : usa
    ServicoBackend "1" -- "1" BancoDados : interage
    ServicoAgregadorConteudo "1" -- "1" BancoDados : interage