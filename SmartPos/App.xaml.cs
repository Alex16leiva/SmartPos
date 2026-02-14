using Aplicacion.Services;
using Aplicacion.Services.ArticuloServices;
using Aplicacion.Services.ClienteServices;
using Aplicacion.Services.ConfiTienda;
using Aplicacion.Services.Factura;
using Aplicacion.Services.Finanzas;
using Aplicacion.Services.FPagos;
using Aplicacion.Services.VendedorSevices;
using CrossCutting.Identity;
using CrossCutting.Network.Identity;
using Dominio.Context.Services;
using Infraestructura.Context;
using Infraestructura.Core;
using Infraestructura.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartPos.Comunes.CommonServices;
using SmartPos.Comunes.Extensions;
using SmartPos.ViewModels;
using SmartPos.Views;
using SmartPos.Views.Factura;
using System.IO;
using System.Windows;

namespace SmartPos
{
    public partial class App : System.Windows.Application
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


            string connectionString = configuration.GetConnectionString("ConnectionString");
            services.Configure<DataBaseSetting>(options =>
            {
                configuration.GetSection("DataBaseSetting").Bind(options);
            });

            // 2. Base de Datos: Cambiamos a Scoped
            services.AddDbContext<SmartPostDbContext>((sp, options) =>
            {
                var dbSettings = sp.GetRequiredService<IOptions<DataBaseSetting>>().Value;
                options.UseSqlServer(dbSettings.ConnectionString);
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
            services.AddTransient<ClienteViewModel>();

            // 6. Vistas
            services.AddTransient<Views.MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<FacturacionView>();
            services.AddTransient<InventarioView>();
            services.AddTransient<ClienteView>();


            // 7. ApplicationServices
            services.AddSingleton<ICommonService, CommonService>();
            services.AddScoped<ISecurityApplicationService, SecurityApplicationService>();
            services.AddScoped<IArticuloApplicationService, ArticuloApplicationService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IFacturaApplicationService, FacturaApplicationService>();
            services.AddScoped<IFacturaServicioDominio, FacturaServicioDominio>();
            services.AddScoped<IVendedorApplicationService, VendedorApplicationService>();
            services.AddScoped<IFinanzasApplicationService, FinanzasApplicationService>();
            services.AddScoped<IConfiguracionTiendaApplicationService, ConfiguracionTiendaApplicationService>();
            services.AddScoped<IFormasPagoApplicationService, FormasPagoApplicationService>();
            services.AddScoped<IClienteApplicationService, ClienteApplicationService>();

            // 8. REGISTRO AUTOMÁTICO DE SERVICIOS (Llamando al nuevo método)
            //services.AddApplicationServicesWithInterceptors();


            // 9. Configuración del Generador de Identidades (Correlativos)
            // Registramos el generador que hereda de IIdentityFactory
            // En tu caso, basándome en tu arquitectura, la clase suele llamarse 'IdentityGeneratorFactory'
            services.AddSingleton<IIdentityFactory, ADOIdentityGeneratorFactory>();

            // Construimos un contenedor temporal para inicializar la clase estática
            var serviceProvider = services.BuildServiceProvider();
            

            var identityFactory = serviceProvider.GetRequiredService<IIdentityFactory>();
            IdentityFactory.SetCurrent(identityFactory);

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            // BUSCAMOS EL SELLO EN TODA LA CADENA (Recursivo)
            //bool yaFueRegistrado = false;
            //Exception exCheck = e.Exception;

            //while (exCheck != null)
            //{
            //    if (exCheck.Data.Contains("Logged"))
            //    {
            //        yaFueRegistrado = true;
            //        break;
            //    }
            //    exCheck = exCheck.InnerException;
            //}

            //if (!yaFueRegistrado)
            //{
            //    // Si no tiene el sello, es un error de UI puro (no pasó por servicios)
            //    var logService = App.ServiceProvider.GetRequiredService<ILogService>();
            //    var exParaLog = e.Exception.GetBaseException();

            //    Task.Run(async () => {
            //        await logService.LogErrorAsync("WPF_UI", "Global", exParaLog, "System");
            //    });
            //}

            //// Mostrar siempre la notificación con tu servicio común
            //var commonService = App.ServiceProvider.GetRequiredService<ICommonService>();

            //string mensajeAmigable = "### ¡Ups! Algo no salió como esperábamos\n\n" +
            //                 "El sistema ha experimentado un inconveniente técnico. " +
            //                 "No te preocupes, el detalle ha sido enviado automáticamente al equipo de soporte.\n\n" +
            //                 "**Acción:** Por favor, intenta realizar la operación nuevamente o contacta al administrador.";

            //commonService.ShowError(mensajeAmigable);
        }

    }
}
