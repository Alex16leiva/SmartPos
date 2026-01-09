using Aplicacion.DTOs.Factura;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartPos.Views.Factura
{
    /// <summary>
    /// Interaction logic for FacturacionView.xaml
    /// </summary>
    public partial class FacturacionView : UserControl
    {
        public FacturacionView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<FacturacionViewModel>();
            Loaded += (s, e) => TxtBusqueda.Focus();
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Pequeño delay para dejar que el Binding actualice el ViewModel primero
                Dispatcher.BeginInvoke(new Action(() => {
                    TxtBusqueda.Focus();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Esperamos un milisegundo a que el binding actualice el DTO
            Dispatcher.BeginInvoke(new Action(() => {
                if (DGTable.SelectedItem.IsNull())
                {
                    var dgItemS = (ObservableCollection<FacturaDetalleDTO>)DGTable.ItemsSource;
                    DGTable.ItemsSource = dgItemS;
                }
                var vm = (FacturacionViewModel)this.DataContext;
                vm.FacturaDetalle = (ObservableCollection<FacturaDetalleDTO>)DGTable.ItemsSource;
                vm.ActualizacionDeDataGrid();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
