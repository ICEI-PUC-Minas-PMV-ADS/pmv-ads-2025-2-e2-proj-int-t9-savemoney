# Projeto de Interface — R3 Conversor de Energia

## 1. Modelos Funcionais

### 1.1 Diagrama de Fluxo (Fluxograma)

Este diagrama representa o fluxo de execução para a funcionalidade de conversão de energia, desde a entrada de dados do usuário até a exibição do resultado e das dicas.

```mermaid
graph TD
    A[Usuário insere os dados do cadastro] --> B{Verificar tipo de pessoa, validar dados obrigatórios, validar email e senha};
    B --> C{Criar Hash da senha};
    C --> D{Registrar usuário no banco de dados};
    M --> N[Fim];
```
