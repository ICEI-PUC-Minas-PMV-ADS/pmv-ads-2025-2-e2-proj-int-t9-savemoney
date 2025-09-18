const fs = require("fs");
const path = require("path");

const outputInterfaceFile = path.join(__dirname, '08-Projeto de Interface.md');

function getAllInterfaceProjects(dir) {
  let files = [];
  const items = fs.readdirSync(dir, { withFileTypes: true });
  for (const item of items) {
    const fullPath = path.join(dir, item.name);
    if (item.isDirectory()) {
      files = files.concat(getAllInterfaceProjects(fullPath));
    } else if (item.name === 'Projeto de Interface.md') {
      files.push(fullPath);
    }
  }
  return files;
}
function mainInterface() {
  const interfaceFiles = getAllInterfaceProjects(requisitosDir);
  let output = '';

  if (interfaceFiles.length === 0) {
    output += 'Nenhum projeto de interface encontrado.';
  } else {
    interfaceFiles.forEach((file) => {
      const content = fs.readFileSync(file, 'utf-8');
      output += content.trim() + '\n\n';
    });
  }

  fs.writeFileSync(outputInterfaceFile, output, 'utf-8');
  console.log(`Arquivo global criado em: ${outputInterfaceFile}`);
}

const outputUsabFile = path.join(
  __dirname,
  "08-Plano de Testes de Usabilidade.md"
);

function getAllUsabilityTestPlans(dir) {
  let files = [];
  const items = fs.readdirSync(dir, { withFileTypes: true });
  for (const item of items) {
    const fullPath = path.join(dir, item.name);
    if (item.isDirectory()) {
      files = files.concat(getAllUsabilityTestPlans(fullPath));
    } else if (item.name === "Plano de Testes de Usabilidade.md") {
      files.push(fullPath);
    }
  }
  return files;
}
function mainUsab() {
  const testPlanFiles = getAllUsabilityTestPlans(requisitosDir);
  let output = "";

  if (testPlanFiles.length === 0) {
    output += "Nenhum plano de testes de usabilidade encontrado.";
  } else {
    testPlanFiles.forEach((file) => {
      const content = fs.readFileSync(file, "utf-8");
      output += content.trim() + "\n\n";
    });
  }

  fs.writeFileSync(outputUsabFile, output, "utf-8");
  console.log(`Arquivo global criado em: ${outputUsabFile}`);
}

const requisitosDir = path.join(__dirname, "requisitos");
const outputFile = path.join(__dirname, "08-Plano de Testes de Software.md");

// Clean output file before running
if (fs.existsSync(outputFile)) {
  fs.unlinkSync(outputFile);
}

function getAllTestPlans(dir) {
  let files = [];
  const items = fs.readdirSync(dir, { withFileTypes: true });
  for (const item of items) {
    const fullPath = path.join(dir, item.name);
    if (item.isDirectory()) {
      files = files.concat(getAllTestPlans(fullPath));
    } else if (item.name === "Plano de Testes de Software.md") {
      files.push(fullPath);
    }
  }
  return files;
}

function main() {
  const testPlanFiles = getAllTestPlans(requisitosDir);
  let output = "";

  if (testPlanFiles.length === 0) {
    output += "Nenhum plano de testes encontrado.";
  } else {
    testPlanFiles.forEach((file) => {
      const content = fs.readFileSync(file, "utf-8");
      output += content.trim() + "\n\n";
    });
  }

  fs.writeFileSync(outputFile, output, "utf-8");
  console.log(`Arquivo global criado em: ${outputFile}`);
}

main();
mainUsab();
mainInterface();
