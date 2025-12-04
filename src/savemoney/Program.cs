using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using savemoney.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using savemoney.Data.Interceptors;

namespace savemoney
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Data Protection (Produção)
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"h:\root\home\maiconvts-001\www\site1\DataProtection-Keys"));
            }

            // MVC + Razor
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            // HttpContext Accessor (necessário para Session no Layout)
            builder.Services.AddHttpContextAccessor();

            // Interceptor de Notificações
            builder.Services.AddScoped<NotificacaoAutomaticaInterceptor>();

            // Database
            builder.Services.AddDbContext<Models.AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                var interceptor = sp.GetRequiredService<NotificacaoAutomaticaInterceptor>();
                options.AddInterceptors(interceptor);
            });

            // Cookies Policy
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Autenticação
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Usuarios/AccessDenied/";
                    options.LoginPath = "/Usuarios/Login/";
                });

            // HttpClient
            builder.Services.AddHttpClient();

            // Services
            builder.Services.AddScoped<NoticiasService>();
            builder.Services.AddScoped<ArtigosService>();
            builder.Services.AddScoped<RecurrenceService>();
            builder.Services.AddScoped<BudgetService>();
            builder.Services.AddScoped<savemoney.Services.Interfaces.ITendenciaFinanceiraService,
                                      savemoney.Services.TendenciaFinanceiraService>();
            builder.Services.AddScoped<savemoney.Services.ServicoNotificacao>();
            builder.Services.AddScoped<savemoney.Services.ServicoExportacao>();

            // ========================================
            // SESSÃO - Contexto PF/PJ/Ambos
            // ========================================
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".SaveMoney.Session";
            });

            var app = builder.Build();

            // ========================================
            // CULTURA PT-BR
            // ========================================
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

            // QuestPDF License
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // ========================================
            // MIDDLEWARE PIPELINE
            // ========================================
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession(); // ← DEVE vir DEPOIS de UseAuthorization

            // Routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=LandingPage}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}