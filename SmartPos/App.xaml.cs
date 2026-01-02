using Aplicacion.Services;
using Aplicacion.Services.ArticuloServices;
using Infraestructura.Context;
using Infraestructura.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // 6. Vistas
            services.AddTransient<Views.MainWindow>();
            services.AddTransient<LoginView>();

            // 7. ApplicationServices
            services.AddSingleton<ICommonService, CommonService>();
            services.AddTransient<SecurityAplicationService>();
            services.AddTransient<IArticuloApplicationService, ArticuloApplicationService>();
            services.AddTransient<ILogService, LogService>();

            // 8. REGISTRO AUTOMÁTICO DE SERVICIOS (Llamando al nuevo método)
            services.AddApplicationServicesWithInterceptors();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            // Buscamos si el sello existe en la excepción actual o en alguna interna
            bool yaEstaRegistrado = false;
            Exception exAux = e.Exception;

            while (exAux != null)
            {
                if (exAux.Data.Contains("Logged"))
                {
                    yaEstaRegistrado = true;
                    break;
                }
                exAux = exAux.InnerException;
            }

            if (!yaEstaRegistrado)
            {
                // Solo aquí registramos si nadie más lo hizo
                var logService = App.ServiceProvider.GetRequiredService<ILogService>();
                Task.Run(async () => await logService.LogErrorAsync("WPF_Global", "UI", e.Exception, "System"));
            }

            // Notificación siempre visible
            var commonService = App.ServiceProvider.GetRequiredService<ICommonService>();
            commonService.ShowError( e.Exception.Message);
        }

    }
}
