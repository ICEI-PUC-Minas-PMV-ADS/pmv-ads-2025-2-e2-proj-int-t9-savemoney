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

        // 1. Método Genérico
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

        // 2. R11 - Verificar Orçamentos
        public async Task VerificarAlertasOrcamento(int userId)
        {
            var hoje = DateTime.Today;

            var categoriasOrcamento = await _context.BudgetCategories
                .Include(bc => bc.Budget)
                .Include(bc => bc.Category)
                .Where(bc => bc.Budget!.UserId == userId // Adicionado ! para silenciar warning
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
                var categoriaNome = item.Category?.Name ?? "Categoria"; // Null safety

                if (percentual >= 100)
                {
                    titulo = "Orçamento Estourado!";
                    msg = $"Você excedeu o limite de {categoriaNome}. Gasto: {item.CurrentSpent:C} / Limite: {item.Limit:C}";
                    tipo = TipoNotificacao.AlertaOrcamento;
                }
                else if (percentual >= 90)
                {
                    titulo = "Atenção ao Orçamento";
                    msg = $"Você já consumiu {percentual:F0}% do limite de {categoriaNome}.";
                    tipo = TipoNotificacao.AlertaOrcamento;
                }

                if (!string.IsNullOrEmpty(titulo))
                {
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

        // 3. R11 - Verificar Contas a Pagar
        public async Task VerificarContasProximas(int userId)
        {
            var hoje = DateTime.Today;
            var limiteAlerta = hoje.AddDays(3);

            var contasPendentes = await _context.Despesas
                .Where(d => d.UsuarioId == userId
                            && !d.Pago
                            && d.DataFim <= limiteAlerta
                            && d.DataFim >= hoje.AddDays(-30))
                .ToListAsync();

            foreach (var conta in contasPendentes)
            {
                string titulo = conta.DataFim < hoje ? "Conta Atrasada!" : "Conta Vencendo";
                string msg = $"{conta.Titulo} ({conta.Valor:C}) vence em {conta.DataFim:dd/MM}.";
                var tipo = conta.DataFim < hoje ? TipoNotificacao.Erro : TipoNotificacao.ContaPendente;

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

        // 4. Sincronizar Updates do Sistema
        public async Task SincronizarSistema(int userId)
        {
            var path = Path.Combine(_env.WebRootPath, "data", "sistema_updates.json");
            if (!File.Exists(path)) return;

            try
            {
                var jsonContent = await File.ReadAllTextAsync(path);
                var updates = JsonSerializer.Deserialize<List<UpdateItemDto>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (updates == null) return;

                var idsRecebidos = await _context.Notificacoes
                    .Where(n => n.UsuarioId == userId && n.CodigoReferenciaSistema != null)
                    .Select(n => n.CodigoReferenciaSistema)
                    .ToListAsync();

                bool houveAdicao = false;

                foreach (var update in updates)
                {
                    // Null safety checks para o DTO
                    if (update.Id != null && !idsRecebidos.Contains(update.Id))
                    {
                        var novaNotif = new NotificacaoUsuario
                        {
                            UsuarioId = userId,
                            Titulo = update.Titulo ?? "Atualização do Sistema",
                            Mensagem = update.Mensagem ?? "Confira as novidades.",
                            Tipo = TipoNotificacao.Sistema,
                            DataCriacao = update.Data,
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
                // Ignora falhas na leitura
            }
        }

        // 5. Gerenciamento de Limite
        private async Task GerenciarLimiteArmazenamento(int userId)
        {
            var count = await _context.Notificacoes.CountAsync(n => n.UsuarioId == userId);

            if (count >= 100)
            {
                var qtdRemover = count - 99;
                var paraRemover = await _context.Notificacoes
                    .Where(n => n.UsuarioId == userId)
                    .OrderBy(n => n.DataCriacao)
                    .Take(qtdRemover)
                    .ToListAsync();

                if (paraRemover.Any())
                {
                    _context.Notificacoes.RemoveRange(paraRemover);
                }
            }
        }
    }

    // DTO Null Safe
    public class UpdateItemDto
    {
        public string? Id { get; set; }
        public string? Titulo { get; set; }
        public string? Mensagem { get; set; }
        public DateTime Data { get; set; }
    }
}