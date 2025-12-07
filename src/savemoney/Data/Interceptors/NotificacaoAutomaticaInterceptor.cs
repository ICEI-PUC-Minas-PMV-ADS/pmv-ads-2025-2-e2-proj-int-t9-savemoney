using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Data.Interceptors
{
    public class NotificacaoAutomaticaInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificacaoAutomaticaInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            // 1. Identificar quem está logado
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdString = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Se não tem usuário (ex: sistema rodando seed), não cria notificação
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            // 2. Detectar mudanças antes de salvar
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                // CRÍTICO: Ignora a tabela de notificações para evitar loop infinito
                if (entry.Entity is NotificacaoUsuario) continue;

                // Prepara os dados da notificação
                string nomeClasse = entry.Entity.GetType().Name;
                string nomeAmigavel = TraduzirEntidade(nomeClasse);
                string? nomeItem = ObterNomeItem(entry.Entity); // Tenta achar o "Titulo" ou "Nome" do item

                string acao = "";
                string mensagem = "";
                TipoNotificacao tipo = TipoNotificacao.Info;
                string? link = GerarLink(nomeClasse);

                switch (entry.State)
                {
                    case EntityState.Added:
                        acao = "Criado(a)";
                        mensagem = $"Novo registro de {nomeAmigavel} '{nomeItem}' foi criado com sucesso.";
                        tipo = TipoNotificacao.Sucesso;
                        break;

                    case EntityState.Modified:
                        acao = "Atualizado(a)";
                        mensagem = $"O registro de {nomeAmigavel} '{nomeItem}' foi alterado.";
                        tipo = TipoNotificacao.Info;
                        break;

                    case EntityState.Deleted:
                        acao = "Removido(a)";
                        mensagem = $"O registro de {nomeAmigavel} '{nomeItem}' foi excluído.";
                        tipo = TipoNotificacao.Alerta; // Usa o Alerta que adicionamos no Enum
                        // Se deletou, mandamos para a listagem, pois o detalhe não existe mais
                        link = GerarLink(nomeClasse);
                        break;
                }

                // 3. Adiciona a notificação na fila de salvamento
                var notificacao = new NotificacaoUsuario
                {
                    UsuarioId = userId,
                    Titulo = $"{nomeAmigavel} {acao}",
                    Mensagem = mensagem,
                    Tipo = tipo,
                    LinkAcao = link,
                    DataCriacao = DateTime.Now,
                    Lida = false
                };

                context.Add(notificacao);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        // Helpers Auxiliares

        private string? ObterNomeItem(object entidade)
        {
            // Tenta pegar propriedades comuns via Reflection de forma segura
            var prop = entidade.GetType().GetProperty("Titulo")
                    ?? entidade.GetType().GetProperty("Name")
                    ?? entidade.GetType().GetProperty("Nome")
                    ?? entidade.GetType().GetProperty("Descricao");

            return prop?.GetValue(entidade)?.ToString() ?? "Item";
        }

        private string TraduzirEntidade(string entidade)
        {
            return entidade switch
            {
                "Receita" => "Receita",
                "Despesa" => "Despesa",
                "Category" => "Categoria",
                "Budget" => "Orçamento",
                "MetaFinanceira" => "Meta",
                "Aporte" => "Aporte",
                "Usuario" => "Perfil",
                _ => entidade
            };
        }

        private string GerarLink(string entidade)
        {
            return entidade switch
            {
                "Receita" => "/Receitas/Index",
                "Despesa" => "/Despesas/Index",
                "Category" => "/Categories/Index",
                "Budget" => "/Budgets/Index",
                "MetaFinanceira" => "/MetasFinanceiras/Index",
                "Usuario" => "/Usuarios/Details",
                _ => "/Dashboard/Index"
            };
        }
    }
}