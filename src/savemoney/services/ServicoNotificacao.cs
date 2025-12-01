using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace savemoney.Services
{
    public class ServicoNotificacao
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServicoNotificacao(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. Método Genérico para criar qualquer notificação
        public async Task Criar(int userId, string titulo, string mensagem, TipoNotificacao tipo, string? link = null)
        {
            var notif = new NotificacaoUsuario
            {
                UsuarioId = userId,
                Titulo = titulo,
                Mensagem = mensagem,
                Tipo = tipo,
                LinkAcao = link,
                DataCriacao = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(notif);
            await GerenciarLimiteArmazenamento(userId);
            await _context.SaveChangesAsync();
        }

        // 2. R11 - Verificar Orçamentos (Gastos próximos ou acima do limite)
        // Deve ser chamado ao carregar o Dashboard ou ao adicionar uma Despesa
        public async Task VerificarAlertasOrcamento(int userId)
        {
            var hoje = DateTime.Today;

            // Busca categorias de orçamentos ativos
            var categoriasOrcamento = await _context.BudgetCategories
                .Include(bc => bc.Budget)
                .Include(bc => bc.Category)
                .Where(bc => bc.Budget.UserId == userId
                             && bc.Budget.StartDate <= hoje
                             && bc.Budget.EndDate >= hoje)
                .ToListAsync();

            foreach (var item in categoriasOrcamento)
            {
                if (item.Limit <= 0) continue;

                var percentual = (item.CurrentSpent / item.Limit) * 100;
                string titulo = "";
                string msg = "";
                TipoNotificacao tipo = TipoNotificacao.Info;

                // Lógica de alerta
                if (percentual >= 100)
                {
                    titulo = "Orçamento Estourado!";
                    msg = $"Você excedeu o limite de {item.Category.Name}. Gasto: {item.CurrentSpent:C} / Limite: {item.Limit:C}";
                    tipo = TipoNotificacao.AlertaOrcamento;
                }
                else if (percentual >= 90)
                {
                    titulo = "Atenção ao Orçamento";
                    msg = $"Você já consumiu {percentual:F0}% do limite de {item.Category.Name}.";
                    tipo = TipoNotificacao.AlertaOrcamento;
                }

                if (!string.IsNullOrEmpty(titulo))
                {
                    // Evita spam: Só cria se não houver notificação idêntica criada hoje
                    bool jaNotificadoHoje = await _context.Notificacoes.AnyAsync(n =>
                        n.UsuarioId == userId &&
                        n.Titulo == titulo &&
                        n.Mensagem == msg &&
                        n.DataCriacao.Date == hoje);

                    if (!jaNotificadoHoje)
                    {
                        await Criar(userId, titulo, msg, tipo, "/Budgets/Index");
                    }
                }
            }
        }

        // 3. R11 - Verificar Contas a Pagar (Próximos 3 dias ou Vencidas)
        public async Task VerificarContasProximas(int userId)
        {
            var hoje = DateTime.Today;
            var limiteAlerta = hoje.AddDays(3);

            // Busca despesas não pagas vencendo em breve ou vencidas recentemente
            var contasPendentes = await _context.Despesas
                .Where(d => d.UsuarioId == userId
                            && !d.Pago
                            && d.DataFim <= limiteAlerta
                            && d.DataFim >= hoje.AddDays(-30)) // Olha até 30 dias pra trás
                .ToListAsync();

            foreach (var conta in contasPendentes)
            {
                string titulo = conta.DataFim < hoje ? "Conta Atrasada!" : "Conta Vencendo";
                string msg = $"{conta.Titulo} ({conta.Valor:C}) vence em {conta.DataFim:dd/MM}.";
                var tipo = conta.DataFim < hoje ? TipoNotificacao.Erro : TipoNotificacao.ContaPendente;

                // Evita spam diário da mesma conta
                bool jaNotificadoHoje = await _context.Notificacoes.AnyAsync(n =>
                        n.UsuarioId == userId &&
                        n.Mensagem == msg &&
                        n.DataCriacao.Date == hoje);

                if (!jaNotificadoHoje)
                {
                    await Criar(userId, titulo, msg, tipo, "/Despesas/Index");
                }
            }
        }

        // 4. Sincronizar Updates do Sistema (Lê do JSON e salva no banco)
        public async Task SincronizarSistema(int userId)
        {
            var path = Path.Combine(_env.WebRootPath, "data", "sistema_updates.json");
            if (!File.Exists(path)) return;

            try
            {
                var jsonContent = await File.ReadAllTextAsync(path);
                // Case insensitive para garantir leitura correta
                var updates = JsonSerializer.Deserialize<List<UpdateItemDto>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (updates == null) return;

                // Busca quais updates esse usuário já recebeu
                var idsRecebidos = await _context.Notificacoes
                    .Where(n => n.UsuarioId == userId && n.CodigoReferenciaSistema != null)
                    .Select(n => n.CodigoReferenciaSistema)
                    .ToListAsync();

                bool houveAdicao = false;

                foreach (var update in updates)
                {
                    if (!idsRecebidos.Contains(update.Id))
                    {
                        var novaNotif = new NotificacaoUsuario
                        {
                            UsuarioId = userId,
                            Titulo = update.Titulo,
                            Mensagem = update.Mensagem,
                            Tipo = TipoNotificacao.Sistema,
                            DataCriacao = update.Data, // Usa a data do update, não a de hoje
                            CodigoReferenciaSistema = update.Id,
                            Lida = false
                        };
                        _context.Notificacoes.Add(novaNotif);
                        houveAdicao = true;
                    }
                }

                if (houveAdicao)
                {
                    await GerenciarLimiteArmazenamento(userId);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                // Ignora falhas na leitura do JSON para não travar o sistema
            }
        }

        // 5. Gerenciamento de Limite (Privado)
        // Mantém apenas as 100 mais recentes
        private async Task GerenciarLimiteArmazenamento(int userId)
        {
            // Verifica contagem local na memória do tracker ou no banco
            var count = await _context.Notificacoes.CountAsync(n => n.UsuarioId == userId);

            if (count >= 100)
            {
                var qtdRemover = count - 99; // Remove o excesso para deixar espaço para a nova

                var paraRemover = await _context.Notificacoes
                    .Where(n => n.UsuarioId == userId)
                    .OrderBy(n => n.DataCriacao) // Pega as mais antigas
                    .Take(qtdRemover)
                    .ToListAsync();

                if (paraRemover.Any())
                {
                    _context.Notificacoes.RemoveRange(paraRemover);
                }
            }
        }
    }

    // DTO auxiliar para ler o JSON
    public class UpdateItemDto
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public DateTime Data { get; set; }
    }
}