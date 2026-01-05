using Aplicacion.Services;
using Aplicacion.Services.ArticuloServices;
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
            // 2. Base de Datos (Transient para que cada repo tenga su propio context)
            services.AddDbContext<SmartPostDbContext>(options => options.UseSqlServer(connectionString), 
                ServiceLifetime.Transient);

            // 3. Mapeo de Interfaz (DEBE SER TRANSIENT TAMBIÉN)
            services.AddTransient<IDataContext>(sp => sp.GetRequiredService<SmartPostDbContext>());

            // 4. Repositorios (DEBE SER TRANSIENT TAMBIÉN)
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 5. ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<InventarioViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<SeguridadViewModel>();

            // 6. Vistas
            services.AddTransient<Views.MainWindow>();
            services.AddTransient<LoginView>();


            // 7. ApplicationServices
            services.AddSingleton<ICommonService, CommonService>();
            services.AddTransient<ISecurityApplicationService, SecurityApplicationService>();
            services.AddTransient<IArticuloApplicationService, ArticuloApplicationService>();
            services.AddTransient<ILogService, LogService>();

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
