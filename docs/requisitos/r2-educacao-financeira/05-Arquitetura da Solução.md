# Arquitetura da Solução — R2 Educação Financeira

## 1. Introdução

Este documento detalha os artefatos de engenharia de software para o requisito de Educação Financeira. Conforme a decisão de design, esta funcionalidade operará em tempo real, sem persistência de dados no banco de dados da aplicação. O foco da arquitetura é a implementação de um sistema que atua como um *proxy*, buscando conteúdo de fontes externas de forma automatizada e exibindo-o diretamente para o usuário, sem salvar os conteúdos ou as interações do usuário.

## 2. Diagrama de Classes

O diagrama de classes a seguir detalha a arquitetura simplificada da funcionalidade. Ele remove as classes de modelo de dados (`ConteudoAgregado`, `ConteudoCurtido`) e a interação com o banco de dados, focando nos serviços que orquestram a busca de conteúdo externo e a entrega para o frontend.

```mermaid
classDiagram
    class ServicoFrontend {
        +List~ConteudoExterno~ obterConteudos()
    }

    class ServicoBackend {
        +List~ConteudoExterno~ obterConteudos()
    }
    
    class ServicoAgregadorConteudo {
        +List~ConteudoExterno~ buscarConteudosExternos()
        +void enviarNewsletterSemanal()
    }

    class ConteudoExterno {
      <<DTO>>
      -string titulo
      -string resumo
      -string url_original
      -string url_imagem
      -string fonte
    }

    ServicoFrontend "1" -- "1" ServicoBackend : consome
    ServicoBackend "1" -- "1" ServicoAgregadorConteudo : utiliza