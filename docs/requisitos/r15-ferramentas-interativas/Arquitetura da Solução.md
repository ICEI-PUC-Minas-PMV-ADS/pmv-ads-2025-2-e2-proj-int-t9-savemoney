# Arquitetura da Solução — R15 Ferramentas Interativas

## 1. Introdução

Este documento detalha os artefatos de engenharia de software para o requisito de Ferramentas Interativas. Conforme a decisão de design, esta funcionalidade operará inteiramente no frontend (client-side). A arquitetura foca na implementação de uma suíte de calculadoras de planejamento financeiro, incluindo a **Calculadora de Metas** e a **Calculadora de Ponto de Equilíbrio**, com todos os cálculos sendo executados em tempo real no navegador do usuário a partir de dados inseridos manualmente.

## 2. Diagrama de Classes

O diagrama de classes a seguir detalha a arquitetura simplificada da funcionalidade, focada exclusivamente nos componentes de frontend. Ele remove todas as classes de backend e banco de dados, representando a lógica de cálculo que será implementada em JavaScript.

```mermaid
classDiagram
    class ServicoFrontend {
        +calcularMeta(valorObjetivo, prazoMeses)
        +calcularPontoEquilibrio(custosFixos, precoVenda, custoVariavel)
    }

    class CalculadoraUI {
        -ServicoFrontend servico
        +renderizarCalculadoraMetas()
        +renderizarCalculadoraPontoEquilibrio()
        +exibirResultado(resultado)
    }
    
    CalculadoraUI "1" -- "1" ServicoFrontend : utiliza