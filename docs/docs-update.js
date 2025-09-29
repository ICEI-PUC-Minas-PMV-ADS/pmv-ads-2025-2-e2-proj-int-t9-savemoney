const fs = require("fs");
const path = require("path");

const requisitosDir = path.join(__dirname, "requisitos");

// Função para obter o conteúdo dos wireframes do template padrão
function getWireframesSection(templatePath) {
  if (!fs.existsSync(templatePath)) return "";
  const content = fs.readFileSync(templatePath, "utf-8");
  // Pega tudo a partir de "## Wireframes"
  const match = content.match(/## Wireframes\s*([\s\S]*)/m);
  return match ? match[1].trim() : "";
}

// Caminho do template padrão
const templatePadraoPath = path.join(
  __dirname,
  "06-Template Padrão da Aplicação.md"
);

// Conteúdo fixo das principais telas e funcionalidades
function getFixedScreensSection() {
  return `## Principais Telas e Funcionalidades

1. **Tela Inicial Pública / Sobre Nós**
  - Apresenta a missão do sistema, opções de login, cadastro, funcionalidades e newsletters

2. **Tela de Login**
  - Permite acessar o sistema informando e-mail e senha

3. **Tela de Cadastro**
  - Permite criar uma conta, selecionando o tipo de pessoa e preenchendo os dados obrigatórios

4. **Dashboard Principal**
  - Exibe navegação para todas as principais funcionalidades do sistema

5. **Receitas**
  - Visualizar, cadastrar, editar e excluir receitas, com informações detalhadas e filtros

6. **Despesas**
  - Visualizar, cadastrar, editar e excluir despesas, com informações detalhadas e filtros

7. **Investimentos**
  - Gerenciar investimentos, exibir status, rentabilidade e histórico

8. **Relatórios**
  - Exibe gráficos e resumos financeiros para análise do usuário

9. **Perfil do Usuário**
  - Visualizar e editar informações pessoais, contato e configurações da conta

10. **Contato**
  - Entrar em contato com a equipe de suporte por meio de formulário

11. **Configurações**
  - Editar dados pessoais, contato, endereço e personalizar o tema

12. **Conversor de Moedas**
  - Ferramenta para conversão de moedas, com histórico e dicas

13. **Conversor de Energia**
  - Ferramenta para simulação e cálculo de consumo de energia

14. **Dicas de Energia**
  - Exibe dicas personalizadas relacionadas ao consumo de energia

\n`;
}

// Wireframes extraídos do template padrão
const wireframesSection = getWireframesSection(templatePadraoPath);

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
    header:
      "# Projeto de Interface\n\n" +
      getFixedScreensSection() +
      (wireframesSection
        ? "## Wireframes\n\n" + wireframesSection + "\n\n"
        : ""),
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
