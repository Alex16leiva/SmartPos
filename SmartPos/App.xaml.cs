using Aplicacion.Services;
using Aplicacion.Services.ArticuloServices;
using Aplicacion.Services.Factura;
using Aplicacion.Services.VendedorSevices;
using Dominio.Context.Services;
using Infraestructura.Context;
using Infraestructura.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using SmartPos.Comunes.CommonServices;
using SmartPos.Comunes.Extensions;
using SmartPos.ViewModels;
using SmartPos.Views;
using SmartPos.Views.Factura;
using System.IO;
using System.Windows;

namespace SmartPos
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Iniciamos la ventana principal a través del ServiceProvider
            var mainWindow = ServiceProvider.GetRequiredService<Views.MainWindow>();


            var loginView = new LoginView();
            loginView.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. Configuración (appsettings.json)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            services.AddSingleton<IConfiguration>(configuration);


            string connectionString = configuration.GetConnectionString("conectionDataBase");
            // 2. Base de Datos: Cambiamos a Scoped
            services.AddDbContext<SmartPostDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            }, ServiceLifetime.Scoped);

            // 3. Mapeo de Interfaz: Scoped
            services.AddScoped<IDataContext>(sp => sp.GetRequiredService<SmartPostDbContext>());

            // 4. Repositorios: Scoped
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 5. ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<InventarioViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<SeguridadViewModel>();
            services.AddTransient<FacturacionViewModel>();

            // 6. Vistas
            services.AddTransient<Views.MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<FacturacionView>();


            // 7. ApplicationServices
            services.AddSingleton<ICommonService, CommonService>();
            services.AddScoped<ISecurityApplicationService, SecurityApplicationService>();
            services.AddScoped<IArticuloApplicationService, ArticuloApplicationService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IFacturaApplicationService, FacturaApplicationService>();
            services.AddScoped<IFacturaServicioDominio, FacturaServicioDominio>();
            services.AddScoped<IVendedorApplicationService, VendedorApplicationService>();

            // 8. REGISTRO AUTOMÁTICO DE SERVICIOS (Llamando al nuevo método)
            services.AddApplicationServicesWithInterceptors();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            // BUSCAMOS EL SELLO EN TODA LA CADENA (Recursivo)
            bool yaFueRegistrado = false;
            Exception exCheck = e.Exception;

            while (exCheck != null)
            {
                if (exCheck.Data.Contains("Logged"))
                {
                    yaFueRegistrado = true;
                    break;
                }
                exCheck = exCheck.InnerException;
            }

            if (!yaFueRegistrado)
            {
                // Si no tiene el sello, es un error de UI puro (no pasó por servicios)
                var logService = App.ServiceProvider.GetRequiredService<ILogService>();
                var exParaLog = e.Exception.GetBaseException();

                Task.Run(async () => {
                    await logService.LogErrorAsync("WPF_UI", "Global", exParaLog, "System");
                });
            }

            // Mostrar siempre la notificación con tu servicio común
            var commonService = App.ServiceProvider.GetRequiredService<ICommonService>();

            string mensajeAmigable = "### ¡Ups! Algo no salió como esperábamos\n\n" +
                             "El sistema ha experimentado un inconveniente técnico. " +
                             "No te preocupes, el detalle ha sido enviado automáticamente al equipo de soporte.\n\n" +
                             "**Acción:** Por favor, intenta realizar la operación nuevamente o contacta al administrador.";

            commonService.ShowError(mensajeAmigable);
        }

    }
}
