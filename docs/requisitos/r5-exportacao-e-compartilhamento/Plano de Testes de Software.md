# Plano de Testes de Software — R5 Exportação e Compartilhamento

## 1. Introdução
Este plano de testes de software tem como objetivo validar a funcionalidade de **Exportação e Compartilhamento (R5)** no sistema SaveMoney V2.  
Essa funcionalidade deve permitir que usuários exportem relatórios financeiros nos formatos PDF e Excel e compartilhem esses arquivos utilizando as opções nativas do dispositivo (ex: WhatsApp, E-mail, Google Drive).

---

## 2. Casos de Teste

| ID     | Funcionalidade                        | Pré-condições                                                                 | Ações                                                                                  | Resultados Esperados                                                                                   |
| ------ | ------------------------------------- | ------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| CT-201 | Exportação em PDF                     | Relatório gerado e exibido na tela                                            | 1. Clicar em **Exportar**.<br>2. Selecionar formato PDF.<br>3. Confirmar exportação.   | O sistema deve gerar e baixar o arquivo PDF, mantendo integridade, layout e dados do relatório.         |
| CT-202 | Exportação em Excel                   | Relatório gerado e exibido na tela                                            | 1. Clicar em **Exportar**.<br>2. Selecionar formato Excel.<br>3. Confirmar exportação. | O sistema deve gerar e baixar o arquivo Excel (.xlsx), mantendo integridade, layout e dados do relatório. |
| CT-203 | Compartilhamento de PDF               | Arquivo PDF exportado disponível no dispositivo                               | 1. Clicar em **Compartilhar**.<br>2. Selecionar arquivo PDF.<br>3. Escolher app (ex: WhatsApp). | O sistema deve abrir o menu de compartilhamento nativo e permitir envio do PDF pelo app escolhido.      |
| CT-204 | Compartilhamento de Excel             | Arquivo Excel exportado disponível no dispositivo                             | 1. Clicar em **Compartilhar**.<br>2. Selecionar arquivo Excel.<br>3. Escolher app.     | O sistema deve abrir o menu de compartilhamento nativo e permitir envio do Excel pelo app escolhido.    |
| CT-205 | Mensagem de erro na exportação        | Falha na geração do arquivo (ex: erro de conexão, dados inconsistentes)        | 1. Tentar exportar relatório.<br>2. Simular falha no backend.                          | O sistema deve exibir mensagem de erro clara, informando o motivo da falha e sugerindo ação corretiva.  |
| CT-206 | Restrições de permissão no dispositivo| Dispositivo com restrições de armazenamento ou compartilhamento                | 1. Tentar exportar ou compartilhar relatório.<br>2. Negar permissão de acesso.         | O sistema deve informar ao usuário sobre a restrição e orientar como conceder permissão, se aplicável.  |
| CT-207 | Exportação com filtros aplicados      | Relatório gerado com filtros específicos                                      | 1. Aplicar filtros.<br>2. Exportar relatório em PDF e Excel.                           | Os arquivos exportados devem refletir exatamente os dados filtrados, sem divergências.                  |
