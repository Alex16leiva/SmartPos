using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls;
using SmartPos.Comunes.CommonServices;

namespace SmartPos.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]private object _currentView;
        [ObservableProperty]private HamburgerMenuItemCollection _menuItems;
        [ObservableProperty]private HamburgerMenuItem _selectedMenuItem;
        [ObservableProperty] private string _usuarioNombre;
        [ObservableProperty] private int _cajaId;
        [ObservableProperty]private bool _isBienvenidaVisible = true;

        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonService _commonService;
        public MainViewModel(IServiceProvider serviceProvider, ICommonService commonService)
        {
            _serviceProvider = serviceProvider;
            _commonService = commonService;

            MenuItems = new HamburgerMenuItemCollection();
        }

        public void CargarMenus()
        {
            
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



        // Este método se dispara automáticamente cuando cambia SelectedMenuItem
        partial void OnSelectedMenuItemChanged(HamburgerMenuItem value)
        {
            if (value != null)
            {
                Navegar(value.Tag.ToString());
            }
        }

        private void Navegar(string destino)
        {
            IsBienvenidaVisible = false;
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
