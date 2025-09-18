const fs = require("fs");
const path = require("path");

const requisitosDir = path.join(__dirname, "requisitos");

// Configuração dos tipos de agregação
const aggregations = [
  {
    output: path.join(__dirname, "08-Plano de Testes de Software.md"),
    searchFile: "Plano de Testes de Software.md",
    header: "",
    emptyMsg: "Nenhum plano de testes encontrado.",
  },
  {
    output: path.join(__dirname, "10-Plano de Testes de Usabilidade.md"),
    searchFile: "Plano de Testes de Usabilidade.md",
    header: "",
    emptyMsg: "Nenhum plano de testes de usabilidade encontrado.",
  },
  {
    output: path.join(__dirname, "04-Projeto de Interface.md"),
    searchFile: "Projeto de Interface.md",
    header: `# Projeto de Interface

#### Principais Telas e Funcionalidades

1. **Tela de Login e Cadastro**
  - Formulário de autenticação
  - Processo de registro de novos usuários
  - Recuperação de senha

2. **Dashboard Principal**
  - Visão geral das finanças
  - Gráficos de receitas vs despesas
  - Resumo mensal e anual
  - Navegação para funcionalidades principais

3. **Gestão de Transações**
  - Formulário para adicionar receitas e despesas
  - Lista de transações com filtros
  - Edição e exclusão de transações
  - Categorização automática e manual

4. **Relatórios e Análises**
  - Gráficos interativos
  - Filtros por período e categoria
  - Exportação de dados
  - Comparativos mensais/anuais

5. **Configurações e Perfil**
  - Dados pessoais do usuário
  - Preferências da aplicação
  - Categorias personalizadas
  - Configurações de notificações
\n\n`,
    emptyMsg: "Nenhum projeto de interface encontrado.",
  },
];

function getAllFilesByName(dir, filename) {
  let files = [];
  let items = fs.readdirSync(dir, { withFileTypes: true });

  // Ordena diretórios de requisitos (r1, r2, ..., r16) corretamente
  items = items.sort((a, b) => {
    const reqRegex = /^r(\d+)[^/\\]*$/i;
    const aMatch = a.name.match(reqRegex);
    const bMatch = b.name.match(reqRegex);
    if (aMatch && bMatch) {
      return parseInt(aMatch[1], 10) - parseInt(bMatch[1], 10);
    }
    if (aMatch) return -1;
    if (bMatch) return 1;
    return a.name.localeCompare(b.name);
  });

  for (const item of items) {
    const fullPath = path.join(dir, item.name);
    if (item.isDirectory()) {
      files = files.concat(getAllFilesByName(fullPath, filename));
    } else if (item.name === filename) {
      files.push(fullPath);
    }
  }
  return files;
}

function aggregateFiles({ output, searchFile, header, emptyMsg }) {
  fs.writeFileSync(output, "", "utf-8");

  const foundFiles = getAllFilesByName(requisitosDir, searchFile);
  let outputContent = header || "";

  if (foundFiles.length === 0) {
    outputContent += emptyMsg;
  } else {
    foundFiles.forEach((file) => {
      const content = fs.readFileSync(file, "utf-8");
      outputContent += content.trim() + "\n\n";
    });
  }

  fs.writeFileSync(output, outputContent, "utf-8");
  console.log(`Arquivo global criado em: ${output}`);
}

// Executa todas as agregações configuradas
aggregations.forEach(aggregateFiles);
