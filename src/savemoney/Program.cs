using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using savemoney.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using savemoney.Data.Interceptors; // <--- NECESSÁRIO PARA O INTERCEPTOR

namespace savemoney
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            if (!builder.Environment.IsDevelopment())
            {
                // Só usa o caminho da hospedagem se NÃO ESTIVER em desenvolvimento
                builder.Services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"h:\root\home\maiconvts-001\www\site1\DataProtection-Keys"));
            }

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            // ==============================================================================
            // 1. REGISTRO DO INTERCEPTOR DE NOTIFICAÇÕES (NOVO)
            // ==============================================================================
            builder.Services.AddHttpContextAccessor(); // Permite acessar o usuário logado no interceptor
            builder.Services.AddScoped<NotificacaoAutomaticaInterceptor>();

            // ==============================================================================
            // 2. CONFIGURAÇÃO DO DBCONTEXT COM INTERCEPTOR
            // ==============================================================================
            builder.Services.AddDbContext<Models.AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

                // Injeta e adiciona o interceptor resolvido via DI
                var interceptor = sp.GetRequiredService<NotificacaoAutomaticaInterceptor>();
                options.AddInterceptors(interceptor);
            });

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Usuarios/AccessDenied/";
                    options.LoginPath = "/Usuarios/Login/";
                });

            // Injeção de Dependências
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<NoticiasService>();
            builder.Services.AddScoped<ArtigosService>();
            builder.Services.AddScoped<RecurrenceService>();
            builder.Services.AddScoped<BudgetService>();
            builder.Services.AddScoped<savemoney.Services.Interfaces.ITendenciaFinanceiraService,
                                      savemoney.Services.TendenciaFinanceiraService>();

            // Serviço de Notificações (Lógica de Negócio e Verificações R11)
            builder.Services.AddScoped<savemoney.Services.ServicoNotificacao>();
            builder.Services.AddScoped<savemoney.Services.ServicoExportacao>();

            var app = builder.Build();

            // ==============================================================================
            // 3. CONFIGURAÇÃO DE CULTURA (PT-BR) - Para aceitar vírgula em decimais
            // ==============================================================================
            var defaultDateCulture = "pt-BR";
            var ci = new CultureInfo(defaultDateCulture);
            ci.NumberFormat.NumberDecimalSeparator = ",";
            ci.NumberFormat.CurrencyDecimalSeparator = ",";

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo> { ci },
                SupportedUICultures = new List<CultureInfo> { ci }
            });
            // ==============================================================================

            // LICENÇA COMMUNITY DO QUESTPDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // app.UseHttpsRedirection(); // Mantido comentado conforme solicitado

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=LandingPage}/{action=Index}/{id?}");

            // MAPEAR RAZOR PAGES (corrige páginas que não eram alcançadas)
            app.MapRazorPages();

            app.Run();
        }
    }
}