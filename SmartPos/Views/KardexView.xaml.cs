using Aplicacion.Services.ArticuloServices;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for KardexView.xaml
    /// </summary>
    public partial class KardexView : MetroWindow
    {
        private readonly string _articuloId;
        public KardexView(string articuloId)
        {
            InitializeComponent();
            _articuloId = articuloId;
            TxtNombreArticulo.Text = articuloId;

            CargarDatos();
        }

        private async void CargarDatos()
        {            
            var service = App.ServiceProvider.GetRequiredService<IArticuloApplicationService>();
            var data = await service.ObtenerKardexArticuloAsync(_articuloId);
            DgKardex.ItemsSource = data.InventarioMovimientos;

            // El último saldo en la lista (que está ordenada por fecha DESC) es el stock actual
            TxtStockActual.Text = data.Articulo.Cantidad.ToString("N2") ?? "0.00";            
        }
    }
}
