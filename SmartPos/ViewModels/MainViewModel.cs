using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls;

namespace SmartPos.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;
        [ObservableProperty]
        private HamburgerMenuItemCollection _menuItems;

        [ObservableProperty]
        private HamburgerMenuItem _selectedMenuItem;

        private readonly IServiceProvider _serviceProvider;
        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // 1. Inicializar los items del menú
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem { Icon = "📦", Label = "Inventario", Tag = "Inventario" },
                new HamburgerMenuIconItem { Icon = "🛒", Label = "Ventas", Tag = "Ventas" }
            };

            // Por defecto, iniciamos en Inventario
            // 2. Cargar vista por defecto
            SelectedMenuItem = (HamburgerMenuItem)MenuItems.FirstOrDefault();
            Navegar(SelectedMenuItem.Label);
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
            switch (destino)
            {
                case "Inventario":
                    CurrentView = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetServiceOrCreateInstance<InventarioViewModel>(_serviceProvider);
                    break;
                case "Ventas":
                    // CurrentView = _serviceProvider.GetRequiredService<VentasViewModel>();
                    break;
            }
        }
    }
}
