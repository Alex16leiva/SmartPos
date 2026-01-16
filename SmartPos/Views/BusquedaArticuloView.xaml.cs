using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SmartPos.Views
{
    /// <summary>
    /// Interaction logic for BusquedaArticuloView.xaml
    /// </summary>
    public partial class BusquedaArticuloView : MetroWindow
    {
        public BusquedaArticuloView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetService<FacturacionViewModel>();
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (FacturacionViewModel)this.DataContext;
            if (viewModel.SeleccionarClienteCommand.CanExecute(null))
            {
                viewModel.SeleccionarClienteCommand.Execute(null);
            }

            this.DialogResult = true;
            this.Close();
        }

        
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = (FacturacionViewModel)this.DataContext;
            if (viewModel.SeleccionarClienteCommand.CanExecute(null))
            {
                viewModel.SeleccionarClienteCommand.Execute(null);
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
