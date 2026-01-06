using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes.CommonServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SmartPos.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]private object _currentView;
        [ObservableProperty]private HamburgerMenuItemCollection _menuItems;
        [ObservableProperty]private HamburgerMenuItem _selectedMenuItem;
        [ObservableProperty] private string _usuarioNombre;
        [ObservableProperty] private int _cajaId;
        [ObservableProperty] private DateTime _fechaActual = DateTime.Now;
        [ObservableProperty]private HamburgerMenuItemCollection _optionMenuItems;

        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonService _commonService;
        private DispatcherTimer _timer;
        public MainViewModel(IServiceProvider serviceProvider, ICommonService commonService)
        {
            _serviceProvider = serviceProvider;
            _commonService = commonService;

            MenuItems = new HamburgerMenuItemCollection();
            SetupTimer();

            OptionMenuItems = new HamburgerMenuItemCollection();
            ConfigurarOpcionesSistema();
        }

        partial void OnSelectedMenuItemChanged(HamburgerMenuItem value)
        {
            if (value != null) Navegar(value.Tag.ToString());
        }

        // Agregamos el manejo de las opciones inferiores
        [ObservableProperty]
        private HamburgerMenuItem _selectedOptionItem;

        partial void OnSelectedOptionItemChanged(HamburgerMenuItem value)
        {
            if (value == null) return;

            if (value.Tag.ToString() == "Logout")
            {
                CerrarSesion();
            }
            else
            {
                Navegar(value.Tag.ToString());
            }
        }

        private void CerrarSesion()
        {
            // 1. Detenemos el reloj para liberar recursos
            _timer?.Stop();

            // 2. Limpiamos la sesión en el Singleton
            _commonService.LimpiarSesion();

            // 3. Obtenemos el Login desde el contenedor de dependencias
            var loginView = _serviceProvider.GetRequiredService<SmartPos.Views.LoginView>();

            // 4. Mostramos el Login PRIMERO
            loginView.Show();

            // 5. Buscamos todas las instancias de MainWindow y las cerramos
            // Hacemos un .ToList() para evitar errores de "colección modificada"
            var ventanasACerrar = Application.Current.Windows
                .OfType<SmartPos.Views.MainWindow>()
                .ToList();

            foreach (var window in ventanasACerrar)
            {
                window.Close();
            }
        }

        private void ConfigurarOpcionesSistema()
        {
            OptionMenuItems.Clear();

            // Opción de Configuración (Solo si es Admin - puedes ajustar la lógica según tus DTOs)
            // Supongamos que verificas si el usuario tiene permiso de "Seguridad"
            OptionMenuItems.Add(new HamburgerMenuIconItem
            {
                Icon = "⚙️",
                Label = "Configuración",
                Tag = "Configuracion"
            });

            // Opción de Cerrar Sesión
            OptionMenuItems.Add(new HamburgerMenuIconItem
            {
                Icon = "🚪",
                Label = "Cerrar Sesión",
                Tag = "Logout"
            });
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // <-- Asegúrate de que no diga FromMinutes
            };
            _timer.Tick += (s, e) =>
            {
                // Al actualizar esta propiedad, el Binding del XAML reacciona
                FechaActual = DateTime.Now;
            };
            _timer.Start();
        }

        public void CargarMenus()
        {
            MenuItems.Clear();
            var accesos = _commonService.ObtenerPermisos();
            var MenuItemsTemporal = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem { Icon = "📦", Label = "Inventario", Tag = "Inventario" },
                new HamburgerMenuIconItem { Icon = "🛒", Label = "Ventas", Tag = "Ventas" },
                new HamburgerMenuIconItem { Icon = "🔒", Label = "Seguridad", Tag = "Seguridad" },

            };

            foreach (var acceso in accesos)
            {
                var nuevoMenu = MenuItemsTemporal
                    .OfType<HamburgerMenuIconItem>()
                    .FirstOrDefault(r => r.Label == acceso.PantallaId && acceso.Ver);

                MenuItems.Add(nuevoMenu);
            }

            // Por defecto, iniciamos en Inventario
            // 1. Cargar vista por defecto
            //SelectedMenuItem = (HamburgerMenuItem)MenuItems.FirstOrDefault();
            //Navegar(SelectedMenuItem.Label);

            var info = _commonService.GetRequestInfo();
            if (info != null)
            {
                UsuarioNombre = info.UsuarioId; // O el nombre completo si lo tienes
                CajaId = info.Caja;
            }
        }

        private void Navegar(string destino)
        {
            switch (destino)
            {
                case "Inventario":
                    CurrentView = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetServiceOrCreateInstance<InventarioViewModel>(_serviceProvider);
                    break;
                case "Seguridad":
                    CurrentView = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetServiceOrCreateInstance<SeguridadViewModel>(_serviceProvider);
                    break;
                case "Ventas":
                    // CurrentView = _serviceProvider.GetRequiredService<VentasViewModel>();
                    break;
            }
        }
    }
}
