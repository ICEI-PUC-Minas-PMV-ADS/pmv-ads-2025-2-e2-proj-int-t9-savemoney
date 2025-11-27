using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using savemoney.Services;
using System.Globalization; // <--- ADICIONADO
using Microsoft.AspNetCore.Localization; // <--- ADICIONADO

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

            builder.Services.AddDbContext<Models.AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

            var app = builder.Build();

            // ==============================================================================
            // 1. CONFIGURAÇÃO DE CULTURA (PT-BR) - Para aceitar vírgula em decimais
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